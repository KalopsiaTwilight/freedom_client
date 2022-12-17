﻿using System;
using System.Collections.Generic;
using System.Linq;
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
        private Timer _timer;
        private List<string> _images;
        private int _currentIndex;

        public CyclingBackgroundImage()
        {
            InitializeComponent();

            //_timer = new Timer(new TimerCallback(UpdateVisualState), this, 0, 10000);
        }

        private void UpdateVisualState(object? state)
        {
            var me = state as CyclingBackgroundImage;
            me.Dispatcher.Invoke(() =>
            {
                VisualStateManager.GoToState(me, "Cycling", true);
            });
            Thread.Sleep(2200);
            me.Dispatcher.Invoke(() =>
            {
                _currentIndex = (_currentIndex + 1) % _images.Count;
                me.Image1Source = new BitmapImage(new Uri(_images[_currentIndex]));
                me.Image2Source = new BitmapImage(new Uri(_images[(_currentIndex + 1)%_images.Count]));
                VisualStateManager.GoToState(me, "Determinate", true);
            });
        }

        private void OnImagePathsChange()
        {
            if (_timer != null)
            {
                _timer.Dispose();
            }
            if (_images.Count > 0)
            {
                Image1Source = new BitmapImage(new Uri(_images[0]));
            }
            if (_images.Count > 1)
            {
                Image2Source = new BitmapImage(new Uri(_images[1]));
                _currentIndex = 0;
                _timer = new Timer(new TimerCallback(UpdateVisualState), this, 10000, 10000);
            }
        }

        public List<string> ImagePaths
        {
            get => _images;
            set { _images = value; OnImagePathsChange(); }
        }

        private ImageSource Image1Source
        {
            get { return (ImageSource)GetValue(Image1SourceProperty); }
            set { SetValue(Image1SourceProperty, value); }
        }
        private ImageSource Image2Source
        {
            get { return (ImageSource)GetValue(Image2SourceProperty); }
            set { SetValue(Image2SourceProperty, value); }
        }

        public static readonly DependencyProperty Image1SourceProperty =
            DependencyProperty.Register(
                "Image1Source",
                typeof(ImageSource),
                typeof(CyclingBackgroundImage));

        public static readonly DependencyProperty Image2SourceProperty =
            DependencyProperty.Register(
                "Image2Source",
                typeof(ImageSource),
                typeof(CyclingBackgroundImage));
    }
}
