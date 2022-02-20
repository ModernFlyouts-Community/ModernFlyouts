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

namespace ModernFlyouts.Wpf
{
    public partial class VolumeTrayWindow : Window
    {
        private VolumeTrayControl VolumeTraycontrol;
        private static NowPlayingSessionManager NPSessionManager;

        public VolumeTrayWindow() => InitializeComponent();

        private async void OnFlyoutControlLoaded(object sender, EventArgs e)
        {
            VolumeTraycontrol = (sender as WindowsXamlHost).Child as VolumeTrayControl;
            NPSessionManager = new NowPlayingSessionManager();
            VolumeTraycontrol.VolumeSlider.Value = (int)AudioService.GetMasterVolume();
            VolumeTraycontrol.VolumeSlider.ValueChanged += VolumeSlider_ValueChanged;
            VolumeTraycontrol.VolumeButton.Click += VolumeButton_Click;
            VolumeTraycontrol.ToggleMuteIcon(AudioService.GetMasterVolumeMute());
            DispatcherTimer VolumeListener = new DispatcherTimer();
            VolumeListener.Tick += VolumeListener_Tick;
            VolumeListener.Interval = new TimeSpan(0, 0, 0, 0, 100);
            VolumeListener.Start();
            ///   NPSessionManager.SessionListChanged += NPSessionManager_SessionListChanged;
            foreach (var i in NPSessionManager.GetSessions())
            {
                // SessionConstructor should work here but it doesnt, maybe it an async problem but it should replace this
                using var process = Process.GetProcessById((int)i.PID);
                if (IsImmersiveProcess(process.Handle))
                {
                ///    VolumeTraycontrol.MediaSessions.Add(await ModernAppInfoExtractor.GetModernAppInfo(i));
                }
                else
                {
                 ///   VolumeTraycontrol.MediaSessions.Add(DesktopAppInfoExtractor.GetDesktopAppInfo(i));
                }
            }
        }
        bool IsUpdatingTEST = false;
        private void VolumeListener_Tick(object sender, EventArgs e)
        {
            if (IsUpdatingTEST == false)
            {
                IsUpdatingTEST = true;
                if ((int)AudioService.GetMasterVolume() != (int)VolumeTraycontrol.VolumeSlider.Value)
                {
                    VolumeTraycontrol.VolumeSlider.Value = (int)AudioService.GetMasterVolume();
                    VolumeTraycontrol.ToggleMuteIcon(AudioService.GetMasterVolumeMute());
                }
                IsUpdatingTEST = false;
            }
        }

        private void VolumeButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            AudioService.SetMasterVolumeMute(!AudioService.GetMasterVolumeMute());
            VolumeTraycontrol.ToggleMuteIcon(AudioService.GetMasterVolumeMute());
        }

        private void VolumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e) => UpdateVolume(e.NewValue);

        public void UpdateVolume(double newVolume)
        {
            if (IsUpdatingTEST == false)
            {
                if (newVolume == 0)
                {
                    VolumeTraycontrol.ToggleMuteIcon(true);
                    AudioService.SetMasterVolume((float)newVolume);
                }
                else
                {
                    VolumeTraycontrol.ToggleMuteIcon(false);
                    AudioService.SetMasterVolume((float)newVolume);
                }
            }
        }

        private async void NPSessionManager_SessionListChanged(object sender, NowPlayingSessionManagerEventArgs e)
        {
            if (e.NotificationType == NowPlayingSessionManagerNotificationType.SessionCreated)
            {
                // Same here should be replaced
                var session = NPSessionManager.FindSession(e.NowPlayingSessionInfo);
                using var process = Process.GetProcessById((int)session.PID);
                if (IsImmersiveProcess(process.Handle))
                {
                    VolumeTraycontrol.AddSession(await ModernAppInfoExtractor.GetModernAppInfo(session));
                }
                else
                {
                    VolumeTraycontrol.AddSession(DesktopAppInfoExtractor.GetDesktopAppInfo(session));
                }
            }
            else if (e.NotificationType == NowPlayingSessionManagerNotificationType.SessionDisconnected)
            {
                 var mediaSession = VolumeTraycontrol.MediaSessions.Cast<MediaSession>().FirstOrDefault(x => x.MediaPlayingSession.GetSessionInfo().Equals(e.NowPlayingSessionInfo));
                 VolumeTraycontrol.RemoveSession(mediaSession);
            }

        }
        [DllImport("user32.dll")]
        public static extern bool IsImmersiveProcess(IntPtr hProcess);
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
            if (VolumeTraycontrol == null) return;
            Activate();
            VolumeTraycontrol.ShowFlyout();
        }
        private void DragWindow(object sender, MouseButtonEventArgs e) => DragMove();

        #endregion
    }
}