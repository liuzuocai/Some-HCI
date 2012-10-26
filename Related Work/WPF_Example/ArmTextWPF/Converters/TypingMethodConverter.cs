using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using ArmText.Typing;
using ArmText.Pointing;

namespace ArmText.Converters
{
  public class TypingMethodConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if ("ABC".Equals(parameter as String) && (value is ABCTypingMethod))
        return true;
      if ("Qwerty".Equals(parameter as String) && (value is QwertyTypingMethod))
        return true;
      if ("Labyrinth".Equals(parameter as String) && (value is LabyrinthTypingMethod))
        return true;
      if ("Box".Equals(parameter as String) && (value is BoxTypingMethod))
        return true;
      if ("Fitaly".Equals(parameter as String) && (value is FitalyTypingMethod))
        return true;
      if ("SplitQwerty".Equals(parameter as String) && (value is SplitQwertyTypingMethod))
        return true;
      if ("Circular".Equals(parameter as String) && (value is CircularTypingMethod))
        return true;
      if ("Tahnu".Equals(parameter as String) && (value is TahnuTypingMethod))
        return true;
      return false;

    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      bool isChecked = (bool)value;
      if (!isChecked)
        return null;

      if ("ABC".Equals(parameter as String))
        return Core.Instance.TypingE.Methods.SingleOrDefault(tmp => tmp is ABCTypingMethod);
      if ("Qwerty".Equals(parameter as String))
        return Core.Instance.TypingE.Methods.SingleOrDefault(tmp => tmp is QwertyTypingMethod);
      if ("Labyrinth".Equals(parameter as String))
        return Core.Instance.TypingE.Methods.SingleOrDefault(tmp => tmp is LabyrinthTypingMethod);
      if ("Box".Equals(parameter as String))
        return Core.Instance.TypingE.Methods.SingleOrDefault(tmp => tmp is BoxTypingMethod);
      if ("Fitaly".Equals(parameter as String))
        return Core.Instance.TypingE.Methods.SingleOrDefault(tmp => tmp is FitalyTypingMethod);
      if ("SplitQwerty".Equals(parameter as String))
        return Core.Instance.TypingE.Methods.SingleOrDefault(tmp => tmp is SplitQwertyTypingMethod);
      if ("Circular".Equals(parameter as String))
        return Core.Instance.TypingE.Methods.SingleOrDefault(tmp => tmp is CircularTypingMethod);
      if ("Tahnu".Equals(parameter as String))
        return Core.Instance.TypingE.Methods.SingleOrDefault(tmp => tmp is TahnuTypingMethod);
      if ("Seato".Equals(parameter as String))
        return Core.Instance.TypingE.Methods.SingleOrDefault(tmp => tmp is SeatoTypingMethod);
      return Core.Instance.TypingE.Methods[0];
    }
  }
}
