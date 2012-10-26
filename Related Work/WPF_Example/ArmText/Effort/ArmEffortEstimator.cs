using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.ComponentModel;
using ArmText.Util;

namespace ArmText.Effort
{

  public class ArmEffortEstimator : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    #region CONST
    private const double HAND_MASS = 0.4;

    private const double MALE_FOREARM_MASS = 1.2;
    private const double FEMALE_FOREARM_MASS = 1.0;

    private const double MALE_UPPERARM_MASS = 2.1;
    private const double FEMALE_UPPERARM_MASS = 1.7;

    private const double MALE_UPPERARM_LENGHT = 33;
    private const double MALE_FOREARM_LENGHT = 26.9;
    private const double MALE_HAND_LENGHT = 19.1;

    private const double FEMALE_UPPERARM_LENGHT = 31;
    private const double FEMALE_FOREARM_LENGHT = 23.4;
    private const double FEMALE_HAND_LENGHT = 18.3;

    private const double UPPER_ARM_CENTER_GRAVITY_RATIO = 0.452;
    private const double MALE_UPPER_ARM_CENTER_OF_GRAVITY = MALE_UPPERARM_LENGHT * UPPER_ARM_CENTER_GRAVITY_RATIO;
    private const double FEMALE_UPPER_ARM_CENTER_OF_GRAVITY = FEMALE_UPPERARM_LENGHT * UPPER_ARM_CENTER_GRAVITY_RATIO;

    private const double FOREARM_CENTER_GRAVITY_RATIO = 0.424;
    private const double MALE_FOREARM_CENTER_OF_GRAVITY = MALE_FOREARM_LENGHT * FOREARM_CENTER_GRAVITY_RATIO;
    private const double FEMALE_FOREARM_CENTER_OF_GRAVITY = FEMALE_FOREARM_LENGHT * FOREARM_CENTER_GRAVITY_RATIO;

    private const double HAND_CENTER_GRAVITY_RATIO = 0.397;
    private const double MALE_HAND_CENTER_OF_GRAVITY = MALE_HAND_LENGHT * HAND_CENTER_GRAVITY_RATIO;
    private const double FEMALE_HAND_CENTER_OF_GRAVITY = FEMALE_HAND_LENGHT * HAND_CENTER_GRAVITY_RATIO;

    private const double UPPER_ARM_INERTIA_RATE = 0.0141;     //141*10^(-4)kg
    private const double FORE_ARM_INERTIA_RATE = 0.0055;    //55*10^(-4)kg

    private const double GRAVITY_ACCELERATION = -9.8; // m/s

    private const double MALE_MAX_FORCE = 101.6;
    private const double FEMALE_MAX_FORCE = 87.2;
    private const double MAX_ENDURANCE = Double.MaxValue;
    //private const double MAX_TORQUE = MAX_FORCE * UPPERARM_LENGHT / 100;
    #endregion

    #region Private Value
    private static readonly System.Windows.Media.Media3D.Vector3D GRAVITY_VECTOR = new System.Windows.Media.Media3D.Vector3D(0, GRAVITY_ACCELERATION, 0);

    public JointType Shoulder { get; set; }
    public JointType Elbow { get; set; }
    public JointType Wrist { get; set; }
    public JointType Hand { get; set; }

    private Dictionary<JointType, Point3D> armJointPoints;
    
    private double armMass;
    private double maxTorque;
    private double upperArmWeightProportion;//Segment weight at the 50th percentile, male
    private double forearmAndHandCenterOfGravity;
    private double foreArmAndHandCenterOfGravityRatio;
    private double maxForce;

    private System.Windows.Media.Media3D.Vector3D armCM;
    private System.Windows.Media.Media3D.Vector3D armLastCM;
    private System.Windows.Media.Media3D.Vector3D upperCM;
    private System.Windows.Media.Media3D.Vector3D upperLastCM;
    private System.Windows.Media.Media3D.Vector3D foreCM;
    private System.Windows.Media.Media3D.Vector3D foreLastCM;

    private double theta = 0;
    private double lastTheta = 0;

