﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Windows.Forms;
using SIM.Instances;
using SIM.Tool.Plugins.TrayPlugin.Messaging;
using SIM.Tool.Plugins.TrayPlugin.TrayIcon.ContextMenu;

namespace TrayPluginProductivityKit.InstancePIDs
{
  public class InsnanceMenuCollector
  {

    public Dictionary<Instance,ToolStripItem> ContextMenu { get; set; }
    protected CollectorState CurrentState { get; set; }
    public volatile bool IsUnderConstruction;

    public void Initialize()
    {
      CurrentState = CollectorState.MenuBuilded;
      ContextMenu = new Dictionary<Instance, ToolStripItem>();
      IsUnderConstruction = false;
      MessageBus.Subscribe(StandardMessagesKinds.ContextMenuProviderEntryConstructed, OnMenuEntryConstructed);
      MessageBus.Subscribe(StandardMessagesKinds.ContextMenuProviderMenuConstructed, OnContextMenuConstructed);
    }

    protected virtual void OnContextMenuConstructed(object sender, TrayPluginMessage message)
    {
      if (CurrentState == CollectorState.MenuBuilded)
        throw new Exception("State inconsistency");
      CurrentState = CollectorState.MenuBuilded;
      IsUnderConstruction = false;
    }

    protected virtual void OnMenuEntryConstructed(object sender, TrayPluginMessage message)
    {
      if (CurrentState == CollectorState.MenuBuilded)
      {
        IsUnderConstruction = true;
        ContextMenu = new Dictionary<Instance, ToolStripItem>();
        CurrentState = CollectorState.MenuBuilding;
      }
      AddInstanceEntry(message);
    }

    protected virtual void AddInstanceEntry(TrayPluginMessage message)
    {
      var constructingParams = message.Arguments as MenuEntryConstructedMessageParams;
      if (constructingParams == null)
        throw new InvalidDataException("message.Arguments is not MenuEntryConstructedMessageParams or is null");
      ToolStripItem menuItem = constructingParams.ContextMenuItem;
      if (constructingParams.Position != MenuEntryPosition.BodyEntry)
        return;
      var relatedInstance = menuItem.Tag as Instance;
      if (relatedInstance == null)
        throw new InvalidDataException("menuItem.Tag is not Instance or is null");
      ContextMenu[relatedInstance] = menuItem;
    }
  }
}
