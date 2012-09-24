using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using ArmText.Effort;
namespace ArmText.Converters
{
  public class TypingGenderConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      TypingGender target = (TypingGender)value;
      TypingGender actual = (TypingGender)Enum.Parse(typeof(TypingGender), parameter as String);
      if (actual == target)
        return true;
      return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      bool isChecked = (bool)value;
      if (!isChecked)
        return null;

      return (TypingGender)Enum.Parse(typeof(TypingGender), parameter as String);
    }
  }
}
