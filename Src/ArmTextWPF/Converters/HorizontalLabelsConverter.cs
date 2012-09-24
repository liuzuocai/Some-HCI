using System;
using System.Windows;
using System.Windows.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArmText.Converters
{
  class HorizontalLabelsConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == DependencyProperty.UnsetValue)
        return 0;

      double width = (double)value;

      String[] parameters = parameter.ToString().Split(',');
      int numberOfItems = Int32.Parse(parameters[0]);
      int itemIndex = Int32.Parse(parameters[1]);

      double step = width / numberOfItems;

      return itemIndex * step;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
