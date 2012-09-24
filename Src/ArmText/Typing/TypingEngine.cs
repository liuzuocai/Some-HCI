using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.Collections;
using System.Windows;
using System.ComponentModel;
using System.Windows.Input;
using System.Threading;
using ArmText.Util;
using ArmText.Typing;
using ArmText.Pointing;

namespace ArmText.Typing
{

  public delegate void KeySelectedEventHandler(object sender, KeySelectedEventArgs e);

  public class TypingEngine : INotifyPropertyChanged
  {

    private int currentFrame = 1;// totalFrames = 0;
    private Object processingLock = new Object();
    private EventWaitHandle processingWaitHandle = new EventWaitHandle(true, EventResetMode.AutoReset);

    private Point3D cursorLocation;
    private Point3D secCursorLocation;
    private Key highlightedKey;
    private Key selectedKey;
    private SelectionMethod selectionMethod;
    private TypingDexterity dexterity;

    private TypingMethod mainTypingMethod;
    private TypingMethod secTypingMethod;
    private PlaneSize planeSize;
    private LayoutAnchoring anchoring;

    protected double errorRateLanding;
    protected double errorRateSelecting;
    private ArmStretch armStretch;

    public IList<TypingMethod> Methods { get; set; }

    public GestureRecognizer Recognizer { get; set; }

    public Point3D CursorLocation
    {
      get { return cursorLocation; }
      set
      {
        cursorLocation = value;
        OnPropertyChanged("CursorLocation");
      }
    }

    public ArmStretch ArmStretch
    {
      get { return armStretch; }
      set
      {
        armStretch = value;
        OnPropertyChanged("ArmStretch", armStretch);
      }
    }

    public Key HighlightedKey
    {
      get { return highlightedKey; }
      set
      {
        if (highlightedKey == value)
          return;
        highlightedKey = value;
        OnPropertyChanged("HighlightedKey");
      }
    }

    public Key SelectedKey
    {
      get { return selectedKey; }
      set
      {
        if (selectedKey == value)
          return;
        selectedKey = value;
        OnPropertyChanged("SelectedKey");
      }
    }

    public SelectionMethod SelectionMethod
    {
      get { return selectionMethod; }
      set
      {
        selectionMethod = value;
        OnPropertyChanged("SelectionMethod");
      }
    }

    public TypingMethod TypingMethod
    {
      get { return mainTypingMethod; }
      set
      {
        mainTypingMethod = value;
        OnPropertyChanged("TypingMethod");
      }
    }

    public LayoutAnchoring Anchoring
    {
      get { return anchoring; }
      set
      {
        anchoring = value;
        OnPropertyChanged("Anchoring", anchoring);
      }
    }

    public PlaneSize PlaneSize
    {
      get { return planeSize; }
      set
      {
        planeSize = value;
        OnPropertyChanged("PlaneSize", planeSize);
      }
    }

    public TypingDexterity Dexterity
    {
      get { return dexterity; }
      set 
      {
        dexterity = value;
        OnPropertyChanged("Dexterity", dexterity);
      }
    }

    public double ErrorRateLanding
    {
      get { return errorRateLanding; }
      set
      {
        errorRateLanding = value;
        OnPropertyChanged("ErrorRateLanding");
      }
    }

    public double ErrorRateSelecting
    {
      get { return errorRateSelecting; }
      set
      {
        errorRateSelecting = value;
        OnPropertyChanged("ErrorRateSelecting");
      }
    }

    private bool hasUserClicked = false;
    private Object clickLock = new Object();
    public bool HasUserClicked
    {
      get
      {
        lock (clickLock)
        {
          if (hasUserClicked)
          {
            hasUserClicked = false;
            return true;
          }
          return false;
        }
      }
      set
      {
        lock (clickLock)
          hasUserClicked = value;
        OnPropertyChanged("HasUserClicked");
      }
    }

    public TypingEngine()
    {
      Methods = new List<TypingMethod>();
      Methods.Add(new ABCTypingMethod());
      Methods.Add(new QwertyTypingMethod());
      Methods.Add(new SplitQwertyTypingMethod());
      Methods.Add(new FitalyTypingMethod());
      Methods.Add(new CircularTypingMethod());
      Methods.Add(new TahnuTypingMethod());
      Methods.Add(new LabyrinthTypingMethod());
      Methods.Add(new SeatoTypingMethod());
      Methods.Add(new BoxTypingMethod());

      TypingMethod = Methods.Last();
      secTypingMethod = new SecondHandSelectionTypingMethod();

      Recognizer = new GestureRecognizer();
      PropertyChanged += new PropertyChangedEventHandler(Recognizer.TypingEngine_PropertyChanged);

      CursorLocation = new Point3D(0, 0, 0);
      secCursorLocation = new Point3D(0, 0, 0);
      planeSize = PlaneSize.p35x35;
      SelectionMethod = ArmText.Typing.SelectionMethod.Click;
      Dexterity = TypingDexterity.Right;
    }

    public bool ProcessNewSkeletonData(Skeleton skeleton, double deltaMilliseconds)
    {
      if (skeleton == null)
        return false;
      if (TypingMethod == null)
        return false;

      //++totalFrames;
      //SkeletonCapture capture = new SkeletonCapture() { Delay = deltaMilliseconds, Skeleton = skeleton, FrameNro = totalFrames };
      //Thread backgroundThread = new Thread(ProcessTypingData);
      //backgroundThread.Priority = ThreadPriority.AboveNormal;
      //backgroundThread.Start(capture);
      DoWork(skeleton, deltaMilliseconds);

      return true;
    }

    public void ProcessTypingData(object data)
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
          _currentFrame = currentFrame;
        if (_currentFrame != threadFrame)
          processingWaitHandle.Set();
      } while (_currentFrame != threadFrame);

      DoWork(skeleton, deltaTimeMilliseconds);

      lock (processingLock)
        currentFrame++;
      processingWaitHandle.Set();
    }

    private void DoWork(Skeleton skeleton, double deltaMilliseconds)
    {
      Size layoutSize = new Size(35, 35);
      if(planeSize == PlaneSize.p25x25)
        layoutSize = new Size(25, 25);

      //1- find the position of the cursor on the layout plane
      CursorLocation = TypingMethod.FindCursorPosition(skeleton, anchoring, layoutSize, dexterity, selectionMethod, armStretch);
      //2- find the second hand cursor
      secCursorLocation = secTypingMethod.FindCursorPosition(skeleton, anchoring, layoutSize, dexterity, selectionMethod, armStretch);
      //3- Looks for gestures [pressed, released]
      ICollection<TypingGesture> gestures = Recognizer.ProcessGestures(skeleton, deltaMilliseconds, cursorLocation, secCursorLocation, selectionMethod, highlightedKey, HasUserClicked);

      //4- Passes it on to the typing method itself
      TypingStatus status = TypingMethod.ProcessNewFrame(CursorLocation, gestures, skeleton, deltaMilliseconds, dexterity);
      HighlightedKey = status.HighlightedKey;
      SelectedKey = status.SelectedKey;
      
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(String name, object value = null)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new ExtendedPropertyChangedEventArgs(name, value));
    }
  }

}
