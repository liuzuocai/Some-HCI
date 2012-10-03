using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.ComponentModel;
using ArmText.Util;
using ArmText.Typing;

namespace ArmText.Pointing
{
  public class LabyrinthTypingMethod : TypingMethod, INotifyPropertyChanged
  {
    private bool started;
    private int layoutRows = 6;
    private int layoutCols = 6;
    private int currentSequenceIndex;
    private int previousPointingIndex;
    private int currentPointingIndex;
    private double errorCount;
    private double correctCount;
    private Key[] layout = { 
                                Key.A,     Key.B,        Key.C,        Key.D,         Key.E,         Key.F,
                                Key.G,     Key.H,        Key.I,        Key.J,         Key.K,         Key.L,
                                Key.M,     Key.N,        Key.O,        Key.P,         Key.Q,         Key.R,
                                Key.S,     Key.T,        Key.U,        Key.V,         Key.W,         Key.X,
                                Key.Y,     Key.Z,        Key.OemComma, Key.OemPeriod, Key.Separator, Key.OemMinus,
                                Key.Space, Key.Multiply, Key.OemPlus,     Key.Left,      Key.OemQuestion,     Key.Right
                            };
    
    private Key[] sequence = {
                                Key.A,   Key.B,        Key.C,        Key.D,         Key.E,         Key.F,
                                Key.L,   Key.R,        Key.X,        Key.OemMinus,  Key.Right,     Key.OemQuestion, 
                                Key.Left,Key.OemPlus,     Key.Multiply, Key.Space,     Key.Y,         Key.S, 
                                Key.M,   Key.G,        Key.H,        Key.I,         Key.J,         Key.K,
                                Key.Q,   Key.W,        Key.Separator,Key.OemPeriod, Key.OemComma,  Key.Z,
                                Key.T,   Key.N,        Key.O,        Key.P,         Key.V,         Key.U
                             };                                          
    private Random randon = new Random((int)DateTime.Now.Ticks);
    private Array allKeys = null;

    public LabyrinthTypingMethod()
    {
      currentSequenceIndex = 0;
      allKeys = Enum.GetValues(typeof(Key));
      previousPointingIndex = -1;
      errorCount = 0;
      correctCount = 0;
      started = false;
    }

    public override TypingStatus ProcessNewFrame(Point3D cursor, ICollection<TypingGesture> gestures, Microsoft.Kinect.Skeleton stableSkeleton, double deltaTimeMilliseconds, TypingDexterity dexterity)
    {
      double columnData = cursor.X;
      double rowData = cursor.Y;
      double constraintData = cursor.Z;

      if (columnData < 0 || rowData < 0)
        return new TypingStatus();

      double stepX = 1.0 / layoutCols;
      double stepY = 1.0 / layoutRows;

      int col = (int)(columnData / stepX);
      int row = (int)(rowData / stepY);
      currentPointingIndex = row * layoutCols + col;
      TypingStatus status = new TypingStatus();
      if (currentSequenceIndex == 36)
      {
        return status;
      }
      if (layout[currentPointingIndex] == sequence[currentSequenceIndex])
      {
        status.HighlightedKey = layout[currentPointingIndex];
        if (currentSequenceIndex < sequence.Length)
        {
          currentSequenceIndex++;
          if (!started)
            started = true;
          else
            correctCount++;
        }
      }
      else 
      {
        if (currentPointingIndex != previousPointingIndex)
        {
          if (started)
          {
            errorCount++;
          }
        }
      }

      if (gestures != null && gestures.Count > 0 && gestures.ElementAt(0).Type == GestureType.Tap)
      {
        TypingGesture gesture = gestures.ElementAt(0);
        col = (int)(gesture.Position.X / stepX);
        row = (int)(gesture.Position.Y / stepY);
        currentPointingIndex = row * layoutCols + col;
        status.SelectedKey = layout[currentPointingIndex];
      }

      previousPointingIndex = currentPointingIndex;
      return status;
    }

    public override void Reset()
    {
      base.Reset();
      currentSequenceIndex = 0;
      started = false;
      errorCount = 0;
      correctCount = 0;
    }

    public override string ToString()
    {
      return "Labyrinth";
    }

  }
}
