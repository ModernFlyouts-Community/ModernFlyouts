using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Toolkit.Wpf.UI.XamlHost;
using ModernFlyouts.Uwp;
using ModernFlyouts.Standard.Classes;
using ModernFlyouts.Standard.Services;
using Windows.UI.Xaml.Controls.Primitives;

namespace ModernFlyouts.Wpf
{
    public partial class BatteryFlyoutWindow : Window
    {
        private BatteryFlyoutControl Batteryflyoutcontrol;

        public BatteryFlyoutWindow() => InitializeComponent();

        private void OnFlyoutControlLoaded(object sender, EventArgs e)
        {
            Batteryflyoutcontrol = (sender as WindowsXamlHost).Child as BatteryFlyoutControl;
            Batteryflyoutcontrol.PowerSlider.ValueChanged += Slider_ValueChanged;
            UpdateSlider();
        }

        public void UpdateSlider() => Batteryflyoutcontrol.PowerSlider.Value = PowerPlanService.GetPlanIndex();

        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e) => ChangePlan(e.NewValue);

        /// <summary>
        /// Sets a power scheme based on the new slider value
        /// </summary>
        /// <param name="value">A double value from the slider position</param>
        /// NOTE THIS CODE IS OUTDATED AFTER THE SWITCH TO TUPLES. TO DO: ADAPT TO TUPLE SYSTEM
        public void ChangePlan(double value)
        {
            Guid currentMode = PowerPlanService.GetCurrentPlan();
            switch (value)
            {
                case 0:
                    if (currentMode != PowerMode.PowerSaver.Item2)
                        PowerPlanService.SetPowerPlan(PowerMode.PowerSaver.Item2);
                    break;
                case 1:
                    if (currentMode != PowerMode.Recommended.Item2)
                        PowerPlanService.SetPowerPlan(PowerMode.Recommended.Item2);
                    break;
                case 2:
                    if(currentMode != PowerMode.BetterPerformance.Item2)
                       PowerPlanService.SetPowerPlan(PowerMode.BetterPerformance.Item2);
                    break;
                case 3:
                    if (currentMode != PowerMode.BestPerformance.Item2)
                        PowerPlanService.SetPowerPlan(PowerMode.BestPerformance.Item2);
                    break;
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
            if (Batteryflyoutcontrol == null) return;

            Activate();
            Batteryflyoutcontrol.ShowFlyout();
        }
        private void DragWindow(object sender, MouseButtonEventArgs e) => DragMove();

        #endregion
    }
}