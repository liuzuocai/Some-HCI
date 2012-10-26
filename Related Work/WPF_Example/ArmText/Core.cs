﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Input;
using ArmText.Util;
using ArmText.Typing;
using ArmText.Effort;
using ArmText.Playback;

namespace ArmText
{

  public class Core
  {

    private static KinectSensor kinectSensor;

    public event EventHandler<ColorImageReadyArgs> ColorImageReady;

    public SkeletonFilter SkeletonF { get; set; }
    public EffortEngine EffortE { get; set; }
    public TypingEngine TypingE { get; set; }
    public SkeletonRecorder Recorder { get; set; }
    public SkeletonPlayer Player { get; set; }

    private SkeletonDrawer skeletonDrawer;
    private Point3D hand;
    private Point3D shoulder;
    private Point3D elbow;
    public bool PlayBackFromFile { get; set; }
    private long lastUpdate = -1;

    private const float RenderWidth = 640.0f;
    private const float RenderHeight = 480.0f;

    private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));
    private readonly Brush inferredJointBrush = Brushes.Yellow;
    private const double JointThickness = 3;

    private static Core instance = null;
    public static Core Instance
    {
      get
      {
        if (instance == null)
          instance = new Core();
        return instance;
      }
    }

    public bool IsKinectConnected { get; set; }

    private Core()
    {
    }

    public void Initialize(Dispatcher uiDispatcher, int skeletonBufferSize, int avgHeight, String destFolder,
      int playerBufferSize)
    {
      IsKinectConnected = false;
      PlayBackFromFile = false;
      SkeletonF = new SkeletonFilter(skeletonBufferSize);
      Recorder = new SkeletonRecorder(destFolder);
      TypingE = new TypingEngine();
      EffortE = new EffortEngine(avgHeight);

      Player = new SkeletonPlayer(playerBufferSize, uiDispatcher);
      Player.SkeletonFrameReady += new EventHandler<PlayerSkeletonFrameReadyEventArgs>(Player_SkeletonFrameReady);

      if (KinectSensor.KinectSensors.Count == 0)
      {
        IsKinectConnected = false;
        Console.WriteLine("No Kinect found");
      }
      else
      {
        IsKinectConnected = true;
        kinectSensor = KinectSensor.KinectSensors[0];
        if (kinectSensor == null)
          IsKinectConnected = false;
        else
        {
          kinectSensor.ColorStream.Enable();
          kinectSensor.SkeletonStream.Enable();
          kinectSensor.DepthStream.Enable();
          kinectSensor.Start();
          kinectSensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(kinectSensor_AllFramesReady);
        }
      }
    }

    public void Shutdown()
    {
      if (kinectSensor != null)
      {
        kinectSensor.Stop();
        kinectSensor.Dispose();
      }
      Recorder.Stop(false, true);
    }

    void kinectSensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
    {
      if (PlayBackFromFile)
        return;

      long currentTimeMilliseconds = -1;
      Skeleton[] skeletons = null;
      Skeleton rawSkeleton = null;
      Skeleton stableSkeleton = null;
      using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
      {
        if (skeletonFrame != null)
        {
          skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
          skeletonFrame.CopySkeletonDataTo(skeletons);
          currentTimeMilliseconds = skeletonFrame.Timestamp;

          foreach (Skeleton skeleton in skeletons)
          {
            if (skeleton.TrackingState != SkeletonTrackingState.Tracked)
              //skip the rest below continue
              continue;
            rawSkeleton = skeleton;
            stableSkeleton = SkeletonF.ProcessNewSkeletonData(rawSkeleton);
            break;
          }
        }
      }

      if (stableSkeleton != null)
      {
        //Calculates the delta in time
        double deltaTime = (currentTimeMilliseconds - lastUpdate);
        if (lastUpdate == -1)
          deltaTime = 0;
        lastUpdate = currentTimeMilliseconds;

        //Process the new skeleton in the typing engine
        TypingE.ProcessNewSkeletonData(stableSkeleton, deltaTime);
        //Process the new skeleton in the effort engine
        EffortE.ProcessNewSkeletonData(stableSkeleton, deltaTime);
        //Sends the new skeleton into the recorder
        Recorder.ProcessNewSkeletonData(rawSkeleton, deltaTime);
      }

      using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
      {
        if (ColorImageReady != null)
        {
          DrawingImage imageCanvas = DrawImage(colorFrame, stableSkeleton,
            EffortE.RightArmE.CenterOfMass, EffortE.RightArmE.ForearmCoM, EffortE.RightArmE.UpperArmCoM);
          ColorImageReady(this, new ColorImageReadyArgs() { Frame = imageCanvas });
        }
      }
    }

    void Player_SkeletonFrameReady(object sender, PlayerSkeletonFrameReadyEventArgs e)
    {
      //We first stabilize it
      Skeleton stableSkeleton = SkeletonF.ProcessNewSkeletonData(e.FrameSkeleton);

      //Process the new skeleton in the typing engine
      TypingE.ProcessNewSkeletonData(stableSkeleton, e.Delay);
      //Process the new skeleton in the effort engine
      EffortE.ProcessNewSkeletonData(stableSkeleton, e.Delay);

      //We paint the skeleton and send the image over to the UI
      if (ColorImageReady != null)
      {
        DrawingImage imageCanvas = DrawImage(null, stableSkeleton,
            EffortE.RightArmE.CenterOfMass, EffortE.RightArmE.ForearmCoM, EffortE.RightArmE.UpperArmCoM);
        ColorImageReady(this, new ColorImageReadyArgs() { Frame = imageCanvas });
      }
    }

    //drawing color image and skeleton
    private DrawingImage DrawImage(ColorImageFrame colorFrame, Skeleton skeleton, 
      System.Windows.Media.Media3D.Vector3D? armCoM = null, 
      System.Windows.Media.Media3D.Vector3D? forearmCoM = null, 
      System.Windows.Media.Media3D.Vector3D? upperArmCoM = null)
    {
      DrawingGroup dgColorImageAndSkeleton = new DrawingGroup();
      DrawingImage drawingImage = new DrawingImage(dgColorImageAndSkeleton);
      skeletonDrawer = new SkeletonDrawer(kinectSensor);
      using (DrawingContext drawingContext = dgColorImageAndSkeleton.Open())
      {
        InitializeDrawingImage(colorFrame, drawingContext);
        skeletonDrawer.DrawSkeleton(skeleton, drawingContext);
        skeletonDrawer.DrawPoint(skeleton, armCoM, drawingContext);
        skeletonDrawer.DrawPoint(skeleton, forearmCoM, drawingContext);
        skeletonDrawer.DrawPoint(skeleton, upperArmCoM, drawingContext);
      }

      //Make sure the image remains within the defined width and height
      dgColorImageAndSkeleton.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));

      return drawingImage;
    }

    private static void InitializeDrawingImage(ColorImageFrame colorFrame, DrawingContext drawingContext)
    {
      if (colorFrame != null)
      {
        byte[] cbyte = new byte[colorFrame.PixelDataLength];
        colorFrame.CopyPixelDataTo(cbyte);
        int stride = colorFrame.Width * 4;

        ImageSource imageBackground = BitmapSource.Create(640, 480, 96, 96, PixelFormats.Bgr32, null, cbyte, stride);
        drawingContext.DrawImage(imageBackground, new Rect(0.0, 0.0, RenderWidth, RenderHeight));
      }
      else
      {
        // Draw a transparent background to set the render size
        drawingContext.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));
      }
    }

    private EventHandler playbackHandler;

    public void PlayBack(string fileName, EventHandler pbFinished, bool useDelay)
    {
      if (playbackHandler != null)
        Player.PlaybackFinished -= playbackHandler;
      playbackHandler = pbFinished;
      Player.UseDelay = useDelay;
      Player.PlaybackFinished += playbackHandler;
      Player.Load(fileName);
      Player.Start();

      PlayBackFromFile = true;
    }

  }

}