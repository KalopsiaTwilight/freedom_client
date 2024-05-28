using FreedomClient.ViewModels.WoW;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FreedomClient.Views.WoW
{
    /// <summary>
    /// Interaction logic for WoWSettingsPage.xaml
    /// </summary>
    public partial class WoWSettingsPageView : Page
    {
        //private readonly ApplicationState _applicationState;
        public WoWSettingsPageView(WoWSettingsPageViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            Unloaded += OnUnload;
            Loaded += OnLoad;
        }

        private void OnLoad(object sender, EventArgs e)
        {
            Window.GetWindow(this).PreviewKeyDown += On_KeyDown;
        }

        private void OnUnload(object sender, EventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window != null)
            {
                window.PreviewKeyDown -= On_KeyDown;
            }
        }


        private readonly Key[] SecretCode = [Key.Up, Key.Up, Key.Down, Key.Down, Key.Left, Key.Right, Key.Left, Key.Right];

        private int SecretIndex = 0;

        private void On_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == SecretCode[SecretIndex]) {
                SecretIndex++;
                if (SecretIndex  >= SecretCode.Length)
                {
                    SecretIndex = 0;
                    ((WoWSettingsPageViewModel)DataContext).SecretTriggeredCommand?.Execute(null);
                }
            } else
            {
                SecretIndex = 0;
            }
        }
    }
}