    private double elbowAngleSum = 0;
    private double wristAngleSum = 0;

    private System.Windows.Media.Media3D.Vector3D displacement;
    private System.Windows.Media.Media3D.Vector3D currentVelocity;
    private System.Windows.Media.Media3D.Vector3D lastVelocity;
    private System.Windows.Media.Media3D.Vector3D measuredAcceleration;

    private double angularAcc;
    private System.Windows.Media.Media3D.Vector3D inertialTorque;
    private System.Windows.Media.Media3D.Vector3D inertialTorqueWithoutMass;

    //Force at the CoM related variables
    private double comMeasuredAcc = 0;
    private double comHumanAcc = 0;
    private double comHumanAccSum = 0;

    private double comHumanForce = 0;
    private double maxComHumanForcePercent = 0;
    private double forceBasedEndurance = 0;

    private double comHumanForceSum = 0;
    private double avgComHumanForce = 0;
    private double maxAvgComHumanForcePercent = 0;
    private double avgForceBasedEndurance = 0;

    private double forceBasedEnduranceLostPercent = 0; //these two are only calculated for the avg force
    private double forceBasedEnduranceLostRate = 0;

    //Torque at the shoulder related variables
    private double measuredTorque = 0;
    private double measuredTorqueSum = 0;
    private double avgMeasuredTorque = 0;
    private double gravityTorque = 0;
    private double gravityTorqueSum = 0;
    private double avgGravityTorque = 0;
    private double inertialTorqueL = 0;
    private double inertialTorqueSum = 0;
    private double avgInertialTorque = 0;

    private double humanTorque = 0;
    private double maxHumanTorquePercent = 0;
    private double torqueBasedEndurance = 0;

    private double humanTorqueSum = 0;
    private double avgHumanTorque = 0;
    private double maxAvgHumanTorquePercent = 0;
    private double avgTorqueBasedEndurance = 0;

    private double torqueBasedEnduranceLostPercent = 0; //these two are only calculated for the avg torque
    private double torqueBasedEnduranceLostRate = 0;

    //These two variables are used to calculate the avg theta and center of mass for the overall measurement
    private double thetaAngleSum = 0;
    private double armCMLenght;
    #endregion

    #region Property
    public System.Windows.Media.Media3D.Vector3D CenterOfMass
    {
      get { return armCM; }
      set
      {
        armCM = value;
        armCMLenght += value.Length;
        OnPropertyChanged("CenterOfMass");
      }
    }

    public System.Windows.Media.Media3D.Vector3D UpperArmCoM
    {
      get { return upperCM; }
    }

    public System.Windows.Media.Media3D.Vector3D ForearmCoM
    {
      get { return foreCM; }
    }

    public double FbEndurance
    {
      get { return forceBasedEndurance; }
      set
      {
        forceBasedEndurance = value;
        OnPropertyChanged("FbEndurance");
      }
    }

    public double AvgCoMHumanForce
    {
      get { return avgComHumanForce; }
      set
      {
        avgComHumanForce = value;
        OnPropertyChanged("AvgCoMHumanForce");
      }
    }

    public double AvgFbEndurance
    {
      get { return avgForceBasedEndurance; }
      set
      {
        avgForceBasedEndurance = value;
        OnPropertyChanged("AvgFbEndurance");
      }
    }

    public double FbEnduranceLostPercent
    {
      get { return forceBasedEnduranceLostPercent; }
      set
      {
        forceBasedEnduranceLostPercent = value;
        OnPropertyChanged("FbEnduranceLostPercent");
      }
    }

    public double FbEnduranceLostRate
    {
      get { return forceBasedEnduranceLostRate; }
      set
      {
        forceBasedEnduranceLostRate = value;
        OnPropertyChanged("FbEnduranceLostRate");
      }
    }

    public double MaxAvgCoMHumanForcePercent
    {
      get { return maxAvgComHumanForcePercent; }
      set
      {
        maxAvgComHumanForcePercent = value;
        OnPropertyChanged("MaxAvgCoMHumanForcePercent");
      }
    }

