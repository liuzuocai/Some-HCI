﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using ArmText.Properties;
using System.Xml;
using System.Reflection;

namespace ArmText
{

  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {

    public MainWindow MainW { get; set; }
    public TextEntryWindow TextEntryW { get; set; }

    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);
      log4net.Config.XmlConfigurator.Configure();
      String destFolder = Settings.Default.DestFolder.Replace("{USERNAME}", Environment.UserName);
      Core.Instance.Initialize(Dispatcher, Settings.Default.SkeletonBufferSize, Settings.Default.AvgHeight,
        destFolder, Settings.Default.PlayerBufferSize);

      MainW = new MainWindow(this);
      TextEntryW = new TextEntryWindow(this);

      MainW.Show();
      TextEntryW.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
      base.OnExit(e);
      Core.Instance.Shutdown();
    }

    public void CloseApp(Window sender)
    {
      if (sender != MainW)
        MainW.Close();
      if (sender != TextEntryW)
        TextEntryW.Close();
    }

  }

}