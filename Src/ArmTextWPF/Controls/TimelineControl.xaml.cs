using System;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ArmText.Controls
{
  /// <summary>
  /// Interaction logic for TimelineControl.xaml
  /// </summary>
  public partial class TimelineControl : UserControl
  {

    public static readonly DependencyProperty DeltaProperty = DependencyProperty.Register("Delta", typeof(double), typeof(TimelineControl));
    public static readonly DependencyProperty GraphValueOneProperty = DependencyProperty.Register("GraphValueOne", typeof(double), typeof(TimelineControl));
    public static readonly DependencyProperty GraphValueTwoProperty = DependencyProperty.Register("GraphValueTwo", typeof(double), typeof(TimelineControl));
    public static readonly DependencyProperty GraphValueThreeProperty = DependencyProperty.Register("GraphValueThree", typeof(double), typeof(TimelineControl));
    public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(double), typeof(TimelineControl));
    public static readonly DependencyProperty LenghtInMinutesProperty = DependencyProperty.Register("LenghtInMinutes", typeof(double), typeof(TimelineControl));
    public static readonly DependencyProperty DynamicLenghtProperty = DependencyProperty.Register("DynamicLenght", typeof(bool), typeof(TimelineControl));
    public static readonly DependencyProperty GraphValueReferenceProperty = DependencyProperty.Register("GraphValueReference", typeof(double), typeof(TimelineControl));
    public static readonly DependencyProperty CurrentFrameProperty = DependencyProperty.Register("CurrentFrame", typeof(int), typeof(TimelineControl));
    public static readonly DependencyProperty ValueOneVisibleProperty = DependencyProperty.Register("ValueOneVisible", typeof(Visibility), typeof(TimelineControl));
    public static readonly DependencyProperty ValueTwoVisibleProperty = DependencyProperty.Register("ValueTwoVisible", typeof(Visibility), typeof(TimelineControl));
    public static readonly DependencyProperty ValueThreeVisibleProperty = DependencyProperty.Register("ValueThreeVisible", typeof(Visibility), typeof(TimelineControl));

    public static readonly DependencyProperty ScaleXProperty = DependencyProperty.Register("ScaleX", typeof(double), typeof(TimelineControl));
    public static readonly DependencyProperty ScaleYProperty = DependencyProperty.Register("ScaleY", typeof(double), typeof(TimelineControl));

    public static readonly DependencyProperty ShiftXProperty = DependencyProperty.Register("ShiftX", typeof(double), typeof(TimelineControl));
    public static readonly DependencyProperty ShiftYProperty = DependencyProperty.Register("ShiftY", typeof(double), typeof(TimelineControl));

    public static readonly DependencyProperty EngineRunningProperty = DependencyProperty.Register("EngineRunning", typeof(bool), typeof(TimelineControl));

    public double Delta
    {
      get { return (double)GetValue(DeltaProperty); }
      set { SetValue(DeltaProperty, value); }
    }

    public double GraphValueOne
    {
      get { return (double)GetValue(GraphValueOneProperty); }
      set { SetValue(GraphValueOneProperty, value); }
    }

    public double GraphValueTwo
    {
      get { return (double)GetValue(GraphValueTwoProperty); }
      set { SetValue(GraphValueTwoProperty, value); }
    }

    public double GraphValueThree
    {
      get { return (double)GetValue(GraphValueThreeProperty); }
      set { SetValue(GraphValueThreeProperty, value); }
    }

    public Visibility ValueOneVisible
    {
      get { return (Visibility)GetValue(ValueOneVisibleProperty); }
      set { SetValue(ValueOneVisibleProperty, value); }
    }

    public Visibility ValueTwoVisible
    {
      get { return (Visibility)GetValue(ValueTwoVisibleProperty); }
      set { SetValue(ValueTwoVisibleProperty, value); }
    }

    public Visibility ValueThreeVisible
    {
      get { return (Visibility)GetValue(ValueThreeVisibleProperty); }
      set { SetValue(ValueThreeVisibleProperty, value); }
    }

    public double MaxValue
    {
      get { return (double)GetValue(MaxValueProperty); }
      set { SetValue(MaxValueProperty, value); }
    }

    public double LenghtInMinutes
    {
      get { return (double)GetValue(LenghtInMinutesProperty); }
      set { SetValue(LenghtInMinutesProperty, value); }
    }

    public double GraphValueReference
    {
      get { return (double)GetValue(GraphValueReferenceProperty); }
      set { SetValue(GraphValueReferenceProperty, value); }
    }

    public double ScaleX
    {
      get { return (double)GetValue(ScaleXProperty); }
      set
      {
        SetValue(ScaleXProperty, value);
        FixScales();
      }
    }

    public double ScaleY
    {
      get { return (double)GetValue(ScaleYProperty); }
      set
      {
        SetValue(ScaleYProperty, value);
        FixScales();
      }
    }

    public double ShiftX
    {
      get { return (double)GetValue(ShiftXProperty); }
      set
      {
        SetValue(ShiftXProperty, value);
        FixShift();
      }
    }

    public double ShiftY
    {
      get { return (double)GetValue(ShiftYProperty); }
      set
      {
        SetValue(ShiftYProperty, value);
        FixShift();
      }
    }

    public int CurrentFrame
    {
      get { return (int)GetValue(CurrentFrameProperty); }
      set { SetValue(CurrentFrameProperty, value); }
    }

    public bool EngineRunning
    {
      get { return (bool)GetValue(ScaleYProperty); }
      set { SetValue(ScaleYProperty, value); }
    }

    public bool DynamicLenght
    {
      get { return (bool)GetValue(DynamicLenghtProperty); }
      set { SetValue(DynamicLenghtProperty, value); }
    }

    public TimelineControl()
    {
      InitializeComponent();
    }

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
      base.OnPropertyChanged(e);
      if (e.Property == TimelineControl.DeltaProperty)
      {
        elapsedTime += Delta;
        if (DynamicLenght && elapsedTime > (LenghtInMinutes * 60))
          LenghtInMinutes += 0.02;
      }
      else if (e.Property == TimelineControl.GraphValueOneProperty)
      {
        InsertNewDataPoint(GraphValueOne, Brushes.Red);
      }
      else if (e.Property == TimelineControl.GraphValueTwoProperty)
      {
        InsertNewDataPoint(GraphValueTwo, Brushes.Blue);
      }
      else if (e.Property == TimelineControl.GraphValueThreeProperty)
      {
        InsertNewDataPoint(GraphValueThree, Brushes.Yellow);
      }
      else if (e.Property == TimelineControl.ValueOneVisibleProperty)
      {
        foreach (UIElement ui in cGraphContent.Children)
        {
          if (ui is Ellipse && ((Ellipse)ui).Fill == Brushes.Red)
            ui.Visibility = ValueOneVisible;
        }
      }
      else if (e.Property == TimelineControl.ValueTwoVisibleProperty)
      {
        foreach (UIElement ui in cGraphContent.Children)
        {
          if (ui is Ellipse && ((Ellipse)ui).Fill == Brushes.Blue)
            ui.Visibility = ValueTwoVisible;
        }
      }
      else if (e.Property == TimelineControl.ValueThreeVisibleProperty)
      {
        foreach (UIElement ui in cGraphContent.Children)
        {
          if (ui is Ellipse && ((Ellipse)ui).Fill == Brushes.Yellow)
            ui.Visibility = ValueThreeVisible;
        }
      }
      else if (e.Property == TimelineControl.MaxValueProperty || e.Property == TimelineControl.ActualHeightProperty)
      {
        double axisLenght = lAxisY.ActualHeight - lAxisX.Margin.Bottom;
        ScaleY = axisLenght / MaxValue;
      }
      else if (e.Property == TimelineControl.LenghtInMinutesProperty || e.Property == TimelineControl.ActualWidthProperty)
      {
        double axisLenght = lAxisX.ActualWidth - lAxisY.Margin.Left;
        ScaleX = axisLenght / (LenghtInMinutes * 60);
      }
      else if (e.Property == TimelineControl.ShiftXProperty)
      {
        ShiftX = (double)e.NewValue;
      }
      else if (e.Property == TimelineControl.ShiftYProperty)
      {
        ShiftY = (double)e.NewValue;
      }
      else if (e.Property == TimelineControl.EngineRunningProperty)
      {
        bool engineRunning = (bool)e.NewValue;
        if (!engineRunning)
          return;

        var ellipses = new List<Ellipse>();
        foreach (UIElement element in cGraphContent.Children)
        {
          if (element is Ellipse)
            ellipses.Add(element as Ellipse);
        }
        foreach (Ellipse ellipse in ellipses)
          cGraphContent.Children.Remove(ellipse);

        elapsedTime = 0;
      }
    }

    private void FixScales()
    {
      lReference.StrokeThickness = 1 / ScaleY;
      lReference.StrokeDashArray = new DoubleCollection();
      lReference.StrokeDashArray.Add(2 / ScaleX);
      lReference.StrokeDashArray.Add(1 / ScaleX);

      foreach (UIElement element in cGraphContent.Children)
      {
        if (element is Ellipse)
        {
          (element as Ellipse).Width = 2 / ScaleX;
          (element as Ellipse).Height = 4 / ScaleY;
        }
      }
    }

    private void FixShift()
    {
      double convertY = lReference.Y1 / MaxValue * (lAxisY.ActualHeight - lAxisX.Margin.Bottom) - 5;
      if (convertY <= ShiftY)
      {
        lReference.Visibility = Visibility.Hidden;
      }
      else
      {
        lReference.Visibility = Visibility.Visible;
      }
      foreach (UIElement element in cGraphContent.Children)
      {
        if (element is Ellipse)
        {
          double convertX;
          convertX = Canvas.GetLeft((element as Ellipse)) / (LenghtInMinutes * 60) * (lAxisX.ActualWidth - lAxisY.Margin.Left) - 2;
          convertY = Canvas.GetTop((element as Ellipse)) / MaxValue * (lAxisY.ActualHeight - lAxisX.Margin.Bottom) - 3;
          if (convertY <= ShiftY || convertX <= ShiftX)
          {
            element.Visibility = Visibility.Hidden;
          }
          else
          {
            element.Visibility = Visibility.Visible;
          }
        }
      }
    }

    private double elapsedTime = 0;
    private void InsertNewDataPoint(double value, Brush colorBrush)
    {
      Ellipse newPoint = new Ellipse();
      newPoint.Width = 2 / ScaleX;
      newPoint.Height = 4 / ScaleY;
      newPoint.Fill = colorBrush;
      Canvas.SetLeft(newPoint, elapsedTime);
      Canvas.SetTop(newPoint, value);
      cGraphContent.Children.Add(newPoint);
      newPoint.ToolTip = new ToolTip() { Content = String.Format("Frame: {0}\n Value: {1}", CurrentFrame, value) };
    }

    private void bVerticalZoonIn_Click(object sender, RoutedEventArgs e)
    {
      MaxValue /= 1.05;
    }

    private void bVerticalZoonOut_Click(object sender, RoutedEventArgs e)
    {
      MaxValue *= 1.05;
    }

    private void bHorizontalZoonIn_Click(object sender, RoutedEventArgs e)
    {
      LenghtInMinutes /= 1.02;
    }

    private void bHorizontalZoonOut_Click(object sender, RoutedEventArgs e)
    {
      LenghtInMinutes *= 1.02;
    }
  }

}
