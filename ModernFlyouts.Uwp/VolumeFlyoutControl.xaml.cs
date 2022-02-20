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
    public sealed partial class VolumeFlyoutControl : UserControl
    {
        public ObservableCollection<MediaSession> MediaSessions { get; set; } = new ObservableCollection<MediaSession>();

        public VolumeFlyoutControl() => InitializeComponent();

        public async void AddSession(MediaSession session) => await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { MediaSessions.Add(session); });

        public async void RemoveSession(MediaSession session) => await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { MediaSessions.Remove(session); });

        public async void ShowFlyout() => await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { FlyoutBase.ShowAttachedFlyout(FlyoutGrid); });

        public void ToggleMuteIcon(bool isMuted) => VolumeIcon.Symbol = isMuted ? FluentSymbol.SpeakerMute20 : VolumeToIcon(VolumeSlider.Value);

        public FluentSymbol VolumeToIcon(double volume)
        {
            return (volume) switch
            {
                double i when i > 0 && i <= 50 => FluentSymbol.Speaker120,
                _ => FluentSymbol.Speaker220
            };
        }
    }
}