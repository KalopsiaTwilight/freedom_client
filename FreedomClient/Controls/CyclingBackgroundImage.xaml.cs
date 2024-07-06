using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FreedomClient.Controls
{
    /// <summary>
    /// Interaction logic for CyclingBackgroundImage.xaml
    /// </summary>
    public partial class CyclingBackgroundImage : UserControl
    {
        private const int CyclingTimeMs = 10000;

        private Timer? _timer;
        private int _currentIndex;
        public ILogger<CyclingBackgroundImage>? Logger;

        public CyclingBackgroundImage()
        {
            InitializeComponent();

            CurrentVisualState = "Determinate";
            CommonStates.CurrentStateChanged += (se, ev) =>
            {
                CurrentVisualState = ev.NewState.Name;
            };
        }

        public static void UpdateVisualState(object? state)
        {
            var me = state as CyclingBackgroundImage;
            if (me == null)
            {
                return;
            }
            GoToNextImage(me);
        }


        private void BtnPrev_Click(object sender, RoutedEventArgs e)
        {
            _timer?.Change(CyclingTimeMs, CyclingTimeMs);
            Task.Run(() => GoToPrevImage(this));
        }
        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            _timer?.Change(CyclingTimeMs, CyclingTimeMs);
            Task.Run(() => GoToNextImage(this));
        }

        private static void GoToNextImage(CyclingBackgroundImage me)
        {
            me.Dispatcher.Invoke(() =>
            {
                VisualStateManager.GoToState(me, "CyclingForward", true);
            });
            Thread.Sleep(2200);
            me.Dispatcher.Invoke(() =>
            {
                me._currentIndex = (me._currentIndex + 1) % me.ImagePaths.Count;
                me.PrevImageSource = me.SafeGetImageAtIndex((me._currentIndex - 1 + me.ImagePaths.Count) % me.ImagePaths.Count);
                me.MainImageSource = me.SafeGetImageAtIndex(me._currentIndex);
                me.NextImageSource = me.SafeGetImageAtIndex((me._currentIndex + 1) % me.ImagePaths.Count);
                VisualStateManager.GoToState(me, "Determinate", true);
            });
        }
        private static void GoToPrevImage(CyclingBackgroundImage me)
        {
            me.Dispatcher.Invoke(() =>
            {
                VisualStateManager.GoToState(me, "CyclingBackward", true);
            });
            Thread.Sleep(2200);
            me.Dispatcher.Invoke(() =>
            {
                me._currentIndex = (me._currentIndex - 1 + me.ImagePaths.Count) % me.ImagePaths.Count;
                me.PrevImageSource = me.SafeGetImageAtIndex((me._currentIndex - 1 + me.ImagePaths.Count) % me.ImagePaths.Count);
                me.MainImageSource = me.SafeGetImageAtIndex(me._currentIndex);
                me.NextImageSource = me.SafeGetImageAtIndex((me._currentIndex + 1) % me.ImagePaths.Count);
                VisualStateManager.GoToState(me, "Determinate", true);
            });
        }

        private BitmapImage SafeGetImageAtIndex(int index)
        {
            if (ImagePaths.Count == 0)
            {
                return new BitmapImage();
            }
            if (index >= ImagePaths.Count)
            {
                index = ImagePaths.Count - 1;
            }
            try
            {
                return new BitmapImage(new Uri(ImagePaths[index]));
            }
            // Corrupted File
            catch (NotSupportedException)
            {
                var filePath = ImagePaths[index];
                Dispatcher.InvokeAsync(async () =>
                {
                    await Task.Delay(5000);
                    try
                    {
                        File.Delete(filePath);
                    }
                    catch { }
                });
                ImagePaths.RemoveAt(index);
                return SafeGetImageAtIndex(index);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, null);
                return new BitmapImage();
            }
        }

        public static void OnImagePathsChange(DependencyObject dObject, DependencyPropertyChangedEventArgs e)
        {
            if (dObject is CyclingBackgroundImage me)
            {
                me._timer?.Dispose();
                if (me.ImagePaths.Count == 1)
                {
                    me.MainImageSource = me.SafeGetImageAtIndex(0);
                }
                else if (me.ImagePaths.Count > 1)
                {
                    var currentImage = (me.MainImageSource as BitmapImage);
                    var foundIndex = me.ImagePaths.IndexOf(currentImage?.UriSource?.AbsolutePath?.Replace("/", "\\") ?? "");
                    if (currentImage == null || foundIndex == -1)
                    {
                        me._currentIndex = Random.Shared.Next(0, me.ImagePaths.Count - 1);
                        me.MainImageSource = me.SafeGetImageAtIndex(me._currentIndex);
                    }
                    else
                    {
                        me._currentIndex = foundIndex;
                    }
                    me.PrevImageSource = me.SafeGetImageAtIndex((me._currentIndex - 1 + me.ImagePaths.Count) % me.ImagePaths.Count);
                    me.NextImageSource = me.SafeGetImageAtIndex(me._currentIndex + 1 % me.ImagePaths.Count);
                    me._timer = new Timer(new TimerCallback(UpdateVisualState), me, CyclingTimeMs, CyclingTimeMs);
                }

            }
        }

        public string CurrentVisualState
        {
            get { return (string)GetValue(CurrentVisualStateProperty); }
            set { SetValue(CurrentVisualStateProperty, value); }
        }

        public ObservableCollection<string> ImagePaths
        {
            get { return (ObservableCollection<string>)GetValue(ImagePathsProperty); }
            set { SetValue(ImagePathsProperty, value); }
        }
        private ImageSource PrevImageSource
        {
            get { return (ImageSource)GetValue(PrevImageSourceProperty); }
            set { SetValue(PrevImageSourceProperty, value); }
        }

        private ImageSource MainImageSource
        {
            get { return (ImageSource)GetValue(MainImageSourceProperty); }
            set { SetValue(MainImageSourceProperty, value); }
        }
        private ImageSource NextImageSource
        {
            get { return (ImageSource)GetValue(NextImageSourceProperty); }
            set { SetValue(NextImageSourceProperty, value); }
        }

        public static readonly DependencyProperty CurrentVisualStateProperty =
            DependencyProperty.Register(
                nameof(CurrentVisualState),
                typeof(string),
                typeof(CyclingBackgroundImage));

        public static readonly DependencyProperty MainImageSourceProperty =
            DependencyProperty.Register(
                nameof(MainImageSource),
                typeof(ImageSource),
                typeof(CyclingBackgroundImage));

        public static readonly DependencyProperty PrevImageSourceProperty =
            DependencyProperty.Register(
                nameof(PrevImageSource),
                typeof(ImageSource),
                typeof(CyclingBackgroundImage));

        public static readonly DependencyProperty NextImageSourceProperty =
            DependencyProperty.Register(
                nameof(NextImageSource),
                typeof(ImageSource),
                typeof(CyclingBackgroundImage));

        public static readonly DependencyProperty ImagePathsProperty =
            DependencyProperty.Register(
                nameof(ImagePaths),
                typeof(ObservableCollection<string>),
                typeof(CyclingBackgroundImage),
                new PropertyMetadata(new PropertyChangedCallback(OnImagePathsChange)));
    }
}
