using System;
using Windows.Devices.Power;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace ModernFlyouts.Uwp
{
  public sealed partial class BatteryFlyoutControl : UserControl
  {
        public BatteryReport BatteryInfo { get; set; }

        public BatteryFlyoutControl()
        {
            InitializeComponent();
            Battery.AggregateBattery.ReportUpdated += AggregateBattery_ReportUpdated;
            RefreshAsync();
        }
       
        private void AggregateBattery_ReportUpdated(Battery sender, object args) => RefreshAsync();

        public async void ShowFlyout() => await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { FlyoutBase.ShowAttachedFlyout(FlyoutGrid); });

        public async void RefreshAsync() => await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
            BatteryInfo = Battery.AggregateBattery.GetReport();
            Bindings.Update();
        });
  }
}