    public double Theta
    {
      get { return theta; }
      set
      {
        theta = value;
        thetaAngleSum += 180 * theta / Math.PI;
        OnPropertyChanged("Theta");
      }
    }

    public double CoMHumanAcc
    {
      get { return comHumanAcc; }
      set
      {
        comHumanAcc = value;
        OnPropertyChanged("CoMHumanAcc");
      }
    }

    public double HumanTorque
    {
      get { return humanTorque; }
      set
      {
        humanTorque = value;
        OnPropertyChanged("HumanTorque");
      }
    }

    public double MaxHumanTorquePercent
    {
      get { return maxHumanTorquePercent; }
      set
      {
        maxHumanTorquePercent = value;
        OnPropertyChanged("MaxHumanTorquePercent");
      }
    }

    public double AvgHumanTorque
    {
      get { return avgHumanTorque; }
      set
      {
        avgHumanTorque = value;
        OnPropertyChanged("AvgHumanTorque");
      }
    }

    public double MaxAvgHumanTorquePercent
    {
      get { return maxAvgHumanTorquePercent; }
      set
      {
        maxAvgHumanTorquePercent = value;
        OnPropertyChanged("MaxAvgHumanTorquePercent");
      }
    }

    public double TbEndurance
    {
      get { return torqueBasedEndurance; }
      set
      {
        torqueBasedEndurance = value;
        OnPropertyChanged("TbEndurance");
      }
    }

    public double AvgTbEndurance
    {
      get { return avgTorqueBasedEndurance; }
      set
      {
        avgTorqueBasedEndurance = value;
        OnPropertyChanged("AvgTbEndurance");
      }
    }

    public double TbEnduranceLostPercent
    {
      get { return torqueBasedEnduranceLostPercent; }
      set
      {
        torqueBasedEnduranceLostPercent = value;
        OnPropertyChanged("TbEnduranceLostPercent");
      }
    }

    public double TbEnduranceLostRate
    {
      get { return torqueBasedEnduranceLostRate; }
      set
      {
        torqueBasedEnduranceLostRate = value;
        OnPropertyChanged("TbEnduranceLostRate");
      }
    }

    public double HumanTorqueSum
    {
      get { return humanTorqueSum; }
      set
      {
        humanTorqueSum = value;
        OnPropertyChanged("HumanTorqueSum");
      }
    }

    public double MeasuredTorqueSum
    {
      get { return measuredTorqueSum; }
      set
      {
        measuredTorqueSum = value;
        OnPropertyChanged("MeasuredTorqueSum");
      }
    }

    public double GravityTorqueSum
    {
      get { return gravityTorqueSum; }
      set
      {
        gravityTorqueSum = value;
        OnPropertyChanged("GravityTorqueSum");
      }
    }

    public double InertialTorqueSum
    {
      get { return inertialTorqueSum; }
      set
      {
        inertialTorqueSum = value;
        OnPropertyChanged("InertialTorqueSum");
      }
    }

    public double CoMHumanAccSum
    {
      get { return comHumanAccSum; }
      set
      {
        comHumanAccSum = value;
        OnPropertyChanged("CoMHumanAccSum");
      }
    }

    public double CoMHumanForce
    {
      get { return comHumanForce; }
      set
      {
        comHumanForce = value;
        OnPropertyChanged("CoMHumanForce");
      }
    }

    public double MaxCoMHumanForcePercent
    {
      get { return maxComHumanForcePercent; }
      set
      {
        maxComHumanForcePercent = value;
        OnPropertyChanged("MaxCoMHumanForcePercent");
      }
    }

    public double CoMHumanForceSum
    {
      get { return comHumanForceSum; }
      set
      {
        comHumanForceSum = value;
        OnPropertyChanged("CoMHumanForceSum");
      }
    }

    public double CoMMeasuredAcc
    {
      get { return comMeasuredAcc; }
      set
      {
        comMeasuredAcc = value;
        OnPropertyChanged("CoMMeasuredAcc");
      }
    }
    #endregion
    public ArmEffortEstimator()
    {
      displacement = new System.Windows.Media.Media3D.Vector3D();
      measuredAcceleration = new System.Windows.Media.Media3D.Vector3D();
      currentVelocity = new System.Windows.Media.Media3D.Vector3D(0, 0, 0);
    }

