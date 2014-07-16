﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using SIM.Instances;
using SIM.Tool.Plugins.TrayPlugin.Resourcing;

namespace TrayPluginProductivityKit.InstanceMarking
{
  public class FilesystemMarkingProvider
  {
    #region Enums

    [Flags]
    public enum FOLDERCUSTOMSETTINGSMASK : uint
    {
      FCSM_VIEWID = 0x0001,
      FCSM_WEBVIEWTEMPLATE = 0x0002,
      FCSM_INFOTIP = 0x0004,
      FCSM_CLSID = 0x0008,
      FCSM_ICONFILE = 0x0010,
      FCSM_LOGO = 0x0020,
      FCSM_FLAGS = 0x0040,
    }

    #endregion

    #region Public Methods and Operators

    public virtual bool IsInstanceMarked(Instance instance)
    {
      return File.Exists(GetDesktopIniPath(instance));
    }

    public virtual bool MarkInstance(Instance instance)
    {
      var rootPath = instance.RootPath;
      if (!this.CopyFavIcon(rootPath))
      {
        return false;
      }
      var iconPath = GetFavIconPath(rootPath);
      return this.ChangeFolderIconNative(rootPath, iconPath);
    }

    public virtual void UnMarkInstance(Instance instance)
    {
      string rootPath = instance.RootPath;
      File.Delete(GetDesktopIniPath(rootPath));
      this.ChangeFolderIconNative(rootPath, null);
    }

    #endregion

    #region Methods

    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    protected static extern UInt32 SHGetSetFolderCustomSettings(ref SHFOLDERCUSTOMSETTINGS pfcs, [MarshalAs(UnmanagedType.LPWStr)] string pszPath, UInt32 dwReadWrite);

    protected virtual bool ChangeFolderIconNative(string folderPath, string iconPath)
    {
      SHFOLDERCUSTOMSETTINGS folderSettings = new SHFOLDERCUSTOMSETTINGS();
      folderSettings.dwMask = FOLDERCUSTOMSETTINGSMASK.FCSM_ICONFILE;

      folderSettings.dwSize = (uint)Marshal.SizeOf(typeof(SHFOLDERCUSTOMSETTINGS));
      if (!string.IsNullOrEmpty(iconPath))
      {
        folderSettings.pszIconFile = iconPath;
        folderSettings.iIconIndex = 0;
        folderSettings.cchIconFile = 0;
      }
      UInt32 FCS_READ = 0x00000001;
      UInt32 FCS_FORCEWRITE = 0x00000002;
      UInt32 FCS_WRITE = FCS_FORCEWRITE;

      string pszPath = folderPath;
      UInt32 HRESULT = SHGetSetFolderCustomSettings(ref folderSettings, pszPath, FCS_WRITE);
      return HRESULT == 0;
    }


    protected virtual bool CopyFavIcon(string folderPath)
    {
      string iconPath = GetFavIconPath(folderPath);
      if (File.Exists(iconPath))
      {
        return true;
      }
      try
      {
        var obj = MultisourceResourcesManager.GetIconResource("ProductivityKitFolderFavicon", null);
        if (obj == null)
        {
          return false;
        }
        using (FileStream favIconStream = File.Create(iconPath))
        {
          obj.Save(favIconStream);
        }
        var fi = new FileInfo(iconPath);
        fi.Attributes = FileAttributes.Hidden | FileAttributes.System;
        return true;
      }
      catch
      {
        return false;
      }
    }

    protected virtual string GetDesktopIniPath(Instance instance)
    {
      return GetDesktopIniPath(instance.RootPath);
    }

    protected virtual string GetDesktopIniPath(string rootPath)
    {
      return Path.Combine(rootPath, "desktop.ini");
    }

    protected virtual string GetFavIconPath(Instance instance)
    {
      return GetFavIconPath(instance.RootPath);
    }

    protected virtual string GetFavIconPath(string rootPath)
    {
      return Path.Combine(rootPath, "favicon.ico");
    }

    #endregion

    /*
     [StructLayout(LayoutKind.Sequential,CharSet = CharSet.Unicode)]
    public struct SHFOLDERCUSTOMSETTINGS
    {
      public uint dwSize;
      public FOLDERCUSTOMSETTINGSMASK dwMask;
      public IntPtr pvid;

      [MarshalAs(UnmanagedType.LPWStr)]
      public string pszWebViewTemplate;
      public uint cchWebViewTemplate;

      [MarshalAs(UnmanagedType.LPWStr)]
      public string pszWebViewTemplateVersion;

      [MarshalAs(UnmanagedType.LPWStr)]
      public string pszInfoTip;
      public uint cchInfoTip;

      public IntPtr pclsid;
      public uint dwFlags;

      [MarshalAs(UnmanagedType.LPWStr)]
      public string pszIconFile;
      public uint cchIconFile;
      public uint iIconIndex;

      [MarshalAs(UnmanagedType.LPWStr)]
      public string pszLogo;
      public uint cchLogo;
    }
     
     */

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct SHFOLDERCUSTOMSETTINGS
    {
      #region Fields

      public uint dwSize;
      public FOLDERCUSTOMSETTINGSMASK dwMask;
      public IntPtr pvid;

      [MarshalAs(UnmanagedType.LPWStr)]
      public string pszWebViewTemplate;
      public uint cchWebViewTemplate;

      [MarshalAs(UnmanagedType.LPWStr)]
      public string pszWebViewTemplateVersion;

      [MarshalAs(UnmanagedType.LPWStr)]
      public string pszInfoTip;
      public uint cchInfoTip;

      public IntPtr pclsid;
      public uint dwFlags;

      [MarshalAs(UnmanagedType.LPWStr)]
      public string pszIconFile;
      public uint cchIconFile;
      public uint iIconIndex;

      [MarshalAs(UnmanagedType.LPWStr)]
      public string pszLogo;
      public uint cchLogo;

      #endregion
    }
  }
}