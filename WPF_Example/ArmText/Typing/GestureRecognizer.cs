using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.ComponentModel;
using System.Text.RegularExpressions;
using ArmText.Util;
using System.Windows.Input;

namespace ArmText.Typing
{
  public class GestureRecognizer : INotifyPropertyChanged
  {

    //Lateral movement is given in percentage of the layout, from [0..1)
    private const double LATERAL_THRESHOLD_PUSH = 0.02;
    //Depth is given in meters
    private const double DEPTH_THRESHOLD = 0.01;
    //Lateral movement is given in percentage of the layout, from [0..1)
    private const double LATERAL_THRESHOLD_TIMER = 0.01;
    //Lateral movement in the second hand is a distance in meters from the hip
    private const double LATERAL_THRESHOLD_SECOND_HAND = 0.2;

    private Point3D lastPos = new Point3D(0, 0, 0);
    private StringBuilder movementSequenceSB = new StringBuilder();
    private List<InputInfo> movementSequence = new List<InputInfo>(120);

    //Variables for the SelectionMethod.Push selection
    private static readonly String PRESSING_REGEX_PUSH = "(M|S){1}(P){3}(P)*(S|M|R){4}(S|M|R)*";     //move or steady once, released for 3 frames, Stop Move or press for 4 times
    private static readonly String RELEASING_REGEX_PUSH = "(P|M|S){1}(R){3}(R)*(S|M|P){4}(S|M|P)*";
    //private static readonly String TAP_REGEX_CRANK = "(M|S)(P){2,10}(S|M|P){0,10}(R)+(M|S)";
    private static readonly String TAP_REGEX_PUSH = "(M|S)(P){3,10}(S|M|P){0,10}(R)*(M|S)";
    private Regex regexTapCrank = new Regex(TAP_REGEX_PUSH);
    private Regex regexPressCrank = new Regex(PRESSING_REGEX_PUSH);
    private Regex regexReleaseCrank = new Regex(RELEASING_REGEX_PUSH);

    //Variables for the SelectionMethod.Timer selection
    private static readonly String REGEX_START_TIMER = "(M|S){15}";
    private static readonly String TAP_REGEX_TIMER = "(M|S){18}";
    private Regex regexStartTimer = new Regex(REGEX_START_TIMER);
    private Regex regexTapTimer = new Regex(TAP_REGEX_TIMER);

    //Variables for the SelectionMethod.Swipe selection
    private static readonly String REGEX_START_SWIPE = "(M|S){15}";
    private static readonly String TAP_REGEX_SWIPE = "(M)(S){5,10}(M|S){0,3}(S){3,10}";
    private Regex regexStartSwipe = new Regex(REGEX_START_SWIPE);
    private Regex regexTapSwipe = new Regex(TAP_REGEX_SWIPE);

    private TimerState timerState = TimerState.Nothing;

    public String MovementSequence
    {
      get { return movementSequenceSB.ToString(); }
    }

    public TimerState TimerState
    {
      get { return timerState; }
      set
      {
        if (timerState == value)
          return;
        timerState = value;
        OnPropertyChanged("TimerState");
      }
    }

    internal ICollection<TypingGesture> ProcessGestures(Skeleton skeleton, double deltaTimeMilliseconds, Point3D cursor, Point3D secondaryCursor,
      SelectionMethod selectionM, Key highlightedKey, bool userClicked)
    {
      List<TypingGesture> gestures = null;
      if (selectionM == SelectionMethod.Push)
        gestures = ProcessGesturesPush(cursor);
      else if (selectionM == SelectionMethod.Swipe)
        gestures = ProcessGesturesSwipe(cursor);
      else if (selectionM == SelectionMethod.Timer)
        gestures = ProcessGesturesTimer(cursor, deltaTimeMilliseconds, highlightedKey);
      else if (selectionM == SelectionMethod.SecondHand)
        gestures = ProcessGesturesSecondHand(cursor, secondaryCursor, deltaTimeMilliseconds, highlightedKey);
      else if (selectionM == SelectionMethod.Click)
        gestures = ProcessGesturesClick(cursor, userClicked);
      return gestures;
    }

