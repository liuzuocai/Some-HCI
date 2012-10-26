using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using ArmText.Util;

namespace ArmText.Typing
{
  public class TahnuTypingMethod : TypingMethod
  {

    private int layoutRows = 6;
    private int layoutCols = 6;
    private Key[] layout = { 
                              Key.OemComma, Key.OemComma, Key.OemPeriod, Key.OemPeriod, Key.OemMinus, Key.Separator,
                              Key.P, Key.J, Key.Q, Key.OemPlus, Key.Enter, Key.Enter,
                              Key.M, Key.C, Key.F, Key.V, Key.X, Key.Z,
                              Key.R, Key.I, Key.D, Key.G, Key.B, Key.K,
                              Key.O, Key.E, Key.S, Key.L, Key.W, Key.Y,
                              Key.Space, Key.T, Key.A, Key.H, Key.N, Key.U
                            };

    private Random randon = new Random((int)DateTime.Now.Ticks);
    private Array allKeys = null;

    public TahnuTypingMethod()
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
      return "Tahnu";
    }

  }
}
