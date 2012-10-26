using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Kinect;
using System.Windows.Input;
using System.ComponentModel;
using ArmText.Util;

namespace ArmText.Typing
{

  public abstract class TypingMethod : INotifyPropertyChanged
  {
    //If there is a displacement of at least the value below of in the dimension of the push then the values
    // on the two other dimensions are blocked.
    private const double PUSH_AXIS_DISTANCE = 0.01;
    private const double SWIPE_THRESHOLD = 0.015;
    private const double LONG_ARM_CONSTRAIN = 0.40;
    private const double SHORT_ARM_CONSTRAIN = 0.4;
    private const double CONSTRAIN_RANGE = 0.18;
    private Point3D planeCenter;
    private Point3D lastPos;

    //joints
    public JointType Shoulder { get; set; }
    public JointType Elbow { get; set; }
    public JointType Wrist { get; set; }
    public JointType Hand { get; set; }
    public JointType Hip { get; set; }

    //this method is to be overridden by each particular typing method
    public abstract TypingStatus ProcessNewFrame(Point3D cursor, ICollection<TypingGesture> gestures, Skeleton stableSkeleton, double deltaTimeMilliseconds, TypingDexterity dexterity);

    public TypingMethod()
    {
      Shoulder = JointType.Head;
      Elbow = JointType.Head;
      Wrist = JointType.Head;
      Hand = JointType.Head;
      Hip = JointType.Head;
    }

    internal virtual Point3D FindCursorPosition(Skeleton skeleton, LayoutAnchoring anchoring, Size layoutSize, TypingDexterity dexterity, SelectionMethod selectionM, ArmStretch armStretch)
    {
      SetJointsForDexterity(dexterity);

      Joint shoulder = skeleton.Joints.SingleOrDefault(tmp => tmp.JointType == Shoulder);
      Joint hand = skeleton.Joints.SingleOrDefault(tmp => tmp.JointType == Hand);
      Joint wrist = skeleton.Joints.SingleOrDefault(tmp => tmp.JointType == Wrist);
      Joint hip = skeleton.Joints.SingleOrDefault(tmp => tmp.JointType == Hip);

      double width = layoutSize.Width / 100;
      double height = layoutSize.Height / 100;

      if (anchoring == LayoutAnchoring.VerticalShoulderLevel)
      {
        planeCenter.X = shoulder.Position.X + width / 2;
        planeCenter.Y = shoulder.Position.Y;
      }
      else if (anchoring == LayoutAnchoring.VerticalBodyCenterLevel)
      {
        planeCenter.X = shoulder.Position.X + width / 2;
        planeCenter.Y = (shoulder.Position.Y + hip.Position.Y) / 2;
      }
      else if (anchoring == LayoutAnchoring.HorizontalBottomLevel)
      {
        planeCenter.X = shoulder.Position.X + width / 2 + 0.05;
        //because of the coordinate direction
        planeCenter.Z = shoulder.Position.Z - height / 2 - 0.05;
      }

      double pointerPosX = (hand.Position.X + wrist.Position.X) / 2;
      double distX = pointerPosX - planeCenter.X;
      double pointerPosY = (hand.Position.Y + wrist.Position.Y) / 2;
      double distY = pointerPosY - planeCenter.Y;
      double pointerPosZ = (hand.Position.Z + wrist.Position.Z) / 2;
      double distZ = pointerPosZ - planeCenter.Z;

      Point3D pointer = new Point3D(pointerPosX, pointerPosY, pointerPosZ);
      Point3D origin = new Point3D(shoulder.Position.X, shoulder.Position.Y, shoulder.Position.Z);
      System.Windows.Media.Media3D.Vector3D distanceFromOrigin = ToolBox.CalculateDisplacement(
        (System.Windows.Media.Media3D.Vector3D)pointer,
        (System.Windows.Media.Media3D.Vector3D)origin);

      Point3D cursorP = new Point3D(-1, -1, -1);
      if (armStretch == ArmStretch.Long && distanceFromOrigin.Length < LONG_ARM_CONSTRAIN)
        return lastPos = cursorP;
      if (armStretch == ArmStretch.Short)
      {
        if (anchoring == LayoutAnchoring.VerticalShoulderLevel || anchoring == LayoutAnchoring.VerticalBodyCenterLevel)
        {
          if (Math.Abs(distanceFromOrigin.Z) > SHORT_ARM_CONSTRAIN)
            return lastPos = cursorP;
        }
        else if (anchoring == LayoutAnchoring.HorizontalBottomLevel)
        {
          if (Math.Abs(distanceFromOrigin.Y) > SHORT_ARM_CONSTRAIN)
            return lastPos = cursorP;
        }
      }

      //if the pointer is close enough to plane
      if (anchoring == LayoutAnchoring.VerticalShoulderLevel || anchoring == LayoutAnchoring.VerticalBodyCenterLevel)
      {
        //out of width boundary
        if (Math.Abs(distX) > width / 2)
          return lastPos = cursorP;
        //out of height boundary
        if (Math.Abs(distY) > height / 2)
          return lastPos = cursorP;

        double absX = distX + width / 2;
        double absY = height - ((height / 2) + distY);

        cursorP.X = absX / width;
        cursorP.Y = absY / height;
        cursorP.Z = distZ;
        cursorP = FreezeMovementesForSelectionGesture(cursorP, selectionM);
        lastPos = cursorP;
      }
      else //if (anchoring == LayoutAnchoring.HorizontalBottomLevel)
      {
        //out of column boundary
        if (Math.Abs(distX) > width / 2)
          return lastPos = cursorP;
        //out of row boundary
        if (Math.Abs(distZ) > height / 2)
          return lastPos = cursorP;

        double absX = distX + width / 2;
        double absZ = ((height / 2) + distZ);

        cursorP.X = absX / width;
        cursorP.Y = absZ / height;
        cursorP.Z = distY;
        cursorP = FreezeMovementesForSelectionGesture(cursorP, selectionM);
        lastPos = cursorP;
      }
      //if not close to plane, return an invalid cursorP
      return lastPos = cursorP;
    }