    private List<TypingGesture> ProcessGesturesPush(Point3D cursor)
    {
      if (cursor.X == -1 || cursor.Y == -1)
        return null;

      List<TypingGesture> gestures = new List<TypingGesture>();
      InputInfo info = new InputInfo() { Position = cursor };
      System.Windows.Media.Media3D.Vector3D displacement = cursor - lastPos;
      lastPos = cursor;

      if (movementSequenceSB.Length == movementSequence.Capacity)
      {
        movementSequenceSB.Remove(0, 1);
        movementSequence.RemoveAt(0);
      }

      if (displacement.X > LATERAL_THRESHOLD_PUSH || displacement.Y > LATERAL_THRESHOLD_PUSH)
        info.Movement = 'M'; //moving
      else if (Math.Abs(displacement.Z) > DEPTH_THRESHOLD)  //doing some movement on depth
      {
        if (displacement.Z < 0)
          info.Movement = 'P'; //released
        else
          info.Movement = 'R'; //pressed
      }
      else
        info.Movement = 'S'; //steady --- not moving
      movementSequenceSB.Append(info.Movement);
      movementSequence.Add(info);


      String tmpSequence = MovementSequence;
      MatchCollection taps = regexTapCrank.Matches(tmpSequence);
      if (taps.Count > 0)
      {
        Match tap = taps[0];
        gestures.Add(new TypingGesture()
        {
          Time = DateTime.Now,
          Type = GestureType.Tap,
          Position = movementSequence[tap.Index].Position
        });
        movementSequenceSB.Remove(tap.Index, tap.Length);
        movementSequence.RemoveRange(tap.Index, tap.Length);
      }
      OnPropertyChanged("MovementSequence");

      return gestures;
    }

    private List<TypingGesture> ProcessGesturesSwipe(Point3D cursor)
    {
      if (cursor.X == -1 || cursor.Y == -1)
        return null;

      List<TypingGesture> gestures = new List<TypingGesture>();
      InputInfo info = new InputInfo() { Position = cursor };
      lastPos = cursor;

      if (movementSequenceSB.Length == movementSequence.Capacity)
      {
        movementSequenceSB.Remove(0, 1);
        movementSequence.RemoveAt(0);
      }

      if (cursor.IsFrozen)
        info.Movement = 'S'; //steady --- not moving
      else
        info.Movement = 'M'; //moving
      movementSequenceSB.Append(info.Movement);
      movementSequence.Add(info);

      String tmpSequence = MovementSequence;
      if (timerState == ArmText.Typing.TimerState.Nothing)
      {
        MatchCollection starts = regexStartSwipe.Matches(tmpSequence);
        if (starts.Count > 0)
        {
          Match start = starts[0];
          TimerState = ArmText.Typing.TimerState.Running;
          secondHandSelectionInfo = movementSequence[start.Index];
          movementSequenceSB.Remove(start.Index, start.Length);
          movementSequence.RemoveRange(start.Index, start.Length);
        }
      }
      else if (timerState == ArmText.Typing.TimerState.Running)
      {
        MatchCollection taps = regexTapSwipe.Matches(tmpSequence);
        if (taps.Count > 0)
        {
          Match tap = taps[0];
          gestures.Add(new TypingGesture()
          {
            Time = DateTime.Now,
            Type = GestureType.Tap,
            Position = movementSequence[tap.Index].Position
          });
          TimerState = ArmText.Typing.TimerState.Nothing;
          movementSequenceSB.Remove(tap.Index, tap.Length);
          movementSequence.RemoveRange(tap.Index, tap.Length);
        }
      }
      OnPropertyChanged("MovementSequence");

      return gestures;
    }

    private List<TypingGesture> ProcessGesturesTimer(Point3D cursor, double delta, Key highlightedKey)
    {
      if (cursor.X == -1 || cursor.Y == -1)
        return null;

      List<TypingGesture> gestures = new List<TypingGesture>();
      InputInfo info = new InputInfo() { Position = cursor };
      System.Windows.Media.Media3D.Vector3D displacement = cursor - lastPos;
      lastPos = cursor;

      if (movementSequenceSB.Length == movementSequence.Capacity)
      {
        movementSequenceSB.Remove(0, 1);
        movementSequence.RemoveAt(0);
      }

      if (Math.Abs(displacement.X) > LATERAL_THRESHOLD_TIMER || Math.Abs(displacement.Y) > LATERAL_THRESHOLD_TIMER)
        info.Movement = 'M'; //moving
      else
        info.Movement = 'S'; //steady --- not moving
      movementSequenceSB.Append(info.Movement);
      movementSequence.Add(info);

      String tmpSequence = MovementSequence;
      if (timerState == ArmText.Typing.TimerState.Nothing)
      {
        MatchCollection starts = regexStartTimer.Matches(tmpSequence);
        if (starts.Count > 0)
        {
          Match start = starts[0];
          TimerState = ArmText.Typing.TimerState.Running;
          movementSequenceSB.Remove(start.Index, start.Length);
          movementSequence.RemoveRange(start.Index, start.Length);
        }
      }
      else if (timerState == ArmText.Typing.TimerState.Running)
      {
        MatchCollection selections = regexTapTimer.Matches(tmpSequence);
        if (selections.Count > 0)
        {
          Match selection = selections[0];
          gestures.Add(new TypingGesture()
          {
            Time = DateTime.Now,
            Type = GestureType.Tap,
            Position = movementSequence[selection.Index].Position
          });
          TimerState = ArmText.Typing.TimerState.Nothing;
          movementSequenceSB.Remove(selection.Index, selection.Length);
          movementSequence.RemoveRange(selection.Index, selection.Length);
        }
      }
      OnPropertyChanged("MovementSequence");

      return gestures;
    }

