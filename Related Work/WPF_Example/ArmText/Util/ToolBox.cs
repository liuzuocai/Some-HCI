using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace ArmText.Util
{
  public class ToolBox
  {

    public static Vector3D CalculateDisplacement(Vector3D lastPos, Vector3D currentPos)
    {
      Vector3D displacement = new Vector3D(0, 0, 0);
      if (lastPos.Length == 0)
        return displacement;

      displacement.X = currentPos.X - lastPos.X;
      displacement.Y = currentPos.Y - lastPos.Y;
      displacement.Z = currentPos.Z - lastPos.Z;
      return displacement;
    }

    public static Vector3D VectorAddition(Vector3D vector1, Vector3D vector2)
    {
      return new Vector3D(vector1.X + vector2.X, vector1.Y + vector2.Y, vector1.Z + vector2.Z);
    }

    public static void VectorReset(ref System.Windows.Media.Media3D.Vector3D vector)
    {
      vector.X = 0;
      vector.Y = 0;
      vector.Z = 0;
    }

    public static double CalculateAngle(Vector3D vector1, Vector3D vector2)
    {
      double angle =Vector3D.AngleBetween(vector1, vector2);
      return angle;
    }
  }
}
