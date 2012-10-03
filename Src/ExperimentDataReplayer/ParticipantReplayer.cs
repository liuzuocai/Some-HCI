using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExperimentDataReplayer.Properties;
using ArmText;
using System.IO;
using System.ComponentModel;
using ArmText.Typing;

namespace ExperimentDataReplayer
{
  public class ParticipantReplayer
  {

    public event PropertyChangedEventHandler PropertyChanged;

    private int playFileNo;
    private int participantID;
    private string[] filePaths;

    public static int TryTime { get; set; }
    public int ParticipantID { get; set; }
    public int TrialNo { get; set; }
    public int ConditionNo { get; set; }
    public String FileDetail { get; set; }
    public ParticipantReplayer(String inputPath, int id)
    {
      participantID = id;
      playFileNo = 0;
      filePaths = SortFiles(Directory.GetFiles(inputPath, "*.kr"));
    }

    public void Replay()
    {
      while (playFileNo < filePaths.Length)
      {
        if (!Core.Instance.PlayBackFromFile)
        {
          GetFileInformation(filePaths[playFileNo]);
          Core.Instance.SkeletonF.Reset();
          Core.Instance.EffortE.Start();
          Core.Instance.PlayBack(filePaths[playFileNo], Player_PlaybackFinished, false);
          playFileNo++;
        }
        else
        {
          System.Threading.Thread.Sleep(300);
        }
      }
    }

    private void GetFileInformation(String fileInfo)
    {
      String[] fileSplit = fileInfo.Split('-');
      if (fileSplit[2].Equals("box"))
        Core.Instance.TypingE.TypingMethod = new ArmText.Pointing.BoxTypingMethod();
      else if (fileSplit[2].Equals("querty"))
        Core.Instance.TypingE.TypingMethod = new ArmText.Typing.QwertyTypingMethod();
      else if (fileSplit[2].Equals("Seato"))
        Core.Instance.TypingE.TypingMethod = new ArmText.Typing.SeatoTypingMethod();
      else if (fileSplit[2].Equals("fitaly"))
        Core.Instance.TypingE.TypingMethod = new ArmText.Typing.SeatoTypingMethod();

      if (fileSplit[3].Equals("vs"))
        Core.Instance.TypingE.Anchoring = LayoutAnchoring.VerticalShoulderLevel;
      else if (fileSplit[3].Equals("hb"))
        Core.Instance.TypingE.Anchoring = LayoutAnchoring.HorizontalBottomLevel;
      else if (fileSplit[3].Equals("ve"))
        Core.Instance.TypingE.Anchoring = LayoutAnchoring.VerticalBodyCenterLevel;

      if (fileSplit[4].Equals("l"))
        Core.Instance.TypingE.ArmStretch = ArmStretch.Long;
      else if (fileSplit[4].Equals("s"))
        Core.Instance.TypingE.ArmStretch = ArmStretch.Short;

      if (fileSplit[5].Equals("click"))
        Core.Instance.TypingE.SelectionMethod = SelectionMethod.Click;
      else if (fileSplit[5].Equals("push"))
        Core.Instance.TypingE.SelectionMethod = SelectionMethod.Push;
      else if (fileSplit[5].Equals("secondhand"))
        Core.Instance.TypingE.SelectionMethod = SelectionMethod.SecondHand;
      else if (fileSplit[5].Equals("swipe"))
        Core.Instance.TypingE.SelectionMethod = SelectionMethod.Swipe;
      else if (fileSplit[5].Equals("timer"))
        Core.Instance.TypingE.SelectionMethod = SelectionMethod.Timer;

      if (fileSplit.Length == 8)
      {
        if (fileSplit[6].Equals("male"))
          Core.Instance.EffortE.Gender = ArmText.Effort.TypingGender.Male;
        else
          Core.Instance.EffortE.Gender = ArmText.Effort.TypingGender.Female;
        FileDetail = fileSplit[7];
      }
      else
      {
        Core.Instance.EffortE.Gender = ArmText.Effort.TypingGender.Male;
        FileDetail = fileSplit[6];
      }

      Core.Instance.TypingE.Dexterity = TypingDexterity.Right;
      Core.Instance.TypingE.PlaneSize = PlaneSize.p35x35;
      ParticipantID = participantID;
      TrialNo = playFileNo % TryTime + 1;
      ConditionNo = playFileNo / TryTime + 1;
    }

    private void Player_PlaybackFinished(object sender, EventArgs e)
    {
      if (Core.Instance.EffortE.IsStarted)
        Core.Instance.EffortE.Stop(this);
      Core.Instance.PlayBackFromFile = false;
      Console.WriteLine("Finished Playing: {0}", filePaths[playFileNo - 1]);
    }

    private string[] SortFiles(string[] files)
    {
      var tmp = files.OrderBy(file => DateTime.ParseExact(file.Substring(file.LastIndexOf(@"\") + 1, 13), "MMddyy-HHmmss", null)).ToArray();
      return tmp;
    }

  }
}