    private InputInfo secondHandSelectionInfo;
    private DateTime lastSecondHandSelection = DateTime.MinValue;
    private TimeSpan secondaryHandSelectionTimeDelta = new TimeSpan(0, 0, 2);

    private List<TypingGesture> ProcessGesturesSecondHand(Point3D cursor, Point3D secondaryCursor, double deltaTimeMilliseconds, Key highlightedKey)
    {
      if (cursor.X == -1 || cursor.Y == -1)
        return null;

      List<TypingGesture> gestures = new List<TypingGesture>();
      InputInfo info = new InputInfo() { Position = cursor };
      System.Windows.Media.Media3D.Vector3D displacement = cursor - lastPos;
      lastPos = cursor;
      if (movementSequenceSB.Length == movementSequence.Capacity)
      {
        movementSequenceSB.Remove(0, 1);
        movementSequence.RemoveAt(0);
      }
      if (displacement.X > LATERAL_THRESHOLD_TIMER || displacement.Y > LATERAL_THRESHOLD_TIMER)
        info.Movement = 'M'; //moving
      else
        info.Movement = 'S'; //steady --- not moving
      movementSequenceSB.Append(info.Movement);
      movementSequence.Add(info);

      if (timerState == ArmText.Typing.TimerState.Nothing)
      {
        String tmpSequence = MovementSequence;
        MatchCollection starts = regexStartTimer.Matches(tmpSequence);
        if (starts.Count > 0)
        {
          Match start = starts[0];
          TimerState = ArmText.Typing.TimerState.Running;
          secondHandSelectionInfo = movementSequence[start.Index];
          movementSequenceSB.Remove(start.Index, start.Length);
          movementSequence.RemoveRange(start.Index, start.Length);
        }
      }
      else if (timerState == ArmText.Typing.TimerState.Running)
      {
        if (secondaryCursor.X > LATERAL_THRESHOLD_SECOND_HAND && DateTime.Now > lastSecondHandSelection + secondaryHandSelectionTimeDelta)
        {
          gestures.Add(new TypingGesture()
          {
            Time = DateTime.Now,
            Type = GestureType.Tap,
            Position = secondHandSelectionInfo.Position
          });
          lastSecondHandSelection = DateTime.Now;
          TimerState = ArmText.Typing.TimerState.Nothing;
        }
      }
      OnPropertyChanged("MovementSequence");

      return gestures;
    }

    private List<TypingGesture> ProcessGesturesClick(Point3D cursor, bool userClicked)
    {
      if (cursor.X == -1 || cursor.Y == -1)
        return null;

      List<TypingGesture> gestures = new List<TypingGesture>();
      if (!userClicked)
        return gestures;

      gestures.Add(new TypingGesture()
      {
        Position = cursor,
        Type = GestureType.Tap,
        Time = DateTime.Now
      });
      return gestures;
    }

    internal void TypingEngine_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ("SelectionMethod".Equals(e.PropertyName))
      {
        movementSequenceSB.Clear();
        movementSequence.Clear();
        TimerState = ArmText.Typing.TimerState.Nothing;
      }
      else if ("HighlightedKey".Equals(e.PropertyName))
      {
        movementSequenceSB.Clear();
        movementSequence.Clear();
        TimerState = ArmText.Typing.TimerState.Nothing;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(String name)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(name));
    }

    private class InputInfo
    {
      public Point3D Position { get; set; }
      public char Movement { get; set; }
    }

  }
}
