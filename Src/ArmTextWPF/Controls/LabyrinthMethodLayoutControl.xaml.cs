using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ArmText.Util;
using ArmText.Typing;

namespace ArmText.Controls
{
  /// <summary>
  /// Interaction logic for LabyrinthLayoutControl.xaml
  /// </summary>
  public partial class LabyrinthMethodLayoutControl : UserControl
  {
    public static readonly DependencyProperty CursorPositionProperty = DependencyProperty.Register("CursorPosition", typeof(Point3D), typeof(LabyrinthMethodLayoutControl));
    public static readonly DependencyProperty HighlightedKeyProperty = DependencyProperty.Register("HighlightedKey", typeof(Key), typeof(LabyrinthMethodLayoutControl));
    public static readonly DependencyProperty SelectedKeyProperty = DependencyProperty.Register("SelectedKey", typeof(Key), typeof(LabyrinthMethodLayoutControl));
    public static readonly DependencyProperty ShowTimerProperty = DependencyProperty.Register("ShowTimer", typeof(bool), typeof(LabyrinthMethodLayoutControl));
    public static readonly DependencyProperty ResetBGProperty = DependencyProperty.Register("ResetBG", typeof(bool), typeof(LabyrinthMethodLayoutControl));
    public static readonly DependencyProperty TimerStateProperty = DependencyProperty.Register("TimerState", typeof(TimerState), typeof(LabyrinthMethodLayoutControl));

    public Point3D CursorPosition
    {
      get { return (Point3D)GetValue(CursorPositionProperty); }
      set { SetValue(CursorPositionProperty, value); }
    }

    public Key HighlightedKey
    {
      get { return (Key)GetValue(HighlightedKeyProperty); }
      set { SetValue(HighlightedKeyProperty, value); }
    }

    public Key SelectedKey
    {
      get { return (Key)GetValue(SelectedKeyProperty); }
      set { SetValue(SelectedKeyProperty, value); }
    }

    public bool ShowTimer
    {
      get { return (bool)GetValue(ShowTimerProperty); }
      set { SetValue(ShowTimerProperty, value); }
    }

    public bool ResetBG
    {
      get { return (bool)GetValue(ResetBGProperty); }
      set { SetValue(ResetBGProperty, value); }
    }

    public TimerState TimerState
    {
      get { return (TimerState)GetValue(TimerStateProperty); }
      set { SetValue(TimerStateProperty, value); }
    }

    public LabyrinthMethodLayoutControl()
    {
      InitializeComponent();
      HighlightedKey = Key.None;
      SelectedKey = Key.None;
    }

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
      base.OnPropertyChanged(e);
      if (!this.Visibility.Equals(Visibility.Visible))
        return;
      if (e.Property == LabyrinthMethodLayoutControl.CursorPositionProperty)
      {
        Canvas.SetLeft(eCursor, ActualWidth * CursorPosition.X - eCursor.ActualWidth / 2);
        Canvas.SetTop(eCursor, ActualHeight * CursorPosition.Y - eCursor.ActualHeight / 2);
      }
    }

    internal void Reset()
    {
      ResetBG = true; //triggers the databinding process
      ResetBG = false; //triggers it again but this time the reset is ignored
    }

  }
}