    private void SetGenderValue(TypingGender gender)
    {
      if (gender == TypingGender.Male)
      {
        armMass = MALE_UPPERARM_MASS + MALE_FOREARM_MASS + HAND_MASS;
        maxForce = MALE_MAX_FORCE;
        maxTorque = maxForce * MALE_UPPERARM_LENGHT / 100 + 
                    (MALE_UPPERARM_MASS * GRAVITY_ACCELERATION * MALE_UPPER_ARM_CENTER_OF_GRAVITY / 100 +
                    MALE_FOREARM_MASS * GRAVITY_ACCELERATION * (MALE_UPPERARM_LENGHT + MALE_FOREARM_CENTER_OF_GRAVITY) / 100 +
                    HAND_MASS * GRAVITY_ACCELERATION * (MALE_UPPERARM_LENGHT + MALE_FOREARM_LENGHT + MALE_HAND_CENTER_OF_GRAVITY) / 100);

        upperArmWeightProportion = MALE_UPPERARM_MASS / armMass;//Segment weight at the 50th percentile, male
        
        forearmAndHandCenterOfGravity = ((MALE_FOREARM_LENGHT + MALE_HAND_CENTER_OF_GRAVITY) - MALE_FOREARM_CENTER_OF_GRAVITY) * (HAND_MASS / (MALE_FOREARM_MASS + HAND_MASS)) + MALE_FOREARM_CENTER_OF_GRAVITY;
        foreArmAndHandCenterOfGravityRatio = forearmAndHandCenterOfGravity / (MALE_FOREARM_LENGHT + MALE_HAND_LENGHT);
        
      }
      else
      {
        armMass = FEMALE_UPPERARM_MASS + FEMALE_FOREARM_MASS + HAND_MASS;
        maxForce = FEMALE_MAX_FORCE;
        maxTorque = maxForce * FEMALE_UPPERARM_LENGHT / 100 +
                    (FEMALE_UPPERARM_MASS * GRAVITY_ACCELERATION * FEMALE_UPPER_ARM_CENTER_OF_GRAVITY / 100 +
                    FEMALE_FOREARM_MASS * GRAVITY_ACCELERATION * (FEMALE_UPPERARM_LENGHT + FEMALE_FOREARM_CENTER_OF_GRAVITY) / 100 +
                    HAND_MASS * GRAVITY_ACCELERATION * (FEMALE_UPPERARM_LENGHT + FEMALE_FOREARM_LENGHT + FEMALE_HAND_CENTER_OF_GRAVITY) / 100);
        upperArmWeightProportion = FEMALE_UPPERARM_MASS / armMass;//Segment weight at the 50th percentile, male
        forearmAndHandCenterOfGravity = ((FEMALE_FOREARM_LENGHT + FEMALE_HAND_CENTER_OF_GRAVITY) - FEMALE_FOREARM_CENTER_OF_GRAVITY) * (HAND_MASS / (FEMALE_FOREARM_MASS + HAND_MASS)) + FEMALE_FOREARM_CENTER_OF_GRAVITY;
        foreArmAndHandCenterOfGravityRatio = forearmAndHandCenterOfGravity / (FEMALE_FOREARM_LENGHT + FEMALE_HAND_LENGHT);
      }
    }

