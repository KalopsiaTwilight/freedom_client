﻿using FreedomClient.Core;
using FreedomClient.Models;
using FreedomClient.ViewModels.WoW;
using FreedomClient.Views;
using FreedomClient.Views.WoW;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;
using Velopack;
using Velopack.Sources;

namespace FreedomClient.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MainWindowViewModel: IViewModel
    {
        private readonly ILogger<MainWindowViewModel> _logger;
        private readonly HttpClient _httpClient;
        private readonly ApplicationState _appState;

        public ICommand? MinimizeCommand { get; set; }
        public ICommand? CloseCommand { get; set; }
        public ICommand? OpenNavMenuCommand { get; set; }
        public ICommand? NavigateSubFrameCommand { get; set; }
        public List<MenuItem> MenuItems { get; set; }

        public object CurrentFrame { get; set; }
        public MainWindow? MainWindow { get; set; }
        public bool IsNavMenuOpen { get; set; }

        public MainWindowViewModel(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<MainWindowViewModel>>();
            _httpClient = serviceProvider.GetRequiredService<HttpClient>();
            _appState = serviceProvider.GetRequiredService<ApplicationState>();


            IsNavMenuOpen = false;

            MinimizeCommand = new RelayCommand((_) => true, (_) => { if (MainWindow != null) { MainWindow.WindowState = WindowState.Minimized; } });
            OpenNavMenuCommand = new RelayCommand((_) => !IsNavMenuOpen, (_) => IsNavMenuOpen =  true);
            CloseCommand = new RelayCommand((_) => true, (_) => { MainWindow?.Close(); });

            NavigateSubFrameCommand = new RelayCommand((_) => true, (x) =>
            {
                if (CurrentFrame is WoWShellView wowShell)
                {
                    (wowShell.DataContext as WoWShellViewModel)?.NavigateFrameCommand?.Execute(x);
                }
                IsNavMenuOpen = false;
            });

            CurrentFrame = serviceProvider.GetRequiredService<WoWShellView>();

            MenuItems = [
                new MenuItem() { PageType = typeof(WoWHomePageView), Text = (string)Application.Current.FindResource("txt_MainWindow_NavBar_PageHome"), NavigateSubFrameCommand = NavigateSubFrameCommand },
                new MenuItem() { PageType = typeof(WoWPatchesPageView), Text = (string)Application.Current.FindResource("txt_MainWindow_NavBar_PagePatches"), NavigateSubFrameCommand = NavigateSubFrameCommand },
                new MenuItem() { PageType = typeof(WoWAddonsPageView), Text = (string)Application.Current.FindResource("txt_MainWindow_NavBar_PageAddons"), NavigateSubFrameCommand = NavigateSubFrameCommand },
                new MenuItem() { PageType = typeof(WoWSettingsPageView), Text = (string)Application.Current.FindResource("txt_MainWindow_NavBar_PageSettings"), NavigateSubFrameCommand = NavigateSubFrameCommand },
            ];

            TestLatestVersion();
        }

        private async void TestLatestVersion()
        {
            try
            {
                _logger.LogInformation("Checking for launcher updates via Velopack...");
                var source = new GithubSource("https://github.com/KalopsiaTwilight/freedom_client", "", false);
                var mgr = new UpdateManager(source);

                var newVersion = await mgr.CheckForUpdatesAsync();
                if (newVersion == null)
                {
                    _logger.LogInformation($"Launcher is up to date!");
                    return;
                }
                var result = MessageBox.Show("A new launcher version is available, would you like the launcher to download and install it?", "New version available", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    await mgr.DownloadUpdatesAsync(newVersion);
                    mgr.ApplyUpdatesAndRestart(null, null);
                    return;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, null);
            }
            TestLatestVersionManual();
        }

        private async void TestLatestVersionManual()
        {
            _logger.LogInformation("Checking for launcher updates manually...");
            try
            {
                var resp = await _httpClient.GetAsync(Constants.CdnUrl + "/latestClientVersion.txt");
                if (resp.IsSuccessStatusCode)
                {
                    var versionTxt = await resp.Content.ReadAsStringAsync();
                    if (Version.Parse(_appState.Version) < Version.Parse(versionTxt))
                    {
                        _logger.LogInformation($"A launcher update is available, new version: {versionTxt}.");
                        var result = MessageBox.Show("A new launcher version is available, would you like to download it now?", "New version available", MessageBoxButton.YesNo);
                        if (result == MessageBoxResult.Yes)
                        {
                            var pStart = new ProcessStartInfo()
                            {
                                UseShellExecute = true,
                                FileName = "https://github.com/KalopsiaTwilight/freedom_client/releases/"
                            };
                            Process.Start(pStart);
                            MainWindow?.Close();
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"Launcher is up to date!");
                    }
                }
            }
            catch (HttpRequestException exc)
            {
                _logger.LogError(exc, null);
            }
        }

    }

    public class MenuItem
    {
        public ICommand? NavigateSubFrameCommand { get; set; }
        public required Type PageType { get; set; }
        public required string Text { get; set; }
    }
}
