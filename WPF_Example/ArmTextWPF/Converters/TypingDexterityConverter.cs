using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using ArmText.Typing;

namespace ArmText.Converters
{
  public class TypingDexterityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      TypingDexterity target = (TypingDexterity)value;
      TypingDexterity actual = (TypingDexterity)Enum.Parse(typeof(TypingDexterity), parameter as String);

      if (actual == target)
        return true;
      return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      bool isChecked = (bool)value;
      if (!isChecked)
        return null;

      return (TypingDexterity)Enum.Parse(typeof(TypingDexterity), parameter as String);
    }
  }
}
