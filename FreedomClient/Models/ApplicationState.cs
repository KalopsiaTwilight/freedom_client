﻿using FreedomClient.Core;
using FreedomClient.Models;
using Newtonsoft.Json;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace FreedomClient.Models
{
    public enum ApplicationLoadState
    {
        NotInstalled,
        CheckForUpdate,
        VerifyingFiles,
        ReadyToLaunch
    }

    [AddINotifyPropertyChangedInterface]
    public class ApplicationState
    {
        public ApplicationState()
        {
            LastManifest = new DownloadManifest();
            Version = Assembly.GetExecutingAssembly().GetName().Version!.ToString();
            LoadState = ApplicationLoadState.NotInstalled;
            LauncherImages = new List<string>();
            InstallPath = string.Empty;
            UIOperation = UIOperation.NoOp;
        }
        public string InstallPath { get; set; }

        public DownloadManifest LastManifest { get; set; }

        public string Version { get; set; }

        public List<string> LauncherImages { get; set; }

        public Dictionary<string, string> InstalledAddons { get; set; }

        [JsonIgnore]
        public ApplicationLoadState LoadState { get;set; }

        [JsonIgnore]
        public UIOperation UIOperation { get; set; }
    }
}
