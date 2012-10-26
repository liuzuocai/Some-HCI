using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace ArmText.Converters
{
  class WidthCenterPointConverter : IValueConverter
  {

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (!(value is double))
        throw new NotImplementedException();
      double width=(double)value;
      if ("MiddleCenter".Equals(parameter as String))
        return new System.Windows.Point(width / 2, width / 2);
      if ("TopCenter".Equals(parameter as String))
      {
        double height = 0;
        return new System.Windows.Point(width / 2, height);
      }
      if ("CenterValue".Equals(parameter as String))
      {
        return width / 2;
      }
      if ("LabelCenterX".Equals(parameter as String))
      {
        return width / 2 + 10;
      }
      if("LabelCenterY".Equals(parameter as String))
      {
        return 0;
      }
      throw new NotImplementedException();

    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
