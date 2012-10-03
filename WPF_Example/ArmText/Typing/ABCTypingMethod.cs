using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using ArmText.Util;

namespace ArmText.Typing
{
  public class ABCTypingMethod : TypingMethod
  {

    private int layoutRows = 6;
    private int layoutCols = 6;
    private Key[] layout = { 
                              Key.A, Key.B, Key.C, Key.D, Key.E, Key.F,
                              Key.G, Key.H, Key.I, Key.J, Key.K, Key.L,
                              Key.M, Key.N, Key.O, Key.P, Key.Q, Key.R,
                              Key.S, Key.T, Key.U, Key.V, Key.W, Key.X,
                              Key.Y, Key.Z, Key.OemComma, Key.OemPeriod, Key.Separator, Key.OemMinus,
                              Key.Space, Key.Space, Key.OemPlus, Key.OemPlus, Key.OemQuestion, Key.OemQuestion
                            };

    private Random randon = new Random((int)DateTime.Now.Ticks);
    private Array allKeys = null;

    public ABCTypingMethod()
    {
      allKeys = Enum.GetValues(typeof(Key));
    }

    public override TypingStatus ProcessNewFrame(Point3D cursor, ICollection<TypingGesture> gestures, Microsoft.Kinect.Skeleton stableSkeleton, double deltaTimeMilliseconds, TypingDexterity dexterity)
    {
      double columnData = cursor.X;
      double rowData = cursor.Y;
      double constraintData = cursor.Z;

      if (columnData < 0 || rowData < 0)
        return new TypingStatus();

      double stepX = 1.0 / layoutCols;
      double stepY = 1.0 / layoutRows;

      int col = (int)(columnData / stepX);
      int row = (int)(rowData / stepY);

      TypingStatus status = new TypingStatus();
      status.HighlightedKey = layout[row * layoutCols + col];
      if (gestures != null && gestures.Count > 0 && gestures.ElementAt(0).Type == GestureType.Tap)
      {
        TypingGesture gesture = gestures.ElementAt(0);
        col = (int)(gesture.Position.X / stepX);
        row = (int)(gesture.Position.Y / stepY);
        status.SelectedKey = layout[row * layoutCols + col];
      }

      return status;
    }

    public override string ToString()
    {
      return "ABC";
    }

  }
}
