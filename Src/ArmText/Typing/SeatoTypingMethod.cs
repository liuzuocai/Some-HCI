using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using ArmText.Util;

namespace ArmText.Typing
{
  public class SeatoTypingMethod : TypingMethod
  {

    private int layoutRows = 6;
    private int layoutCols = 6;
    private Key[] layout = { 
                              Key.Q, Key.K, Key.J, Key.OemComma, Key.OemPeriod, Key.Separator,
                              Key.V, Key.Z, Key.P, Key.B, Key.X, Key.OemMinus,
                              Key.W, Key.U, Key.L, Key.Y, Key.F, Key.OemQuestion,
                              Key.G, Key.I, Key.N, Key.D, Key.C, Key.OemQuestion,
                              Key.S, Key.E, Key.A, Key.T, Key.O, Key.OemPlus,
                              Key.Space, Key.Space, Key.H, Key.R, Key.M, Key.OemPlus
                            };

    private Random randon = new Random((int)DateTime.Now.Ticks);
    private Array allKeys = null;

    public SeatoTypingMethod()
    {
      allKeys = Enum.GetValues(typeof(Key));
    }

    public override TypingStatus ProcessNewFrame(Point3D cursor, ICollection<TypingGesture> gestures, Microsoft.Kinect.Skeleton stableSkeleton, double deltaTimeMilliseconds, TypingDexterity dexterity)
    {
      double columnData = cursor.X;
      double rowData = cursor.Y;
      double constraintData = cursor.Z;

      if (cursor.IsFrozen)
      {
        columnData = cursor.FrozenX;
        rowData = cursor.FrozenY;
        constraintData = cursor.FrozenZ;
      }

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
      return "Seato";
    }

  }
}
