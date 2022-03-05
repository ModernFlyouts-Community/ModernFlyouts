using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.Toolkit.Wpf.UI.XamlHost;
using NPSMLib;
using ModernFlyouts.Uwp;
using System.Diagnostics;
using ModernFlyouts.Core.Utilities;
using ModernFlyouts.Standard.Classes;
using System.Linq;
using ModernFlyouts.Utilities;
using ModernFlyouts.Wpf.Services;
using Windows.UI.Xaml.Controls.Primitives;
using System.Timers;
using System.Windows.Threading;
using ModernFlyouts.Wpf.Helpers;

namespace ModernFlyouts.Wpf
{
    public partial class BrightnessFlyoutWindow : Window
    {
        private BrightnessFlyoutControl Brightnessflyoutcontrol;

        public BrightnessFlyoutWindow() => InitializeComponent();

        private void OnFlyoutControlLoaded(object sender, EventArgs e)
        {
            Brightnessflyoutcontrol = (sender as WindowsXamlHost).Child as BrightnessFlyoutControl;
            Brightnessflyoutcontrol.BrightnessSlider.Value = (int)LaptopBrightnessHelper.GetCurrentBrightness();
            Brightnessflyoutcontrol.BrightnessSlider.ValueChanged += BrightnessSlider_ValueChanged;
            DispatcherTimer BrightnessListener = new DispatcherTimer();
            BrightnessListener.Tick += BrightnessListener_Tick;
            BrightnessListener.Interval = new TimeSpan(0, 0, 0, 0, 100);
            BrightnessListener.Start();
        }

        bool IsUpdatingTEST = false;
        private void BrightnessListener_Tick(object sender, EventArgs e)
        {
             if (IsUpdatingTEST == false)
             {
                 IsUpdatingTEST = true;
                 if ((int)LaptopBrightnessHelper.GetCurrentBrightness() != (int)Brightnessflyoutcontrol.BrightnessSlider.Value)
                 {
                     ShowFlyout();
                     Brightnessflyoutcontrol.BrightnessSlider.Value = (int)LaptopBrightnessHelper.GetCurrentBrightness();
                }
                 IsUpdatingTEST = false;
             }
        }

        private void BrightnessSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e) => UpdateBrightness(e.NewValue);

        public void UpdateBrightness(double newBrightness)
        {
            if (IsUpdatingTEST == false)
            {
                LaptopBrightnessHelper.SetBrightness((byte)newBrightness);
            }
        }

        #region Flyout skeleton code

        private void Window_Activated(object sender, EventArgs e)
        {
            var desktopWorkingArea = SystemParameters.WorkArea;
            Left = desktopWorkingArea.Right - Width;
            Top = desktopWorkingArea.Bottom - Height;
        }

        private void Window_Deactivated(object sender, EventArgs e) => Dispatcher.Invoke(() => { Close(); });

        public void ShowFlyout()
        {
            if (Brightnessflyoutcontrol == null) return;
            Activate();
            Brightnessflyoutcontrol.ShowFlyout();
        }
        private void DragWindow(object sender, MouseButtonEventArgs e) => DragMove();

        #endregion
    }
}