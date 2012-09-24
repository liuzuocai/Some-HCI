using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArmText.Util;

namespace ArmText.Typing
{
  public enum GestureType { Pressing, Releasing, Tap };

  public struct TypingGesture 
  {
    public Point3D Position;
    public GestureType Type;
    public DateTime Time;
  }
}
