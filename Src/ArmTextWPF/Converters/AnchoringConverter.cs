using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using ArmText.Typing;

namespace ArmText.Converters
{
  class AnchoringConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      LayoutAnchoring target = (LayoutAnchoring)value;
      LayoutAnchoring actual = (LayoutAnchoring)Enum.Parse(typeof(LayoutAnchoring), parameter as String);

      if (actual == target)
        return true;
      return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      bool isChecked = (bool)value;
      if (!isChecked)
        return null;

      return (LayoutAnchoring)Enum.Parse(typeof(LayoutAnchoring), parameter as String);
    }
  }
}