    public void EstimateEffort(Skeleton skeleton, double delta, double totalTime)
    {

      armJointPoints = GetArmPoints(skeleton);
      elbowAngleSum += ToolBox.CalculateAngle((System.Windows.Media.Media3D.Vector3D)armJointPoints[Elbow], GRAVITY_VECTOR);
      wristAngleSum += ToolBox.CalculateAngle((System.Windows.Media.Media3D.Vector3D)armJointPoints[Hand], GRAVITY_VECTOR);

      //total arm mass center
      CenterOfMass = CalculateCenterMass(armJointPoints[Shoulder], armJointPoints[Elbow], armJointPoints[Hand]);

      //angel of shoulder-centermass vector with gravity acceleration
      Theta = Math.PI * ToolBox.CalculateAngle(armCM, GRAVITY_VECTOR) / 180;
      //4- Calculate movement acceleration
      displacement = ToolBox.CalculateDisplacement(armLastCM, armCM);
      //v
      currentVelocity = CalculateVelocity(displacement, delta);
      //a
      measuredAcceleration = CalculateMovingAcc(currentVelocity, lastVelocity, delta);
      //α
      angularAcc = CalculateAngularAcc(measuredAcceleration, CenterOfMass.Length);
      //Iα/m
      inertialTorqueWithoutMass = CalculateMaxInertialTorqueWithoutMass();

      //AmMxR= GMxR + Iα + FhxR  --- (measured_acceleration times arm_mass) cross radius = (gravity_acceleration times arm_mass) cross radius + arm_inertia times angular_acceleration + human_force cross radius
      //FhxR = AmMxR - (GMxR + Iα)
      System.Windows.Media.Media3D.Vector3D amM = measuredAcceleration * armMass;
      System.Windows.Media.Media3D.Vector3D amMxR = System.Windows.Media.Media3D.Vector3D.CrossProduct(amM, CenterOfMass);
      System.Windows.Media.Media3D.Vector3D gM = GRAVITY_VECTOR * armMass;
      System.Windows.Media.Media3D.Vector3D gMxR = System.Windows.Media.Media3D.Vector3D.CrossProduct(gM, CenterOfMass);
      System.Windows.Media.Media3D.Vector3D fhxR = amMxR - (gMxR + inertialTorque);

      HumanTorque = fhxR.Length;
      MaxHumanTorquePercent = HumanTorque / maxTorque * 100;
      if (MaxHumanTorquePercent - 15 <= 0)
        TbEndurance = MAX_ENDURANCE;
      else
        TbEndurance = 1236.5 / Math.Pow((MaxHumanTorquePercent - 15), 0.618) - 72.5;

      HumanTorqueSum += fhxR.Length * delta;
      MeasuredTorqueSum += amMxR.Length * delta;
      GravityTorqueSum += gMxR.Length * delta;
      InertialTorqueSum += inertialTorque.Length * delta;

      AvgHumanTorque = HumanTorqueSum / totalTime;
      MaxAvgHumanTorquePercent = AvgHumanTorque / maxTorque * 100;
      if (MaxAvgHumanTorquePercent - 15 <= 0)
        AvgTbEndurance = MAX_ENDURANCE;
      else
        AvgTbEndurance = 1236.5 / Math.Pow((MaxAvgHumanTorquePercent - 15), 0.618) - 72.5;

      TbEnduranceLostPercent = 100 * totalTime / AvgTbEndurance;
      TbEnduranceLostRate = TbEnduranceLostPercent / totalTime;

      //AmMxR= GMxR + Iα + FhxR  --- (measured_acceleration times arm_mass) cross radius = (gravity_acceleration times arm_mass) cross radius + arm_inertia times angular_acceleration + human_force cross radius
      //FhxR = AmMxR - (GMxR + Iα)
      //FhxR/M = AmMxR/M - (GMxR/M + Iα/M) --- as Fh = Ah * M
      //AhxR = AmxR - (GxR + Iα/M)
      System.Windows.Media.Media3D.Vector3D measuredAcceletarionCrossRadius = System.Windows.Media.Media3D.Vector3D.CrossProduct(measuredAcceleration, CenterOfMass);
      System.Windows.Media.Media3D.Vector3D gravityCrossRadius = System.Windows.Media.Media3D.Vector3D.CrossProduct(GRAVITY_VECTOR, CenterOfMass);
      System.Windows.Media.Media3D.Vector3D midTerm = ToolBox.VectorAddition(gravityCrossRadius, inertialTorqueWithoutMass);
      System.Windows.Media.Media3D.Vector3D humanAccelerationCrossRadius = ToolBox.VectorAddition(measuredAcceletarionCrossRadius, -midTerm);
      CoMMeasuredAcc = measuredAcceleration.Length;
      CoMHumanAcc = humanAccelerationCrossRadius.Length / CenterOfMass.Length;
      CoMHumanForce = CoMHumanAcc * armMass;
      MaxCoMHumanForcePercent = CoMHumanForce / maxForce * 100;
      if (MaxCoMHumanForcePercent - 15 <= 0)
        FbEndurance = MAX_ENDURANCE;
      else
        FbEndurance = 1236.5 / Math.Pow((MaxCoMHumanForcePercent - 15), 0.618) - 72.5;

      CoMHumanAccSum += comHumanAcc * delta;
      CoMHumanForceSum += comHumanForce * delta;
      AvgCoMHumanForce = CoMHumanForceSum / totalTime;
      MaxAvgCoMHumanForcePercent = AvgCoMHumanForce / maxForce *100;
      if (MaxAvgCoMHumanForcePercent - 15 <= 0)
        AvgFbEndurance = MAX_ENDURANCE;
      else
        AvgFbEndurance = 1236.5 / Math.Pow((MaxAvgCoMHumanForcePercent - 15), 0.618) - 72.5;

      FbEnduranceLostPercent = 100 * totalTime / AvgFbEndurance;
      FbEnduranceLostRate = FbEnduranceLostPercent / totalTime;

      //shift center of mass value
      upperLastCM = upperCM;
      foreLastCM = foreCM;
      armLastCM = armCM;
      lastTheta = theta;
      lastVelocity = currentVelocity;
    }

