using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using ArmText.Util;
using ArmText.Typing;

namespace ArmText.Pointing
{
  public class BoxTypingMethod : TypingMethod
  {

    private int layoutRows = 6;
    private int layoutCols = 6;
    public static readonly Key[] layout = { 
                              Key.A, Key.B, Key.C, Key.D, Key.E, Key.F,
                              Key.G, Key.H, Key.I, Key.J, Key.K, Key.L,
                              Key.M, Key.N, Key.O, Key.P, Key.Q, Key.R,
                              Key.S, Key.T, Key.U, Key.V, Key.W, Key.X,
                              Key.Y, Key.Z, Key.D0, Key.D1, Key.D2, Key.D3,
                              Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9
                            };

    private Array allKeys = null;

    public BoxTypingMethod()
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
        col = (int)(gesture.Position.FrozenX / stepX);
        row = (int)(gesture.Position.FrozenY / stepY);
        status.SelectedKey = layout[row * layoutCols + col];
      }

      return status;
    }

    public override string ToString()
    {
      return "Box";
    }

  }
}
