﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SIM.Tool.Windows;

namespace TrayPluginProductivityKit.SilentStartup
{
  public class MinimizedStartupManager
  {
    #region Constants

    private const string MinimizedStartupKey = "/minimized";

    #endregion

    #region Static Fields

    public static MinimizedStartupManager ActualManager = new MinimizedStartupManager();

    #endregion

    #region Fields

    private bool _shouldHideDuringStartup = true;

    #endregion

    #region Properties

    private MainWindow WindowReference { get; set; }

    #endregion

    #region Public Methods and Operators

    public static void Init(MainWindow mainWindow)
    {
      ActualManager.InitInstance(mainWindow);
    }

    #endregion

    #region Methods

    private void InitInstance(MainWindow mainWindow)
    {
      this.WindowReference = mainWindow;
      this.MakeStartupHiddenIfNeeded();
    }

    private bool IsHiddenStartup()
    {
      return
        Environment.CommandLine.Split(" ".ToCharArray())
          .Any(line => line.Equals(MinimizedStartupKey, StringComparison.OrdinalIgnoreCase));
    }

    private void MakeStartupHiddenIfNeeded()
    {
      if (!this.IsHiddenStartup())
      {
        return;
      }

      this.WindowReference.Activated += this.WindowReferenceOnActivated;
    }

    private void WindowReferenceOnActivated(object sender, EventArgs eventArgs)
    {
      //One time subscription
      this.WindowReference.Activated -= this.WindowReferenceOnActivated;
      this.WindowReference.Hide();
    }

    #endregion
  }
}