    private Dictionary<JointType, Point3D> GetArmPoints(Skeleton skeleton)
    {
      Dictionary<JointType, Point3D> tempJointPoints = new Dictionary<JointType, Point3D>();
      //right side joint tracking
      if (skeleton.Joints[Hand].TrackingState == JointTrackingState.Tracked &&
        skeleton.Joints[Shoulder].TrackingState == JointTrackingState.Tracked &&
        skeleton.Joints[Elbow].TrackingState == JointTrackingState.Tracked)
      {
        Joint rightHand = skeleton.Joints[Hand];
        Joint rightWrist = skeleton.Joints[Wrist];
        Point3D hand = new Point3D(
          0.397 * (rightHand.Position.X - rightWrist.Position.X) + rightWrist.Position.X,
          0.397 * (rightHand.Position.Y - rightWrist.Position.Y) + rightWrist.Position.Y,
          0.397 * (rightHand.Position.Z - rightWrist.Position.Z) + rightWrist.Position.Z);

        Point3D elbow = new Point3D(
          skeleton.Joints[Elbow].Position.X,
          skeleton.Joints[Elbow].Position.Y,
          skeleton.Joints[Elbow].Position.Z);
        Point3D shoulder = new Point3D(
          skeleton.Joints[Shoulder].Position.X,
          skeleton.Joints[Shoulder].Position.Y,
          skeleton.Joints[Shoulder].Position.Z);

        tempJointPoints.Add(Hand, new Point3D(hand.X - shoulder.X, hand.Y - shoulder.Y, hand.Z - shoulder.Z));
        tempJointPoints.Add(Elbow, new Point3D(elbow.X - shoulder.X, elbow.Y - shoulder.Y, elbow.Z - shoulder.Z));
        tempJointPoints.Add(Shoulder, new Point3D(0, 0, 0));
      }
      return tempJointPoints;
    }

