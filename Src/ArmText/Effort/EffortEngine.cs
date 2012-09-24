using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Kinect;
using System.ComponentModel;
using System.Data;
using System.Threading;
using ArmText.Util;

namespace ArmText.Effort
{
  public class EffortEngine : INotifyPropertyChanged
  {

    public event PropertyChangedEventHandler PropertyChanged;

    private int currentFrame = 1, totalFrames = 0;
    private Object processingLock = new Object();
    private EventWaitHandle processingWaitHandle = new EventWaitHandle(true, EventResetMode.AutoReset);

    private double delta = 0;
    private double totalTime = 0;
    private bool isStarted = false;
    private TypingGender gender;

    public ArmEffortEstimator RightArmE { get; set; }
    public ArmEffortEstimator LeftArmE { get; set; }

    public TypingGender Gender
    {
      get { return gender; }
      set
      {
        gender = value;
        OnPropertyChanged("Gender");
      }
    }

    public int CurrentFrame
    {
      get { return currentFrame; }
      set
      {
        currentFrame = value;
        OnPropertyChanged("CurrentFrame");
      }
    }

    public double TotalTime
    {
      get { return totalTime; }
      set
      {
        totalTime = value;
        OnPropertyChanged("TotalTime");
      }
    }

    public double Delta
    {
      get { return delta; }
      set
      {
        delta = value;
        OnPropertyChanged("Delta");
      }
    }

    public bool IsStarted
    {
      get { return isStarted; }
      private set
      {
        isStarted = value;
        OnPropertyChanged("IsStarted");
        OnPropertyChanged("IsNotStarted");
      }
    }

    public bool IsNotStarted
    {
      get { return !isStarted; }
    }

    public EffortEngine(int avgHeight)
    {
      RightArmE = new ArmEffortEstimator()
      {
        Shoulder = JointType.ShoulderRight,
        Elbow = JointType.ElbowRight,
        Wrist = JointType.WristRight,
        Hand = JointType.HandRight
      };
      LeftArmE = new ArmEffortEstimator()
      {
        Shoulder = JointType.ShoulderLeft,
        Elbow = JointType.ElbowLeft,
        Wrist = JointType.WristLeft,
        Hand = JointType.HandLeft
      };
      Gender = TypingGender.Male;
    }

    public bool ProcessNewSkeletonData(Skeleton skeleton, double deltaTimeMilliseconds)
    {
      if (!IsStarted)
        return false;
      if (skeleton == null)
        return false;

      //++totalFrames;
      //SkeletonCapture capture = new SkeletonCapture() { Delay = deltaTimeMilliseconds, Skeleton = skeleton, FrameNro = totalFrames };
      //Thread backgroundThread = new Thread(AnalyzeEffort);
      //backgroundThread.Priority = ThreadPriority.Lowest;
      //backgroundThread.Start(capture);

      //-------------
      DoWork(skeleton, deltaTimeMilliseconds);
      CurrentFrame++;
      //-------------

      return true;
    }

    /// <summary>
    /// See details here: https://github.com/hcilab-um/ArmText/wiki/calculate-arm-tangential-force-from-sum-of-forces
    /// </summary>
    /// <param name="data"></param>
    public void AnalyzeEffort(object data)
    {
      SkeletonCapture capture = (SkeletonCapture)data;
      double deltaTimeMilliseconds = capture.Delay;
      Skeleton skeleton = capture.Skeleton;
      int threadFrame = capture.FrameNro;
      int _currentFrame = 0;

      do
      {
        processingWaitHandle.WaitOne();
        lock (processingLock)
          _currentFrame = CurrentFrame;
        if (_currentFrame != threadFrame)
          processingWaitHandle.Set();
      } while (_currentFrame != threadFrame);

      DoWork(skeleton, deltaTimeMilliseconds);

      lock (processingLock)
        CurrentFrame++;
      processingWaitHandle.Set();
    }

    private void DoWork(Skeleton skeleton, double deltaTimeMilliseconds)
    {
      //millisecond to second convert
      Delta = deltaTimeMilliseconds / 1000.000;
      TotalTime += Delta;
      RightArmE.EstimateEffort(skeleton, Delta, TotalTime);
      LeftArmE.EstimateEffort(skeleton, Delta, TotalTime);
    }

    public void Start()
    {
      RightArmE.Reset(gender);
      LeftArmE.Reset(gender);
      IsStarted = true;
      TotalTime = 0;
      Delta = 0;
      TotalTime = 0;
    }

    public void Stop()
    {
      IsStarted = false;
      currentFrame = 1;
      totalFrames = 0;
    }

    public Object StopParameter { get; set; }
    public void Stop(Object parameter)
    {
      StopParameter = parameter;
      Stop();
    }

    private void OnPropertyChanged(String name)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(name));
    }

  }

}
