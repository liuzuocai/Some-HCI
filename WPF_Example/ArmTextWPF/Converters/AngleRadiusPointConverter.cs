using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace ArmText.Converters
{
  class AngleRadiusPointConverter : IMultiValueConverter
  {


    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (values == DependencyProperty.UnsetValue || values.Length != 2)
        return new Point();
      if (values[0] == DependencyProperty.UnsetValue)
        return new Point();

      double radius = (double)values[0] * 0.5;
      Point startPoint = (Point)values[1];

      double angleRadian = ArmText.Typing.CircularTypingMethod.CIRCLE_RADIAN / ArmText.Typing.CircularTypingMethod.Layout.Length;
      double x = startPoint.X + Math.Sin(angleRadian) * radius;
      double y = startPoint.Y - Math.Cos(angleRadian) * radius;

      return new Point(x, y);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }

  }
}
