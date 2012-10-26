using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Kinect;
using ArmText.Util;

namespace ArmText.Typing
{
  public class SecondHandSelectionTypingMethod : TypingMethod
  {

    public SecondHandSelectionTypingMethod()
    {
    }

    internal override Point3D FindCursorPosition(Skeleton skeleton, LayoutAnchoring anchoring, System.Windows.Size layoutSize, TypingDexterity dexterity, SelectionMethod selectionM, ArmStretch armStretch)
    {
      TypingDexterity opposite = TypingDexterity.Right;
      if (dexterity == TypingDexterity.Right)
        opposite = TypingDexterity.Left;
      SetJointsForDexterity(opposite);

      Joint shoulder = skeleton.Joints.SingleOrDefault(tmp => tmp.JointType == Shoulder);
      Joint hand = skeleton.Joints.SingleOrDefault(tmp => tmp.JointType == Hand);
      Joint wrist = skeleton.Joints.SingleOrDefault(tmp => tmp.JointType == Wrist);
      Joint hip = skeleton.Joints.SingleOrDefault(tmp => tmp.JointType == Hip);

      double pointerPosX = (hand.Position.X + wrist.Position.X) / 2;
      double pointerPosY = (hand.Position.Y + wrist.Position.Y) / 2;
      double pointerPosZ = (hand.Position.Z + wrist.Position.Z) / 2;

      //this positions are calculated from the hip
      double posX = Math.Abs(pointerPosX - hip.Position.X);
      double posY = Math.Abs(pointerPosY - hip.Position.Y);
      double posZ = Math.Abs(pointerPosZ - hip.Position.Z);

      return new Point3D(posX, posY, posZ);
    }

    public override TypingStatus ProcessNewFrame(Point3D cursor, ICollection<TypingGesture> gestures, Skeleton stableSkeleton, double deltaTimeMilliseconds, TypingDexterity dexterity)
    {
      throw new NotImplementedException();
    }
  }
}
