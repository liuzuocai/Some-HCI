using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace ArmText.Converters
{
  class VisibilityToBooleanConverter : IValueConverter
  {

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == DependencyProperty.UnsetValue)
        return true;
      if (!(value is Visibility))
        throw new NotImplementedException();
      Visibility isVisible = (Visibility)value;
      if (isVisible == Visibility.Visible)
        return true;
      else
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (!(value is bool))
        throw new NotImplementedException();
      bool isVisible = (bool)value;
      if (isVisible)
        return Visibility.Visible;
      else
        return Visibility.Hidden;
    }

  }
}