    private System.Windows.Media.Media3D.Vector3D CalculateCenterMass(Point3D shoulder, Point3D elbow, Point3D hand)
    {
      //upper arm center mass and fore arm center mass
      upperCM.X = (elbow.X - shoulder.X) * UPPER_ARM_CENTER_GRAVITY_RATIO;
      upperCM.Y = (elbow.Y - shoulder.Y) * UPPER_ARM_CENTER_GRAVITY_RATIO;
      upperCM.Z = (elbow.Z - shoulder.Z) * UPPER_ARM_CENTER_GRAVITY_RATIO;

      //lower arm
      foreCM.X = (hand.X - elbow.X) * foreArmAndHandCenterOfGravityRatio + elbow.X;
      foreCM.Y = (hand.Y - elbow.Y) * foreArmAndHandCenterOfGravityRatio + elbow.Y;
      foreCM.Z = (hand.Z - elbow.Z) * foreArmAndHandCenterOfGravityRatio + elbow.Z;

      //base on equation get whole arm center mass
      armCM.X = (foreCM.X - upperCM.X) * (1 - upperArmWeightProportion) + upperCM.X;
      armCM.Y = (foreCM.Y - upperCM.Y) * (1 - upperArmWeightProportion) + upperCM.Y;
      armCM.Z = (foreCM.Z - upperCM.Z) * (1 - upperArmWeightProportion) + upperCM.Z;

      var normalizingFactor = MALE_UPPERARM_LENGHT / ((elbow - shoulder).Length * 100);
      if(maxForce == FEMALE_MAX_FORCE)
        normalizingFactor = FEMALE_UPPERARM_LENGHT / ((elbow - shoulder).Length * 100);
      var normalizedArmCM = armCM * normalizingFactor;

      return normalizedArmCM;
    }

