using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;
using System.ComponentModel;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using Microsoft.Kinect;

namespace ArmText.Playback
{

  public class SkeletonRecorder : INotifyPropertyChanged
  {

    public event PropertyChangedEventHandler PropertyChanged;

    private String folderPath = Environment.CurrentDirectory;
    private String tmpFileName = null;
    private BinaryFormatter formatter = null;
    private FileStream recordFile = null;
    private BinaryWriter writer = null;
    private bool isRecording = false;

    private double delta = 0;
    private double totalTime = 0;

    public bool IsRecording
    {
      get { return isRecording; }
      private set
      {
        isRecording = value;
        OnPropertyChanged("IsRecording");
        OnPropertyChanged("IsNotRecording");
      }
    }

    public bool IsNotRecording
    {
      get { return !isRecording; }
    }

    private int framesRecorded = 0;
    public int FramesRecorded
    {
      get { return framesRecorded; }
      set
      {
        framesRecorded = value;
        OnPropertyChanged("FramesRecorded");
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

    public SkeletonRecorder(String fPath)
    {
      folderPath = fPath;
      formatter = new BinaryFormatter();
    }

    public void ProcessNewSkeletonData(Skeleton stableSkeleton, double deltaTimeMilliseconds)
    {
      if (!isRecording)
        return;

      Delta = deltaTimeMilliseconds / 1000.000;
      TotalTime += Delta;

      SkeletonCapture capture = new SkeletonCapture() { Delay = deltaTimeMilliseconds, Skeleton = stableSkeleton };
      try
      {
        MemoryStream memTmp = new MemoryStream();
        formatter.Serialize(memTmp, capture);
        byte[] buffer = memTmp.GetBuffer();

        writer.Write(buffer.Length);
        writer.Write(buffer, 0, (int)buffer.Length);
        FramesRecorded++;
      }
      catch (SerializationException e)
      {
        Console.WriteLine("Failed to serialize. Reason: " + e.Message);
        throw;
      }
    }

    public void Start()
    {
      if (isRecording)
        throw new Exception("The system is already recording, therefore cannot be started");

      tmpFileName = System.IO.Path.GetTempFileName().Replace(".tmp", ".kr");
      recordFile = File.Open(tmpFileName, FileMode.CreateNew, FileAccess.ReadWrite);
      writer = new BinaryWriter(recordFile);

      IsRecording = true;
      Delta = 0;
      TotalTime = 0;
    }

    public String Stop(bool saveFile, bool shutdown, String sentence = null)
    {
      if (!isRecording)
      {
        if (shutdown)
          return String.Empty;
        throw new Exception("The system is not recording, therefore cannot be stopped");
      }

      IsRecording = false;
      writer.Flush();
      writer.Close();

      if (saveFile)
      {
        String qualifiedName = String.Format("{0}-{1}-{2}-{3}-{4}-{5}",
          DateTime.Now.ToString("MMddyy-HHmmss"), GetTypingMethod(), GetLayoutAnchoring(), GetArmStrech(), GetSelectionMechanism(), GetGender());
        if (sentence != null)
          qualifiedName = String.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}",
            DateTime.Now.ToString("MMddyy-HHmmss"), GetTypingMethod(), GetLayoutAnchoring(), GetArmStrech(), GetSelectionMechanism(), GetGender(), sentence);
        String newFileName = folderPath + @"\" + qualifiedName + ".kr";

        File.Move(tmpFileName, newFileName);
        return newFileName;
      }
      else
      {
        File.Delete(tmpFileName);
        return String.Empty;
      }
    }

    private String GetSelectionMechanism()
    {
      return Core.Instance.TypingE.SelectionMethod.ToString().ToLower();
    }

    private String GetGender()
    {
      return Core.Instance.EffortE.Gender.ToString().ToLower();
    }

    private String GetTypingMethod()
    {
      if (Core.Instance.TypingE.TypingMethod is Pointing.BoxTypingMethod)
        return "box";
      else if (Core.Instance.TypingE.TypingMethod is Pointing.LabyrinthTypingMethod)
        return "laberynth";
      else if (Core.Instance.TypingE.TypingMethod is Typing.ABCTypingMethod)
        return "abc";
      else if (Core.Instance.TypingE.TypingMethod is Typing.CircularTypingMethod)
        return "circular";
      else if (Core.Instance.TypingE.TypingMethod is Typing.FitalyTypingMethod)
        return "fitaly";
      else if (Core.Instance.TypingE.TypingMethod is Typing.QwertyTypingMethod)
        return "querty";
      else if (Core.Instance.TypingE.TypingMethod is Typing.SplitQwertyTypingMethod)
        return "sqwerty";
      return Core.Instance.TypingE.TypingMethod.ToString();
    }

    private object GetLayoutAnchoring()
    {
      switch (Core.Instance.TypingE.Anchoring)
      {
        case Typing.LayoutAnchoring.HorizontalBottomLevel:
          return "hb";
        case Typing.LayoutAnchoring.VerticalShoulderLevel:
          return "vs";
        case Typing.LayoutAnchoring.VerticalBodyCenterLevel:
          return "ve";
      }
      return Core.Instance.TypingE.Anchoring.ToString();
    }

    private object GetArmStrech()
    {
      switch (Core.Instance.TypingE.ArmStretch)
      {
        case Typing.ArmStretch.Short:
          return "s";
        case Typing.ArmStretch.Long:
          return "l";
      }
      return Core.Instance.TypingE.Anchoring;
    }

    private void OnPropertyChanged(String name)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(name));
    }

  }

}
