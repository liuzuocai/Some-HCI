using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;

namespace ArmText.Converters
{
  class SequentialHighlightConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (values.Length != 4 ||
        values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue ||
        values[2] == DependencyProperty.UnsetValue)
        return Brushes.Transparent;
      if (values[1] == null || values[2] == null)
        return Brushes.Transparent;

      Key highlightedKey = (Key)values[0];
      SolidColorBrush backgroundBrush = (SolidColorBrush)values[1];
      Key keyIcon = (Key)Enum.Parse(typeof(Key), (String)values[2]);
      bool reset = (bool)values[3];

      if (reset)
        return Brushes.Transparent;

      if (highlightedKey == keyIcon)
        return Brushes.Pink;

      //any grid with background color pink, keep it pink
      if (backgroundBrush.Color == Brushes.Pink.Color)
        return Brushes.Pink;

      return Brushes.Transparent;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
