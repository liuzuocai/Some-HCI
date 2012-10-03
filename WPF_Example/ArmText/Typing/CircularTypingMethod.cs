using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using ArmText.Typing;
using ArmText.Util;
using Microsoft.Kinect;
using System.Windows;

namespace ArmText.Typing
{
  public class CircularTypingMethod : TypingMethod
  {
    public const double CIRCLE_RADIAN = Math.PI * 2;
    public const double CIRCLE_ANGLE = 360.00;
    public static Key[] Layout = { 
                              Key.A, Key.B, Key.C, Key.D, Key.E, Key.F,
                              Key.G, Key.H, Key.I, Key.J, Key.K, Key.L,
                              Key.M, Key.N, Key.O, Key.P, Key.Q, Key.R,
                              Key.S, Key.T, Key.U, Key.V, Key.W, Key.X,
                              Key.Y, Key.Z, Key.OemComma, Key.OemPeriod, Key.Separator, Key.OemMinus,
                              Key.Space, Key.OemPlus, Key.OemQuestion
                            };

    private Random randon = new Random((int)DateTime.Now.Ticks);
    private double unitAngle = CIRCLE_RADIAN / Layout.Length;

    public CircularTypingMethod()
    {
    }

    public override TypingStatus ProcessNewFrame(Point3D cursor, ICollection<TypingGesture> gestures, Microsoft.Kinect.Skeleton stableSkeleton, double deltaTimeMilliseconds, TypingDexterity dexterity)
    {
      if (cursor.X < 0 || cursor.Y < 0)
        return new TypingStatus();

      double anglet = Vector.AngleBetween(new Vector(0, 10), new Vector(-10, -10));
      double finalAngle;
      double constraint = cursor.Z;
      Vector pointVector = new Vector(cursor.X - 0.5, -(cursor.Y - 0.5));
      Vector standard = new Vector(0, 0.5);
      double angle = Vector.AngleBetween(standard, pointVector);

      if (angle <= 0)
        finalAngle = -angle;
      else
        finalAngle = 360 - angle;

      int index = (int)Math.Round((Layout.Length-1) * finalAngle / CIRCLE_ANGLE);
      //index = 1;
      TypingStatus status = new TypingStatus();
      if (index < 0 || index > 32)
        index = -1;

      status.HighlightedKey = Layout[index];
      if (gestures != null && gestures.Count > 0 && gestures.ElementAt(0).Type == GestureType.Tap)
      {
        TypingGesture gesture = gestures.ElementAt(0);
        pointVector.X = gesture.Position.X - 0.5;
        pointVector.Y = -(gesture.Position.Y - 0.5);
        angle = Vector.AngleBetween(standard, pointVector);
        if (angle <= 0)
          finalAngle = -angle;
        else
          finalAngle = 360 - angle;

        index = (int)Math.Round((Layout.Length-1) * finalAngle / CIRCLE_ANGLE);
        status.SelectedKey = Layout[index];
      }

      return status;
    }

  }
}
