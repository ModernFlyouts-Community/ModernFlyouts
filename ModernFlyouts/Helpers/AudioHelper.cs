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

        public override event ShowFlyoutEventHandler ShowFlyoutRequested;

        public AudioHelper()
        {
           if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
           {
                volumeControl = new VolumeControl();
                PrimaryContent = volumeControl;

                SecondaryContent = new Border() { Background = Brushes.Blue, CornerRadius = new CornerRadius(10), Height = 40 };
                SecondaryContentVisible = true;
           } else { Initialize(); }
        }

        public void Initialize()
        {
            FlyoutHandler.Instance.KeyboardHook.KeyDown += KeyPressed;

            #region Creating Volume Control

            volumeControl = new VolumeControl();
            volumeControl.VolumeButton.Click += VolumeButton_Click;
            volumeControl.VolumeSlider.ValueChanged += VolumeSlider_ValueChanged;
            volumeControl.VolumeSlider.PreviewMouseWheel += VolumeSlider_PreviewMouseWheel;

            #endregion

            #region Creating Session Controls

            try { SetupSMTCAsync(); } catch { }

            #endregion
            
            PrimaryContent = volumeControl;
            client = new AudioDeviceNotificationClient();
            client.DefaultDeviceChanged += Client_DefaultDeviceChanged;

            enumerator = new MMDeviceEnumerator();
            enumerator.RegisterEndpointNotificationCallback(client);

            if (enumerator.HasDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia))
                UpdateDevice(enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia));
        }

        private void KeyPressed(Key Key)
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
                ShowFlyoutRequested?.Invoke(this, true);
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
                UpdateDevice(enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia));
            else
                UpdateDevice(null);
        }

        private void UpdateDevice(MMDevice mmdevice)
        {
            device = mmdevice;
            if (device != null)
            {
                UpdateVolume(device.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
                device.AudioEndpointVolume.OnVolumeNotification += (data) => UpdateVolume(data.MasterVolume * 100);
                PrimaryContentVisible = true;
            }
            else { PrimaryContentVisible = false; }
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
                    return;

                if (device != null)
                {
                    device.AudioEndpointVolume.MasterVolumeLevelScalar = (float)(value / 100);

                    if (device.AudioEndpointVolume.Mute)
                    {
                        device.AudioEndpointVolume.Mute = false;
                    }
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
            var change = (e.Delta / 120);

            if (value + change > 100 || value + change < 0)
                return;


            if (device != null)
            {
                device.AudioEndpointVolume.MasterVolumeLevelScalar = (float)((value + change) / 100);

                if (device.AudioEndpointVolume.Mute)
                {
                    device.AudioEndpointVolume.Mute = false;
                }
            }

            e.Handled = true;
        }

        #endregion

        #region SMTC

        [MethodImpl(MethodImplOptions.NoInlining)]
        private async void SetupSMTCAsync()
        {
            SessionsStackPanel = new StackPanel();
            SecondaryContent = SessionsStackPanel;
            SecondaryContentVisible = true;

            GlobalSystemMediaTransportControlsSessionManager SMTC;

            SMTC = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();

            LoadSessionControls();

            SMTC.SessionsChanged += (_, __) => Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(LoadSessionControls));

            void LoadSessionControls()
            {
                SessionsStackPanel.Children.Clear();

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
                    SessionsStackPanel.Margin = new Thickness(0, 2, 0, 0);
                    (SessionsStackPanel.Children[0] as SessionControl).Margin = new Thickness(0);
                }
                else { SessionsStackPanel.Margin = new Thickness(0); }
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