using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArmText;
using ExperimentDataReplayer.Properties;
using System.IO;

namespace ExperimentDataReplayer
{
  class ReplaySystem
  {
    private const int SUMMABLE_START_INDEX = 8;

    private static readonly log4net.ILog recaptureLogger = log4net.LogManager.GetLogger("RecaptureLogger");
    private static readonly log4net.ILog avgRecaptureLogger = log4net.LogManager.GetLogger("AvgRecaptureLogger");

    private static List<ParticipantReplayer> participantList = null;
    private static ParticipantReplayer currentReplayer = null;
    private static Object[] avgLogObjects = null;
    private static bool[] summableList = null;

    static void Main(string[] args)
    {
      participantList = new List<ParticipantReplayer>();

      log4net.Config.XmlConfigurator.Configure();
      Core.Instance.Initialize(null, Settings.Default.SkeletonBufferSize, Settings.Default.AvgHeight,
        null, Settings.Default.PlayerBufferSize);
      Core.Instance.EffortE.PropertyChanged += EffortE_PropertyChanged;
      if (!Directory.Exists(args[0]))
        throw new ArgumentException(String.Format("Folder {0} does not exist.", args[0]));
      Console.WriteLine("Data Folder: {0}", args[0]);
      ParticipantReplayer.TryTime = Int16.Parse(args[1]);
      CreateParticipantList(args[0]);
      foreach (ParticipantReplayer pReplayer in participantList)
      {
        currentReplayer = pReplayer;
        currentReplayer.Replay();
      }

      Console.Write("\nPlease press any key to finish> ");
      Console.ReadKey();
    }

    public static void CreateParticipantList(String inputPath)
    {
      String[] participantPaths = SortFolders(Directory.GetDirectories(inputPath, "P*"));
      for (int i = 0; i < participantPaths.Length; i++)
      {
        participantList.Add(new ParticipantReplayer(participantPaths[i], i + 1));
      }
    }

    static void EffortE_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      //when analysis finshed
      if ("IsStarted".Equals(e.PropertyName))
      {
        if (Core.Instance.EffortE.IsStarted)
        {
        }
        else
        {
          ParticipantReplayer replayer = (ParticipantReplayer)Core.Instance.EffortE.StopParameter;

          Object[] logObjects = new Object[]
          {
            "P"+ replayer.ParticipantID,
            "C"+ replayer.ConditionNo,
            "T"+ replayer.TrialNo,
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
            replayer.FileDetail
          };

          if (avgLogObjects == null)
            avgLogObjects = logObjects;

          if (summableList == null)
          {
            summableList = new bool[logObjects.Length];
            for (int i = SUMMABLE_START_INDEX; i < summableList.Length - 1; i++)
              summableList[i] = true;
          }

          for (int i = 0; i < logObjects.Length; i++)
          {
            if (replayer.TrialNo == 1)
              avgLogObjects[i] = logObjects[i];
            else
            {
              if (!summableList[i])
                continue;

              if (logObjects[i] is double)
              {
                avgLogObjects[i] = (double)avgLogObjects[i] + (double)logObjects[i];
                if (replayer.TrialNo ==ParticipantReplayer.TryTime)
                  avgLogObjects[i] = (double)avgLogObjects[i] / ParticipantReplayer.TryTime;
              }
              else if (logObjects[i] is int)
              {
                avgLogObjects[i] = (int)avgLogObjects[i] + (int)logObjects[i];
                if (replayer.TrialNo == ParticipantReplayer.TryTime)
                  avgLogObjects[i] = (int)avgLogObjects[i] / ParticipantReplayer.TryTime;
              }
            }
          }

          recaptureLogger.Info(FormatObject(logObjects));
          if (replayer.TrialNo == ParticipantReplayer.TryTime)
            avgRecaptureLogger.Info(FormatObject(avgLogObjects));
        }
      }
    }

    private static String FormatObject(Object[] objects)
    {
      int count = 0;
      StringBuilder formatSt = new StringBuilder();
      foreach (Object obj in objects)
        formatSt.Append("{" + (count++) + "};");
      String objectArrayString = String.Format(formatSt.ToString(), objects);
      return objectArrayString;
    }

    private static string[] SortFolders(string[] folders)
    {
      int tmp = 0;
      var participantFolders = folders.Where(folder => Int32.TryParse(folder.Substring(folder.LastIndexOf(@"P") + 1), out tmp));
      var orderedFolders = participantFolders.OrderBy(folder => Int32.Parse(folder.Substring(folder.LastIndexOf(@"P") + 1))).ToArray();
      return orderedFolders;
    }
  }
}
