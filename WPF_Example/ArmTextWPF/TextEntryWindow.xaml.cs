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
using System.Windows.Shapes;
using ArmText.Typing;
using ArmText.Pointing;
using ArmText.Properties;
using System.ComponentModel;
using ArmText.Util;

namespace ArmText
{

  /// <summary>
  /// Interaction logic for TextEntryWindow.xaml
  /// </summary>
  public partial class TextEntryWindow : Window, INotifyPropertyChanged
  {
    private const String ERROR_FEED_BACK = "You typed a wrong character!!!!!";
    private int taskCount = 0;
    private Key targetKey = Key.None;
    private PhraseProvider phraseProvider = null;
    private bool arrivedToTargetKey = false;
    private bool stillCorrect = true;

    private Random randon = new Random((int)DateTime.Now.Ticks);

    public App AppInstance { get; set; }

    public Core CoreInstance
    {
      get { return Core.Instance; }
    }

    public String SampleSentence
    {
      get { return tbSample.Text; }
    }

    public String EnteredSentence
    {
      get { return tbEntry.Text.Replace('_', ' '); }
    }

    public Key TargetKey
    {
      get { return targetKey; }
      set
      {
        targetKey = value;
        OnPropertyChanged("TargetKey");
      }
    }

    public int TaskCount
    {
      get { return taskCount; }
      set
      {
        taskCount = value;
        OnPropertyChanged("TaskCount");
      }
    }

    public TextEntryWindow(App appInst)
    {
      TaskCount = 20;
      AppInstance = appInst;
      InitializeComponent();
      phraseProvider = new PhraseProvider(Settings.Default.PhrasesFile);

      CoreInstance.TypingE.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(TypingE_PropertyChanged);
    }

    private void textEntryW_Closed(object sender, EventArgs e)
    {
      AppInstance.CloseApp(this);
    }

    public void Reset()
    {
      if (CoreInstance.TypingE.TypingMethod is LabyrinthTypingMethod)
      {
        labyrinthML.Reset();
        if (CoreInstance.TypingE.TypingMethod is LabyrinthTypingMethod)
          (CoreInstance.TypingE.TypingMethod as LabyrinthTypingMethod).Reset();
      }
      else if (CoreInstance.TypingE.TypingMethod is BoxTypingMethod)
      {
        if (tbEntry.Text.Length != TaskCount)
        {
          tbEntry.Text = String.Empty;
          tbSample.Text = GenerateRandomBoxSequence();
        }
        else
        {
          tbSample.Text = tbEntry.Text;
        }
        TargetKey = KeyConverter.StringToKey(tbSample.Text[0]);
      }
      else
      {
        tbSample.Text = phraseProvider.GetPhrase().ToUpper();
        TaskCount = tbSample.Text.Length;
        tbEntry.Text = String.Empty;
        TargetKey = KeyConverter.StringToKey(tbSample.Text[0]);
      }
      tbFeedback.Text = "";
      tbFeedback.Background = Brushes.Transparent;
    }

    private void RemoveKeys(List<Key> keys)
    {
      keys.Remove(Key.B);
      keys.Remove(Key.E);
      keys.Remove(Key.G);
      keys.Remove(Key.I);
      keys.Remove(Key.J);
      keys.Remove(Key.L);
      keys.Remove(Key.N);
      keys.Remove(Key.Q);
      keys.Remove(Key.T);
      keys.Remove(Key.W);
      keys.Remove(Key.Y);
      keys.Remove(Key.D0);
      keys.Remove(Key.D1);
      keys.Remove(Key.D3);
      keys.Remove(Key.D5);
      keys.Remove(Key.D8);
    }

    private string GenerateRandomBoxSequence()
    {
      StringBuilder rdnSequence = new StringBuilder();
      List<Key> unusedKeys = new List<Key>(BoxTypingMethod.layout);
      RemoveKeys(unusedKeys);
      List<Key> randomOrder = new List<Key>();
      Key randomKey;
      while (unusedKeys.Count > 0)
      {
        randomKey = unusedKeys.ElementAt(randon.Next(unusedKeys.Count));
        randomOrder.Add(randomKey);
        unusedKeys.Remove(randomKey);
      }

      foreach (Key key in randomOrder)
        rdnSequence.Append(KeyConverter.KeyToString(key, false));

      return rdnSequence.ToString();
    }

