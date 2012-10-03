using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;
using ArmText.Typing;


namespace ArmText.Converters
{
  public class CircularLabelAngleConverter : IValueConverter
  {



    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if(value == null)
        throw new NotImplementedException();

      double angleRadian = ArmText.Typing.CircularTypingMethod.CIRCLE_RADIAN / ArmText.Typing.CircularTypingMethod.Layout.Length;
      if(value ==null)
        throw new NotImplementedException();
      Key actualKey = (Key)Enum.Parse(typeof(Key), (String)value);
      int index = 0;
      for (; index < CircularTypingMethod.Layout.Length; index++)
      {
        if (actualKey == CircularTypingMethod.Layout[index])
          break;
      }
      if("RotateLabel".Equals(parameter as String))
        return -index * angleRadian / Math.PI / 2 * 360;
      else
        return index * angleRadian/Math.PI/2*360;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
