using System;
using Windows.Devices.Power;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using ModernFlyouts.Standard.Classes;
using System.Collections.ObjectModel;

namespace ModernFlyouts.Uwp
{
    public sealed partial class VolumeFlyoutControl : UserControl
    {
        public ObservableCollection<MediaSession> MediaSessions { get; set; } = new ObservableCollection<MediaSession>();

        public VolumeFlyoutControl() => InitializeComponent();

        public async void AddSession(MediaSession session) => await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { MediaSessions.Add(session); });

        public async void RemoveSession(MediaSession session) => await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { MediaSessions.Remove(session); });

        public async void ShowFlyout() => await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { FlyoutBase.ShowAttachedFlyout(FlyoutGrid); });
    }
}