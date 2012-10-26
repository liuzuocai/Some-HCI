using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using ArmText.Typing;
using ArmText.Pointing;

namespace ArmText.Converters
{
  class MethodVisibilityConverter: IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if ("Timer".Equals(parameter as String) && (bool)value)
        return Visibility.Visible;
      if ("ABC".Equals(parameter as String) && (value is ABCTypingMethod))
        return  Visibility.Visible;
      if ("Qwerty".Equals(parameter as String) && (value is QwertyTypingMethod))
        return  Visibility.Visible;
      if ("Labyrinth".Equals(parameter as String) && (value is LabyrinthTypingMethod))
        return  Visibility.Visible;
      if ("RightArm".Equals(parameter as String) && (bool)value)
        return Visibility.Visible;
      if ("Box".Equals(parameter as String) && (value is BoxTypingMethod))
        return Visibility.Visible;
      if ("Circular".Equals(parameter as String) && (value is CircularTypingMethod))
        return Visibility.Visible;
      if ("Fitaly".Equals(parameter as String) && (value is FitalyTypingMethod))
        return Visibility.Visible;
      if ("SplitQwerty".Equals(parameter as String) && (value is SplitQwertyTypingMethod))
        return Visibility.Visible;
      if ("Tahnu".Equals(parameter as String) && (value is TahnuTypingMethod))
        return Visibility.Visible;
      if ("Seato".Equals(parameter as String) && (value is SeatoTypingMethod))
        return Visibility.Visible;
      return Visibility.Hidden;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }

  }
}
