using ModernFlyouts.Utilities;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Windows.Media.Control;

namespace ModernFlyouts
{
    public class AudioHelper : HelperBase
    {
        private AudioDeviceNotificationClient client;
        private MMDeviceEnumerator enumerator;
        private MMDevice device;
        private VolumeControl volumeControl;
        private StackPanel SessionsStackPanel;
        private bool _isinit = false;

        public override event ShowFlyoutEventHandler ShowFlyoutRequested;

        public AudioHelper()
        {
           Initialize();
        }

        public void Initialize()
        {
            AlwaysHandleDefaultFlyout = true;

            #region Creating Volume Control

            volumeControl = new VolumeControl();
            volumeControl.VolumeButton.Click += VolumeButton_Click;
            volumeControl.VolumeSlider.ValueChanged += VolumeSlider_ValueChanged;
            volumeControl.VolumeSlider.PreviewMouseWheel += VolumeSlider_PreviewMouseWheel;

            #endregion

            #region Creating Session Controls

            SessionsStackPanel = new StackPanel();
            SecondaryContent = SessionsStackPanel;

            try { SetupSMTCAsync(); } catch { }

            #endregion
            
            PrimaryContent = volumeControl;
            client = new AudioDeviceNotificationClient();

            enumerator = new MMDeviceEnumerator();
            enumerator.RegisterEndpointNotificationCallback(client);

            if (enumerator.HasDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia))
            {
                UpdateDevice(enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia));
            }