    private Point3D FreezeMovementesForSelectionGesture(Point3D cursorP, SelectionMethod selectionM)
    {
      cursorP.FrozenX = cursorP.X;
      cursorP.FrozenY = cursorP.Y;
      cursorP.FrozenZ = cursorP.Z;

      if (selectionM == SelectionMethod.Push)
      {
        //If the movement looks like a push (movement in Z), then it ignores the movements in the other two dimensions
        if (Math.Abs(lastPos.FrozenZ - cursorP.Z) >= TypingMethod.PUSH_AXIS_DISTANCE)
        {
          cursorP.IsFrozen = true;
          cursorP.X = lastPos.X;
          cursorP.Y = lastPos.Y;
          //cursorP.Z = lastPos.Z;
        }
      }
      else if (selectionM == SelectionMethod.Swipe)
      {
        //If the movement looks like a swipe (movement in X), then it ignores the movements in the other two dimensions
        if (Math.Abs(lastPos.X  - cursorP.X) < TypingMethod.SWIPE_THRESHOLD)
          return cursorP;
        //if (Math.Abs(lastPos.Y - cursorP.Y) >= TypingMethod.SWIPE_THRESHOLD)
        //  return cursorP;

        cursorP.IsFrozen = true;
        cursorP.FrozenX = lastPos.FrozenX;
        cursorP.FrozenY = lastPos.FrozenY;
        cursorP.FrozenZ = lastPos.FrozenZ;
      }
      return cursorP;
    }

    protected void SetJointsForDexterity(TypingDexterity dexterity)
    {
      if (dexterity == TypingDexterity.Right)
      {
        Shoulder = JointType.ShoulderRight;
        Elbow = JointType.ElbowRight;
        Wrist = JointType.WristRight;
        Hand = JointType.HandRight;
        Hip = JointType.HipRight;
      }
      else if (dexterity == TypingDexterity.Left)
      {
        Shoulder = JointType.ShoulderLeft;
        Elbow = JointType.ElbowLeft;
        Wrist = JointType.WristLeft;
        Hand = JointType.HandLeft;
        Hip = JointType.HipLeft;
      }
    }

    public virtual void Reset()
    {
    }


    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(String name)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(name));
    }
  }

}
