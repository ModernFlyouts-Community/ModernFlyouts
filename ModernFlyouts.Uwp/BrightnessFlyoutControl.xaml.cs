using System;
using Windows.Devices.Power;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using ModernFlyouts.Standard.Classes;
using System.Collections.ObjectModel;
using Fluent.Icons;

namespace ModernFlyouts.Uwp
{
    public sealed partial class BrightnessFlyoutControl : UserControl
    {
        public BrightnessFlyoutControl() => InitializeComponent();

        public async void ShowFlyout() => await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { FlyoutBase.ShowAttachedFlyout(FlyoutGrid); });
    }
}