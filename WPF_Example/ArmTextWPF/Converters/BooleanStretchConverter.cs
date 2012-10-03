using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using ArmText.Typing;
using System.Windows;

namespace ArmText.Converters
{
  class BooleanStretchConverter: IValueConverter
  {

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == DependencyProperty.UnsetValue)
        return false;

      ArmStretch target = (ArmStretch)value;
      ArmStretch actual = (ArmStretch)Enum.Parse(typeof(ArmStretch), parameter as String);

      if (actual == target)
        return true;
      return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      bool isChecked = (bool)value;
      if (!isChecked)
        return null;

      return (ArmStretch)Enum.Parse(typeof(ArmStretch), parameter as String);
    }
  }
}
