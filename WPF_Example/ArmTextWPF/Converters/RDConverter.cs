using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace ArmText.Converters
{
  /// <summary>
  /// Radians to degress converter
  /// </summary>
  class RDConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == DependencyProperty.UnsetValue)
        return "0.00";
      double valueRadians = (double)value;
      double degrees = (valueRadians * 180) / Math.PI;
      int decimalPoints = Int32.Parse((String)parameter);
      String format = "F" + decimalPoints;
      String returnValue = degrees.ToString(format);
      return returnValue;

    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