            OnEnabled();
            _isinit = true;
        }

        private void KeyPressed(Key Key, int virtualKey)
        {
            if (Key == Key.VolumeUp || Key == Key.VolumeDown || Key == Key.VolumeMute)
            {
                ShowFlyout();
            }
            if ((Key == Key.MediaNextTrack || Key == Key.MediaPreviousTrack || Key == Key.MediaPlayPause || Key == Key.MediaStop) && SMTCAvail())
            {
                ShowFlyout();
            }

            void ShowFlyout()
            {
                ShowFlyoutRequested?.Invoke(this);
            }

            bool SMTCAvail()
            {
                return SessionsStackPanel.Children.Count > 0;
            }
        }

        #region Volume

        private void Client_DefaultDeviceChanged(object sender, string e)
        {
            if (e != null)
            {
                UpdateDevice(enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia));
            }
            else
            {
                UpdateDevice(null);
            }
        }

        private void UpdateDevice(MMDevice mmdevice)
        {
            if (device != null)
            {
                device.AudioEndpointVolume.OnVolumeNotification -= AudioEndpointVolume_OnVolumeNotification;
            }

            device = mmdevice;
            if (device != null)
            {
                UpdateVolume(device.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
                device.AudioEndpointVolume.OnVolumeNotification += AudioEndpointVolume_OnVolumeNotification;
                PrimaryContentVisible = true;
            }
            else { PrimaryContentVisible = false; }
        }

        private void AudioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData data)
        {
            UpdateVolume(data.MasterVolume * 100);
        }

        private bool _isInCodeValueChange = false; //Prevents a LOOP between changing volume.

        private void UpdateVolume(double volume)
        {
            volumeControl.Dispatcher.Invoke(() =>
            {
                UpdateVolumeGlyph(volume);
                _isInCodeValueChange = true;
                volumeControl.VolumeSlider.Value = Math.Round(volume);
                _isInCodeValueChange = false;
                volumeControl.textVal.Text = Math.Round(volume).ToString("00");
            });
        }

        private void UpdateVolumeGlyph(double volume)
        {
            if (device != null && !device.AudioEndpointVolume.Mute)
            {
                volumeControl.VolumeShadowGlyph.Visibility = Visibility.Visible;
                if (volume >= 66)
                    volumeControl.VolumeGlyph.Glyph = CommonGlyphs.Volume3;
                else if (volume < 1)
                    volumeControl.VolumeGlyph.Glyph = CommonGlyphs.Volume0;
                else if (volume < 33)
                    volumeControl.VolumeGlyph.Glyph = CommonGlyphs.Volume1;
                else if (volume < 66)
                    volumeControl.VolumeGlyph.Glyph = CommonGlyphs.Volume2;
            }
            else
            {
                volumeControl.VolumeShadowGlyph.Visibility = Visibility.Collapsed;
                volumeControl.VolumeGlyph.Glyph = CommonGlyphs.Mute;
            }
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_isInCodeValueChange)
            {
                var value = Math.Truncate(e.NewValue);
                var oldValue = Math.Truncate(e.OldValue);

                if (value == oldValue)
                {
                    return;
                }

                if (device != null)
                {
                    device.AudioEndpointVolume.MasterVolumeLevelScalar = (float)(value / 100);
                    device.AudioEndpointVolume.Mute = false;
                }

                e.Handled = true;
            }
        }

        private void VolumeButton_Click(object sender, RoutedEventArgs e)
        {
            if (device != null)
            {
                device.AudioEndpointVolume.Mute = !device.AudioEndpointVolume.Mute;
            }
        }

        private void VolumeSlider_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var slider = sender as Slider;
            var value = Math.Truncate(slider.Value);
            var change = e.Delta / 120;

            if (value + change > 100 || value + change < 0)
            {
                return;
            }


            if (device != null)
            {
                device.AudioEndpointVolume.MasterVolumeLevelScalar = (float)((value + change) / 100);
                device.AudioEndpointVolume.Mute = false;
            }

            e.Handled = true;
        }

        #endregion

        #region SMTC

        private object _SMTC;

        [MethodImpl(MethodImplOptions.NoInlining)]
        private async void SetupSMTCAsync()
        {
            GlobalSystemMediaTransportControlsSessionManager SMTC;

            SMTC = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
            SMTC.SessionsChanged += SMTC_SessionsChanged;
            _SMTC = SMTC;

            LoadSessionControls();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void DetachSMTC()
        {
            if (_SMTC is GlobalSystemMediaTransportControlsSessionManager SMTC)
            {
                SMTC.SessionsChanged -= SMTC_SessionsChanged;
                _SMTC = null;
            }

            ClearSessionControls();
            SecondaryContentVisible = false;
        }

        private void ClearSessionControls()
        {
            foreach (var child in SessionsStackPanel.Children)
            {
                var s = child as SessionControl;
                s.DisposeSession();
            }

            SessionsStackPanel.Children.Clear();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void SMTC_SessionsChanged(GlobalSystemMediaTransportControlsSessionManager sender, SessionsChangedEventArgs args)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(LoadSessionControls));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void LoadSessionControls()
        {
            ClearSessionControls();

            if (!IsEnabled)
            {
                return;
            }

            if (_SMTC is GlobalSystemMediaTransportControlsSessionManager SMTC)
            {
                var sessions = SMTC.GetSessions();

                foreach (var session in sessions)
                {
                    SessionsStackPanel.Children.Add(new SessionControl
                    {
                        SMTCSession = session,
                        Margin = new Thickness(0, 2, 0, 0)
                    });
                }

                if (SessionsStackPanel.Children.Count > 0)
                {
                    (SessionsStackPanel.Children[0] as SessionControl).Margin = new Thickness(0);
                    SecondaryContentVisible = true;
                }
            }
        }
        #endregion

        #region SMTC Thumbnails

        public static ImageSource GetDefaultAudioThumbnail()
        {
            return new BitmapImage(PackUriHelper.GetAbsoluteUri("Assets/Images/DefaultAudioThumbnail.png"));
        }

        public static ImageSource GetDefaultImageThumbnail()
        {
            return new BitmapImage(PackUriHelper.GetAbsoluteUri("Assets/Images/DefaultImageThumbnail.png"));
        }

        public static ImageSource GetDefaultVideoThumbnail()
        {
            return new BitmapImage(PackUriHelper.GetAbsoluteUri("Assets/Images/DefaultVideoThumbnail.png"));
        }

        #endregion

        protected override void OnEnabled()
        {
            base.OnEnabled();

            Properties.Settings.Default.AudioModuleEnabled = IsEnabled;
            Properties.Settings.Default.Save();

            if (!IsEnabled)
            {
                return;
            }

            FlyoutHandler.Instance.KeyboardHook.KeyDown += KeyPressed;
            client.DefaultDeviceChanged += Client_DefaultDeviceChanged;

            if (device != null)
            {
                device.AudioEndpointVolume.OnVolumeNotification += AudioEndpointVolume_OnVolumeNotification;
                PrimaryContentVisible = true;
            }
            else { PrimaryContentVisible = false; }

            if (_isinit)
            {
                try { SetupSMTCAsync(); } catch { }
            }
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();

            FlyoutHandler.Instance.KeyboardHook.KeyDown -= KeyPressed;
            client.DefaultDeviceChanged -= Client_DefaultDeviceChanged;

            if (device != null)
            {
                device.AudioEndpointVolume.OnVolumeNotification -= AudioEndpointVolume_OnVolumeNotification;
            }
            PrimaryContentVisible = false;

            try { DetachSMTC(); } catch { }

            Properties.Settings.Default.AudioModuleEnabled = IsEnabled;
            Properties.Settings.Default.Save();
        }
    }

    public class AudioDeviceNotificationClient : IMMNotificationClient
    {
        public event EventHandler<string> DefaultDeviceChanged;

        public void OnDefaultDeviceChanged(DataFlow dataFlow, Role deviceRole, string defaultDeviceId)
        {
            if (dataFlow == DataFlow.Render && deviceRole == Role.Multimedia)
            {
                DefaultDeviceChanged?.Invoke(this, defaultDeviceId);
            }
        }

        public void OnDeviceAdded(string deviceId)
        { }

        public void OnDeviceRemoved(string deviceId)
        { }

        public void OnDeviceStateChanged(string deviceId, DeviceState newState)
        { }

        public void OnPropertyValueChanged(string deviceId, PropertyKey propertyKey)
        { }
    }
}