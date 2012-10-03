using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using ArmText.Typing;

namespace ArmText.Converters
{
  public class PlaneSizeConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      PlaneSize target = (PlaneSize)value;
      PlaneSize actual = (PlaneSize)Enum.Parse(typeof(PlaneSize), parameter as String);

      if (actual == target)
        return true;
      return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      bool isChecked = (bool)value;
      if (!isChecked)
        return null;

      return (PlaneSize)Enum.Parse(typeof(PlaneSize), parameter as String);
    }
  }
}
