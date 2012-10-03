using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using ArmText.Util;

namespace ArmText.Typing
{
  public class FitalyTypingMethod : TypingMethod
  {

    private int layoutRows = 6;
    private int layoutCols = 6;
    private Key[] layout = { 
                              Key.Z, Key.V, Key.C, Key.H, Key.W, Key.K,
                              Key.F, Key.I, Key.T, Key.A, Key.L, Key.Y,
                              Key.Space, Key.Space, Key.N, Key.E, Key.Space, Key.Space,
                              Key.G, Key.D, Key.O, Key.R, Key.S, Key.B,
                              Key.Q, Key.J, Key.U, Key.M, Key.P, Key.X,
                              Key.OemComma, Key.OemPeriod, Key.OemMinus, Key.Separator, Key.OemPlus, Key.OemQuestion
                            };

    private Random randon = new Random((int)DateTime.Now.Ticks);
    private Array allKeys = null;

    public FitalyTypingMethod()
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
      return "Fitaly";
    }

  }
}