    void TypingE_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (CoreInstance.TypingE.TypingMethod is LabyrinthTypingMethod)
      {
        if (!AppInstance.MainW.AutoStartEffortE)
          return;

        if (CoreInstance.TypingE.HighlightedKey == Key.A)
          TaskStarted();
        else if (CoreInstance.TypingE.HighlightedKey == Key.U && CoreInstance.EffortE.IsStarted)
        {
          TaskFinished();
          labyrinthML.Reset();
        }
      }
      else
      {
        if ("HighlightedKey".Equals(e.PropertyName))
        {
          if (CoreInstance.TypingE.HighlightedKey == TargetKey)
            arrivedToTargetKey = true;
          else if (arrivedToTargetKey)
            CoreInstance.TypingE.ErrorRateLanding++;
        }
        else if ("SelectedKey".Equals(e.PropertyName))
        {
          if (CoreInstance.TypingE.TypingMethod is BoxTypingMethod && EnteredSentence.Length == TaskCount)
            return;
          if (CoreInstance.TypingE.SelectedKey == TargetKey)
          {
            arrivedToTargetKey = false;
            AppendCharacterToEntryText();
            if (EnteredSentence.Length == 1)
              TaskStarted();
            else if (CoreInstance.TypingE.SelectedKey == Key.OemQuestion && EnteredSentence.Equals(tbSample.Text))
              TaskFinished();
          }
          else if (CoreInstance.TypingE.SelectedKey != Key.OemPlus)
          {
            AppendCharacterToEntryText();
            if (CoreInstance.TypingE.SelectedKey != Key.None)
            {
              CoreInstance.TypingE.ErrorRateSelecting++;
              stillCorrect = false;
              tbFeedback.Text = ERROR_FEED_BACK;
              tbFeedback.Background = Brushes.Red;
            }
          }
          else if(CoreInstance.TypingE.SelectedKey == Key.OemPlus)
            AppendCharacterToEntryText();

          if (EnteredSentence.Length < tbSample.Text.Length)
            TargetKey = KeyConverter.StringToKey(tbSample.Text[EnteredSentence.Length]);
          else if (EnteredSentence.Equals(tbSample.Text))
            TargetKey = Key.OemQuestion;
        }
      }
    }

    private void AppendCharacterToEntryText()
    {
      if (CoreInstance.TypingE.TypingMethod is BoxTypingMethod && CoreInstance.TypingE.SelectedKey != TargetKey)
        return;
      if (CoreInstance.TypingE.SelectedKey == Key.OemPlus) //this is delete
      {
        if (tbEntry.Text.Length > 0)
        {
          stillCorrect = true;
          tbFeedback.Text = "";
          tbFeedback.Background = Brushes.Transparent;
          tbEntry.Text = tbEntry.Text.Substring(0, tbEntry.Text.Length - 1);
        }
      }
      else if (CoreInstance.TypingE.SelectedKey != Key.OemQuestion && stillCorrect) //this is enter
        tbEntry.Text = tbEntry.Text + KeyConverter.KeyToString(CoreInstance.TypingE.SelectedKey, true);
    }

    private void TaskStarted()
    {
      if (!AppInstance.MainW.AutoStartEffortE)
        return;

      if (!CoreInstance.EffortE.IsStarted)
        CoreInstance.EffortE.Start();
      if (!CoreInstance.Recorder.IsRecording)
        CoreInstance.Recorder.Start();
    }

    private void TaskFinished()
    {
      if (CoreInstance.Recorder.IsRecording)
        AppInstance.MainW.FilePath = CoreInstance.Recorder.Stop(true, false, EnteredSentence);
      if (CoreInstance.EffortE.IsStarted)
        CoreInstance.EffortE.Stop();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(String name, object value = null)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new ExtendedPropertyChangedEventArgs(name, value));
    }
  }

}
