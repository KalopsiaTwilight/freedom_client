﻿using FreedomClient.Commands;
using FreedomClient.Core;
using FreedomClient.Models;
using MediatR;
using Ookii.Dialogs.Wpf;
using PropertyChanged;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace FreedomClient.ViewModels.WoW
{

    [AddINotifyPropertyChangedInterface]
    public class WoWSettingsPageViewModel: IViewModel
    {
        public ICommand? SoftResetInstallCommand { get; set; }
        public ICommand? HardResetInstallCommand { get; set; }

        public ICommand? OpenLogDirCommand { get; set; }

        public ICommand? ChangeInstallPathCommand { get; set; }

        public ICommand? ResetDB2FilesCommand { get; set; }

        public ICommand? SecretTriggeredCommand { get; set; }

        public ApplicationState ApplicationState { get; set; }

        public string LogPath { get; set; }

        public string Version { get; set; }

        public WoWSettingsPageViewModel(ApplicationState appState, IMediator mediator)
        {
            ApplicationState = appState;
            var localDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            LogPath = System.IO.Path.Join(localDataPath, Constants.AppIdentifier);
            Version = appState.Version;
            OpenLogDirCommand = new RelayCommand((_) => true, (_) =>
            {
                Process.Start("explorer", $@"""{LogPath}""");
            });
            SoftResetInstallCommand = new RelayCommand((_) => !ApplicationState.UIOperation.IsBusy && !string.IsNullOrEmpty(ApplicationState.InstallPath),
                (_) =>
                {
                    mediator.Send(new RestoreWoWClientFilesCommand() { CompleteReset = false });
                });
            HardResetInstallCommand = new RelayCommand((_) => !ApplicationState.UIOperation.IsBusy && !string.IsNullOrEmpty(ApplicationState.InstallPath),
                (_) =>
                {
                    var result = MessageBox.Show("Warning: This will remove any files that weren't included in a base install including TRP data and any addons/patches you've installed. You might want to back up your WTF/Addon & any folders for personal patches before proceeding. Are you sure you want to COMPLETELY reset your install?", "Hard Reset Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        mediator.Send(new RestoreWoWClientFilesCommand() { CompleteReset = true });
                    }
                });
            ChangeInstallPathCommand = new RelayCommand((_) => !ApplicationState.UIOperation.IsBusy,
                (_) =>
                {
                    var folderDialog = new VistaFolderBrowserDialog();
                    if (folderDialog.ShowDialog() != true)
                    {
                        return;
                    }

                    ApplicationState.InstallPath = folderDialog.SelectedPath;
                    mediator.Send(new RestoreWoWClientFilesCommand() { CompleteReset = false });
                });
            SecretTriggeredCommand = new RelayCommand((_) => true,
                (_) =>
                {
                    ApplicationState.UIOperation.Message = "You entered the secret code!";
                });
            ResetDB2FilesCommand = new RelayCommand((_) => !ApplicationState.UIOperation.IsBusy,
                (_) =>
                {
                    mediator.Send(new RestoreWoWDB2FilesCommand() { });
                });
        }
    }
}
