using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using Microsoft.Win32;
using ArmText.Properties;
using System.IO;
using Microsoft.Kinect;
using System.Windows.Threading;
using ArmText.Pointing;
using ArmText.Typing;

namespace ArmText
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window, INotifyPropertyChanged
  {

    private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(MainWindow));

    public event PropertyChangedEventHandler PropertyChanged;

    public static readonly DependencyProperty AutoStartEffortEProperty = DependencyProperty.Register("AutoStartEffortE", typeof(bool), typeof(MainWindow));

    private App appInstance = null;
    private Dispatcher uiDispatcher = null;
    private String filePath = String.Empty;
    private String initialPlaybackFolder = Settings.Default.DestFolder;

    public Core CoreInstance
    {
      get { return Core.Instance; }
    }

    public String FilePath
    {
      get { return filePath; }
      set
      {
        filePath = value;
        OnPropertyChanged("FilePath");

      }
    }

    public bool AutoStartEffortE
    {
      get { return (bool)GetValue(AutoStartEffortEProperty); }
      set { SetValue(AutoStartEffortEProperty, value); }
    }

    public int NroTrails { get; set; }
    public String ConditionsSequence { get; set; }
    private int currentConditionToShow = 0;
    public int CurrentConditionToShow
    {
      get { return currentConditionToShow; }
      set
      {
        currentConditionToShow = value;
        OnPropertyChanged("CurrentConditionToShow");
      }
    }

    public int CurrentTrial
    {
      get { return currentTrial; }
      set
      {
        currentTrial = value;
        OnPropertyChanged("CurrentTrial");
      }
    }

    public MainWindow(App appInst)
    {
      InitializeComponent();
      appInstance = appInst;
      uiDispatcher = Dispatcher;
      AutoStartEffortE = false;

      CoreInstance.EffortE.PropertyChanged += new PropertyChangedEventHandler(EffortE_PropertyChanged);
    }


    private void bBrowse_Click(object sender, RoutedEventArgs e)
    {
      OpenFileDialog ofdForm = new OpenFileDialog();
      ofdForm.Filter = "Skeleton Record (*.kr)|*.kr|All Files|*.*";
      ofdForm.FilterIndex = 1;
      ofdForm.InitialDirectory = initialPlaybackFolder;
      if (ofdForm.ShowDialog().Value)
      {
        FilePath = ofdForm.FileName;
        initialPlaybackFolder = new FileInfo(FilePath).DirectoryName;
      }
    }

    private void cbUseFile_Checked(object sender, RoutedEventArgs e)
    {
      if (cbUseFile.IsChecked == true)
      {
        if (!File.Exists(FilePath))
        {
          cbUseFile.IsChecked = false;
          return;
        }
      }
    }

    /// <summary>
    /// Starts measuring effort and if "use file" is selected, then starts playing the file
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void bStart_Click(object sender, RoutedEventArgs e)
    {
      CoreInstance.EffortE.Start();
    }

    private void bStop_Click(object sender, RoutedEventArgs e)
    {
      CoreInstance.EffortE.Stop();
    }

    private void bStartPlay_click(object sender, RoutedEventArgs e)
    {
      if (cbUseFile.IsChecked == true)
        CoreInstance.PlayBack(tbFilePath.Text, Player_PlaybackFinished, true);
    }

    //start recording skeleton frame
    private void bRecordStart_Click(object sender, RoutedEventArgs e)
    {
      CoreInstance.Recorder.Start();
    }

    //stop recording skeleton frame
    private void bRecordPlayStop_Click(object sender, RoutedEventArgs e)
    {
      FilePath = CoreInstance.Recorder.Stop(true, false);
      CoreInstance.PlayBackFromFile = false;
    }

    public void Player_PlaybackFinished(object sender, EventArgs e)
    {
      CoreInstance.PlayBackFromFile = false;
      if (CoreInstance.EffortE.IsStarted)
        CoreInstance.EffortE.Stop();
    }

    private void OnPropertyChanged(String name)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(name));
    }

    private void mwObject_Loaded(object sender, RoutedEventArgs e)
    {
      CoreInstance.ColorImageReady += new EventHandler<ColorImageReadyArgs>(CoreInstance_ColorImageReady);
    }

    private void mwObject_Closed(object sender, EventArgs e)
    {
      appInstance.CloseApp(this);
    }

    void CoreInstance_ColorImageReady(object sender, ColorImageReadyArgs e)
    {
      ImageSource colorFrame = e.Frame;
      if (colorFrame == null)
        return;

      iSource.Source = colorFrame;
      //if (CoreInstance.Recorder.IsRecording && CoreInstance.Recorder.TotalTime >= 10)
      //  FilePath = CoreInstance.Recorder.Stop(true, false);
    }

    private void bSaveSequence_Click(object sender, RoutedEventArgs e)
    {
      if (tbMovementSequence.Text != null && tbMovementSequence.Text.Length > 0)
        logger.Info(tbMovementSequence.Text);
    }

    private void bReset_Click(object sender, RoutedEventArgs e)
    {
      if (CoreInstance.EffortE.IsStarted)
        CoreInstance.EffortE.Stop();
      CoreInstance.SkeletonF.Reset();
      CoreInstance.TypingE.ErrorRateLanding = 0;
      CoreInstance.TypingE.ErrorRateSelecting = 0;
      CoreInstance.TypingE.TypingMethod.Reset();
      appInstance.TextEntryW.Reset();
    }

    void EffortE_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      //when analysis finshed
      if ("IsStarted".Equals(e.PropertyName))
      {
        if (CoreInstance.EffortE.IsStarted)
        {
        }
        else
        {
          Object[] logObjects = new Object[]
          {
            DateTime.Now,
            Core.Instance.TypingE.Anchoring,
            Core.Instance.TypingE.TypingMethod,
            Core.Instance.TypingE.SelectionMethod,
            Core.Instance.TypingE.ArmStretch,
            Core.Instance.TypingE.Dexterity,
            Core.Instance.TypingE.PlaneSize,
            //summable value
            Core.Instance.EffortE.TotalTime,
            Core.Instance.TypingE.ErrorRateLanding,
            Core.Instance.TypingE.ErrorRateSelecting,

            //--- agreggated variables for right hand
            Core.Instance.EffortE.RightArmE.GetAvgCenterOfMassLenght(Core.Instance.EffortE.CurrentFrame),
            Core.Instance.EffortE.RightArmE.GetAvgCoMTheta(Core.Instance.EffortE.CurrentFrame),
            Core.Instance.EffortE.RightArmE.GetAvgElbowTheta(Core.Instance.EffortE.CurrentFrame),
            Core.Instance.EffortE.RightArmE.GetAvgWristTheta(Core.Instance.EffortE.CurrentFrame),
            Core.Instance.EffortE.RightArmE.HumanTorqueSum,
            Core.Instance.EffortE.RightArmE.AvgHumanTorque,
            Core.Instance.EffortE.RightArmE.MaxAvgHumanTorquePercent,
            Core.Instance.EffortE.RightArmE.AvgTbEndurance,
            Core.Instance.EffortE.RightArmE.TbEnduranceLostPercent,
            Core.Instance.EffortE.RightArmE.TbEnduranceLostRate,
            Core.Instance.EffortE.RightArmE.AvgCoMHumanForce,
            Core.Instance.EffortE.RightArmE.CoMHumanForceSum,
            Core.Instance.EffortE.RightArmE.MaxAvgCoMHumanForcePercent,
            Core.Instance.EffortE.RightArmE.AvgFbEndurance,
            Core.Instance.EffortE.RightArmE.FbEnduranceLostPercent,
            Core.Instance.EffortE.RightArmE.FbEnduranceLostRate,
            Core.Instance.EffortE.RightArmE.MeasuredTorqueSum,
            Core.Instance.EffortE.RightArmE.GravityTorqueSum,
            Core.Instance.EffortE.RightArmE.InertialTorqueSum,
            Core.Instance.EffortE.RightArmE.GetAvgMeasuredTorque(Core.Instance.EffortE.TotalTime),
            Core.Instance.EffortE.RightArmE.GetAvgGravityTorque(Core.Instance.EffortE.TotalTime),
            Core.Instance.EffortE.RightArmE.GetAvgInertialTorque(Core.Instance.EffortE.TotalTime),

            //--- agreggated variables for left hand
            Core.Instance.EffortE.LeftArmE.GetAvgCenterOfMassLenght(Core.Instance.EffortE.CurrentFrame), 
            Core.Instance.EffortE.LeftArmE.GetAvgCoMTheta(Core.Instance.EffortE.CurrentFrame),
            Core.Instance.EffortE.LeftArmE.GetAvgElbowTheta(Core.Instance.EffortE.CurrentFrame),
            Core.Instance.EffortE.LeftArmE.GetAvgWristTheta(Core.Instance.EffortE.CurrentFrame),
            Core.Instance.EffortE.LeftArmE.HumanTorqueSum,
            Core.Instance.EffortE.LeftArmE.AvgHumanTorque,
            Core.Instance.EffortE.LeftArmE.MaxAvgHumanTorquePercent,
            Core.Instance.EffortE.LeftArmE.AvgTbEndurance,
            Core.Instance.EffortE.LeftArmE.TbEnduranceLostPercent,
            Core.Instance.EffortE.LeftArmE.TbEnduranceLostRate,
            Core.Instance.EffortE.LeftArmE.AvgCoMHumanForce,
            Core.Instance.EffortE.LeftArmE.CoMHumanForceSum,
            Core.Instance.EffortE.LeftArmE.MaxAvgCoMHumanForcePercent,
            Core.Instance.EffortE.LeftArmE.AvgFbEndurance,
            Core.Instance.EffortE.LeftArmE.FbEnduranceLostPercent,
            Core.Instance.EffortE.LeftArmE.FbEnduranceLostRate,
            Core.Instance.EffortE.LeftArmE.MeasuredTorqueSum,
            Core.Instance.EffortE.LeftArmE.GravityTorqueSum,
            Core.Instance.EffortE.LeftArmE.InertialTorqueSum,
            Core.Instance.EffortE.LeftArmE.GetAvgMeasuredTorque(Core.Instance.EffortE.TotalTime),
            Core.Instance.EffortE.LeftArmE.GetAvgGravityTorque(Core.Instance.EffortE.TotalTime),
            Core.Instance.EffortE.LeftArmE.GetAvgInertialTorque(Core.Instance.EffortE.TotalTime),
            Core.Instance.EffortE.Gender,
            //others
            appInstance.TextEntryW.EnteredSentence
          };

          int count = 0;
          StringBuilder formatSt = new StringBuilder();
          foreach (Object obj in logObjects)
            formatSt.Append("{" + (count++) + "};");
          String experimentLog = String.Format(formatSt.ToString(), logObjects);
          logger.Info(experimentLog);

          if (conditionsInt != null)
          {
            MessageBoxResult result = MessageBox.Show("[YES]: Continue to next condition, [NO]: Re-do current condition",
              "Task Finished", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Yes)
            {
              if (CurrentTrial == NroTrails)
              {
                if (currentCondition == (conditionsInt.Length - 1))
                {
                  MessageBox.Show("Experiment Finished");
                  return;
                }
                MessageBox.Show("Condition Finished");

                currentCondition++;
                CurrentTrial = 1;
                CurrentConditionToShow = conditionsInt[currentCondition];
                LoadCondition(CurrentConditionToShow - 1);
                bReset_Click(this, null);
              }
              else
              {
                CurrentTrial++;
                bReset_Click(this, null);
              }
            }
            else
            {
              CurrentTrial = 1;
              bReset_Click(this, null);
            }
          }
        }
      }
    }

    private void btSelectionClick_Click(object sender, RoutedEventArgs e)
    {
      Core.Instance.TypingE.HasUserClicked = true;
    }

    int currentTrial = 0;
    int currentCondition = 0;
    int[] conditionsInt = null;
    Object[] conditionsValues = new Object[] 
    {
      //Experiment 2
      //SelectionMethod.Timer, PlaneSize.p35x35, 0, "Box",
      //SelectionMethod.Click, PlaneSize.p35x35, 1, "Box",
      //SelectionMethod.Click, PlaneSize.p25x25, 1, "Box",
      //SelectionMethod.Click, PlaneSize.p25x25, 0, "Box",
      //SelectionMethod.Swipe, PlaneSize.p35x35, 0, "Box",
      //SelectionMethod.Swipe, PlaneSize.p35x35, 1, "Box",
      //SelectionMethod.Swipe, PlaneSize.p25x25, 1, "Box",
      //SelectionMethod.Swipe, PlaneSize.p25x25, 0, "Box",
      //SelectionMethod.Timer, PlaneSize.p35x35, 0, "Box",
      //SelectionMethod.Timer, PlaneSize.p35x35, 1, "Box",
      //SelectionMethod.Timer, PlaneSize.p25x25, 1, "Box",
      //SelectionMethod.Timer, PlaneSize.p25x25, 0, "Box",
      //SelectionMethod.SecondHand,	PlaneSize.p35x35,	0, "Box",
      //SelectionMethod.SecondHand,	PlaneSize.p35x35,	1, "Box",
      //SelectionMethod.SecondHand,	PlaneSize.p25x25, 1, "Box",
      //SelectionMethod.SecondHand,	PlaneSize.p25x25, 0, "Box" 

      //Experiment 3
      SelectionMethod.Timer, PlaneSize.p35x35, 0, "Fitaly",
      SelectionMethod.Timer, PlaneSize.p35x35, 0, "Qwerty",
      SelectionMethod.Timer, PlaneSize.p35x35, 0, "Seato"
    };

    private void btStartExperiment_Click(object sender, RoutedEventArgs e)
    {
      if (ConditionsSequence.Length != 3)
        return;

      char[] delimiterChars = { ' ', '\t' };
      String[] conditions = ConditionsSequence.Split(delimiterChars);
      conditionsInt = conditions.Select<String, Int32>(tmp => Int32.Parse(tmp)).ToArray();
      currentCondition = 0;
      currentTrial = 0;

      CurrentTrial = currentTrial + 1;
      CurrentConditionToShow = conditionsInt[currentCondition];
      LoadCondition(CurrentConditionToShow - 1);
      bReset_Click(this, null);
    }

    private void LoadCondition(int condition)
    {
      SelectionMethod selM = (SelectionMethod)conditionsValues[condition * 4 + 0];
      PlaneSize planeS = (PlaneSize)conditionsValues[condition * 4 + 1];
      int planeLocation = (int)conditionsValues[condition * 4 + 2];
      String layoutCondition = (String)conditionsValues[condition * 4 + 3];
      CoreInstance.TypingE.SelectionMethod = selM;
      CoreInstance.TypingE.PlaneSize = planeS;
      if (planeLocation == 0)
      {
        CoreInstance.TypingE.Anchoring = LayoutAnchoring.VerticalBodyCenterLevel;
        CoreInstance.TypingE.ArmStretch = ArmStretch.Short;
      }
      else
      {
        CoreInstance.TypingE.Anchoring = LayoutAnchoring.HorizontalBottomLevel;
        CoreInstance.TypingE.ArmStretch = ArmStretch.Long;
      }

      Converters.TypingMethodConverter typingMethodConverter = new Converters.TypingMethodConverter();
      CoreInstance.TypingE.TypingMethod = (TypingMethod)typingMethodConverter.ConvertBack(true, null, layoutCondition, null);
    }
  }

}