    private void OnPropertyChanged(String name)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(name));
    }

    private System.Windows.Media.Media3D.Vector3D CalculateVelocity(System.Windows.Media.Media3D.Vector3D disp, double delta)
    {
      System.Windows.Media.Media3D.Vector3D velocity = new System.Windows.Media.Media3D.Vector3D();
      //calculate velocity
      //4.1- Calculate new velocity
      if (delta != 0)
      {
        velocity.X = disp.X / delta;
        velocity.Y = disp.Y / delta;
        velocity.Z = disp.Z / delta;
      }
      return velocity;
    }

    private System.Windows.Media.Media3D.Vector3D CalculateMovingAcc(System.Windows.Media.Media3D.Vector3D currentV,
      System.Windows.Media.Media3D.Vector3D lastV, double delta)
    {
      System.Windows.Media.Media3D.Vector3D acceleration = new System.Windows.Media.Media3D.Vector3D();
      //calculate velocity and acceleration vector
      //4.2- Calculate new acceleration
      if (delta != 0)
      {
        acceleration.X = (currentV.X - lastV.X) / delta;
        acceleration.Y = (currentV.Y - lastV.Y) / delta;
        acceleration.Z = (currentV.Z - lastV.Z) / delta;
      }
      return acceleration;
    }

    //angular acceleration is movining acceleration / rotational radius
    private double CalculateAngularAcc(System.Windows.Media.Media3D.Vector3D acceleration, double radious)
    {
      return acceleration.Length / radious;
    }

    public System.Windows.Media.Media3D.Vector3D CalculateMaxInertialTorqueWithoutMass()
    {
      inertialTorque =System.Windows.Media.Media3D.Vector3D.CrossProduct(displacement, armCM);
      if (inertialTorque.Length!=0)
        inertialTorque.Normalize();
      inertialTorque.X *= (UPPER_ARM_INERTIA_RATE+FORE_ARM_INERTIA_RATE)*angularAcc;
      inertialTorque.Y *= (UPPER_ARM_INERTIA_RATE + FORE_ARM_INERTIA_RATE) * angularAcc;
      inertialTorque.Z *= (UPPER_ARM_INERTIA_RATE + FORE_ARM_INERTIA_RATE) * angularAcc;

      inertialTorqueWithoutMass.X = inertialTorque.X / armMass;
      inertialTorqueWithoutMass.Y = inertialTorque.Y / armMass;
      inertialTorqueWithoutMass.Z = inertialTorque.Z / armMass;
      return inertialTorqueWithoutMass;
    }

    private System.Windows.Media.Media3D.Vector3D CalculateSingleInertia(double inertiaRate, System.Windows.Media.Media3D.Vector3D lastCM, System.Windows.Media.Media3D.Vector3D currentCM)
    {
      System.Windows.Media.Media3D.Vector3D displacementCM = ToolBox.CalculateDisplacement(currentCM, lastCM);
      System.Windows.Media.Media3D.Vector3D inertia = System.Windows.Media.Media3D.Vector3D.CrossProduct(displacement, currentCM); //Vector perpendicular to the movement plane
      inertia.Normalize();
      inertia.X *= inertiaRate;
      inertia.Y *= inertiaRate;
      inertia.Z *= inertiaRate;
      if (ToolBox.CalculateAngle(displacementCM, inertia) > 90)
        return -inertia;
      else
        return inertia;
    }

    public void Reset(TypingGender gender)
    {
      ToolBox.VectorReset(ref armCM);
      ToolBox.VectorReset(ref armLastCM);
      ToolBox.VectorReset(ref upperCM);
      ToolBox.VectorReset(ref upperLastCM);
      ToolBox.VectorReset(ref foreCM);
      ToolBox.VectorReset(ref foreLastCM);
      SetGenderValue(gender);
      theta = 0;
      lastTheta = 0;
      elbowAngleSum = 0;
      wristAngleSum = 0;

      ToolBox.VectorReset(ref displacement);
      ToolBox.VectorReset(ref currentVelocity);
      ToolBox.VectorReset(ref lastVelocity);
      ToolBox.VectorReset(ref measuredAcceleration);

      angularAcc = 0;
      ToolBox.VectorReset(ref inertialTorque);
      ToolBox.VectorReset(ref inertialTorqueWithoutMass);

      //Force at the CoM related variables
      comMeasuredAcc = 0;
      comHumanAcc = 0;
      comHumanAccSum = 0;

      comHumanForce = 0;
      maxComHumanForcePercent = 0;
      forceBasedEndurance = 0;

      comHumanForceSum = 0;
      avgComHumanForce = 0;
      maxAvgComHumanForcePercent = 0;
      avgForceBasedEndurance = 0;

      forceBasedEnduranceLostPercent = 0; //these two are only calculated for the avg force
      forceBasedEnduranceLostRate = 0;

      //Torque at the shoulder related variables
      measuredTorque = 0;
      measuredTorqueSum = 0;
      avgMeasuredTorque = 0;
      gravityTorque = 0;
      gravityTorqueSum = 0;
      avgGravityTorque = 0;
      inertialTorqueL = 0;
      inertialTorqueSum = 0;
      avgInertialTorque = 0;

      humanTorque = 0;
      maxHumanTorquePercent = 0;
      torqueBasedEndurance = 0;

      humanTorqueSum = 0;
      avgHumanTorque = 0;
      maxAvgHumanTorquePercent = 0;
      avgTorqueBasedEndurance = 0;

      torqueBasedEnduranceLostPercent = 0; //these two are only calculated for the avg torque
      torqueBasedEnduranceLostRate = 0;

      //These two variables are used to calculate the avg theta and center of mass for the overall measurement
      thetaAngleSum = 0;
      armCMLenght = 0;
    }

    public double GetAvgCoMTheta(int nroFrames)
    {
      if (nroFrames == 0)
        return 0;
      return thetaAngleSum / nroFrames;
    }

    public double GetAvgCenterOfMassLenght(int nroFrames)
    {
      if (nroFrames == 0)
        return 0;
      return armCMLenght / nroFrames;
    }

    public double GetAvgElbowTheta(int nroFrames)
    {
      if (nroFrames == 0)
        return 0;
      return elbowAngleSum / nroFrames;
    }

    public double GetAvgWristTheta(int nroFrames)
    {
      if (nroFrames == 0)
        return 0;
      return wristAngleSum / nroFrames;
    }

    public double GetAvgMeasuredTorque(double totalTime)
    {
      if (totalTime == 0)
        return 0;
      return measuredTorqueSum / totalTime;
    }

    public double GetAvgHumanTorque(double totalTime)
    {
      if (totalTime == 0)
        return 0;
      return humanTorqueSum / totalTime;
    }

    public double GetAvgGravityTorque(double totalTime)
    {
      if (totalTime == 0)
        return 0;
      return gravityTorqueSum / totalTime;
    }

    public double GetAvgInertialTorque(double totalTime)
    {
      if (totalTime == 0)
        return 0;
      return inertialTorqueSum / totalTime;
    }
  }

}
