using ModernFlyouts.Helpers;
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
    public class AudioFlyoutHelper : FlyoutHelperBase
    {
        private AudioDeviceNotificationClient client;
        private MMDeviceEnumerator enumerator;
        private MMDevice device;
        private VolumeControl volumeControl;
        private SessionsPanel sessionsPanel;
        private TextBlock noDeviceMessageBlock;
        private bool _isinit;
        private bool _SMTCAvail;
        private bool isVolumeFlyout = true;

        public override event ShowFlyoutEventHandler ShowFlyoutRequested;

        #region Properties

        private bool showGSMTCInVolumeFlyout = true;

        public bool ShowGSMTCInVolumeFlyout
        {
            get => showGSMTCInVolumeFlyout;
            set
            {
                if (SetProperty(ref showGSMTCInVolumeFlyout, value))
                {
                    OnShowGSMTCInVolumeFlyoutChanged();
                }
            }
        }

        private bool showVolumeControlInGSMTCFlyout = true;

        public bool ShowVolumeControlInGSMTCFlyout
        {
            get => showVolumeControlInGSMTCFlyout;
            set
            {
                if (SetProperty(ref showVolumeControlInGSMTCFlyout, value))
                {
                    OnShowVolumeControlInGSMTCFlyoutChanged();
                }
            }
        }

        #endregion

        public AudioFlyoutHelper()
        {
           Initialize();
        }

        public void Initialize()
        {
            AlwaysHandleDefaultFlyout = true;

            ShowGSMTCInVolumeFlyout = AppDataHelper.ShowGSMTCInVolumeFlyout;
            ShowVolumeControlInGSMTCFlyout = AppDataHelper.ShowVolumeControlInGSMTCFlyout;

            #region Creating Volume Control

            volumeControl = new VolumeControl();
            volumeControl.VolumeButton.Click += VolumeButton_Click;
            volumeControl.VolumeSlider.ValueChanged += VolumeSlider_ValueChanged;
            volumeControl.VolumeSlider.PreviewMouseWheel += VolumeSlider_PreviewMouseWheel;

            noDeviceMessageBlock = new TextBlock()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 18.0,
                Margin = new Thickness(20),
                Text = Properties.Strings.AudioFlyoutHelper_NoDevices
            };
            noDeviceMessageBlock.SetResourceReference(TextBlock.StyleProperty, "BaseTextBlockStyle");

            #endregion

            #region Creating Session Controls

            sessionsPanel = new SessionsPanel();
            SecondaryContent = sessionsPanel;

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

        public void OnExternalUpdated(bool isMediaKey)
        {
            if ((!isMediaKey && device != null) || (isMediaKey && _SMTCAvail))
            {
                isVolumeFlyout = !isMediaKey;

                if (isVolumeFlyout)
                {
                    PrimaryContentVisible = true;
                    SecondaryContentVisible = _SMTCAvail && ShowGSMTCInVolumeFlyout;
                }
                else
                {
                    PrimaryContentVisible = ShowVolumeControlInGSMTCFlyout;
                    SecondaryContentVisible = _SMTCAvail;
                }

                ShowFlyoutRequested?.Invoke(this);
            }
        }

        private void OnShowGSMTCInVolumeFlyoutChanged()
        {
            if (isVolumeFlyout)
            {
                SecondaryContentVisible = _SMTCAvail && showGSMTCInVolumeFlyout;
            }
            else
            {
                SecondaryContentVisible = _SMTCAvail;
            }

            AppDataHelper.ShowGSMTCInVolumeFlyout = showGSMTCInVolumeFlyout;
        }

        private void OnShowVolumeControlInGSMTCFlyoutChanged()
        {
            if (isVolumeFlyout)
            {
                PrimaryContentVisible = true;
            }
            else
            {
                PrimaryContentVisible = showVolumeControlInGSMTCFlyout;
            }

            AppDataHelper.ShowVolumeControlInGSMTCFlyout = showVolumeControlInGSMTCFlyout;
        }


        #region Volume

        private void Client_DefaultDeviceChanged(object sender, string e)
        {
            MMDevice mmdevice = string.IsNullOrEmpty(e) ? null : enumerator.GetDevice(e);
            UpdateDevice(mmdevice);
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
                App.Current.Dispatcher.Invoke(() => PrimaryContent = volumeControl);
            }
            else { App.Current.Dispatcher.Invoke(() => PrimaryContent = noDeviceMessageBlock); }
        }

        private void AudioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData data)
        {
            UpdateVolume(data.MasterVolume * 100);
        }

        private bool _isInCodeValueChange; //Prevents a LOOP between changing volume.

        private void UpdateVolume(double volume)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                UpdateVolumeGlyph(volume);
                volumeControl.textVal.Text = Math.Round(volume).ToString("00");
                _isInCodeValueChange = true;
                volumeControl.VolumeSlider.Value = volume;
                _isInCodeValueChange = false;
            });
        }

        private void UpdateVolumeGlyph(double volume)
        {
            if (device != null && !device.AudioEndpointVolume.Mute)
            {
                volumeControl.VolumeShadowGlyph.Visibility = Visibility.Visible;
                if (volume >= 50)
                    volumeControl.VolumeGlyph.Glyph = CommonGlyphs.Volume2;
                else if (volume < 1)
                    volumeControl.VolumeGlyph.Glyph = CommonGlyphs.Volume0;
                else if (volume < 50)
                    volumeControl.VolumeGlyph.Glyph = CommonGlyphs.Volume1;

                volumeControl.textVal.ClearValue(TextBlock.ForegroundProperty);
                volumeControl.VolumeSlider.IsEnabled = true;
            }
            else
            {
                volumeControl.textVal.SetResourceReference(TextBlock.ForegroundProperty, "SystemControlForegroundBaseMediumBrush");
                volumeControl.VolumeSlider.IsEnabled = false;
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

            var volume = value + change;

            if (volume > 100 || volume < 0)
            {
                return;
            }


            if (device != null)
            {
                device.AudioEndpointVolume.MasterVolumeLevelScalar = (float)(volume / 100);
                device.AudioEndpointVolume.Mute = false;
            }

            e.Handled = true;
        }

        #endregion

        #region SMTC

        private GlobalSystemMediaTransportControlsSessionManager GSMTC;

        [MethodImpl(MethodImplOptions.NoInlining)]
        private async void SetupSMTCAsync()
        {
            if (!IsEnabled)
            {
                return;
            }

            GSMTC = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
            GSMTC.SessionsChanged += SMTC_SessionsChanged;

            LoadSessionControls();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void DetachSMTC()
        {
            if (GSMTC != null)
            {
                GSMTC.SessionsChanged -= SMTC_SessionsChanged;
                GSMTC = null;
            }

            ClearSessionControls();
            SecondaryContentVisible = false;
        }

        private void ClearSessionControls()
        {
            foreach (var child in sessionsPanel.SessionsStackPanel.Children)
            {
                var s = child as SessionControl;
                s?.DisposeSession();
            }

            sessionsPanel.SessionsStackPanel.Children.Clear();
            _SMTCAvail = false;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void SMTC_SessionsChanged(GlobalSystemMediaTransportControlsSessionManager sender, SessionsChangedEventArgs args)
        {
            App.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(LoadSessionControls));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void LoadSessionControls()
        {
            ClearSessionControls();

            if (!IsEnabled)
            {
                return;
            }

            if (GSMTC != null)
            {
                var sessions = GSMTC.GetSessions();

                foreach (var session in sessions)
                {
                    sessionsPanel.SessionsStackPanel.Children.Add(new SessionControl(session));
                }

                if (sessionsPanel.SessionsStackPanel.Children.Count > 0)
                {
                    SecondaryContentVisible = !isVolumeFlyout || ShowGSMTCInVolumeFlyout;
                    _SMTCAvail = true;
                }
            }
        }

        #endregion

        #region SMTC Thumbnails

        public static ImageSource GetDefaultAudioThumbnail() => new BitmapImage(PackUriHelper.GetAbsoluteUri("Assets/Images/DefaultAudioThumbnail.png"));

        public static ImageSource GetDefaultImageThumbnail() => new BitmapImage(PackUriHelper.GetAbsoluteUri("Assets/Images/DefaultImageThumbnail.png"));

        public static ImageSource GetDefaultVideoThumbnail() => new BitmapImage(PackUriHelper.GetAbsoluteUri("Assets/Images/DefaultVideoThumbnail.png"));

        #endregion

        protected override void OnEnabled()
        {
            base.OnEnabled();

            AppDataHelper.AudioModuleEnabled = IsEnabled;

            if (!IsEnabled)
            {
                return;
            }

            client.DefaultDeviceChanged += Client_DefaultDeviceChanged;

            if (device != null)
            {
                device.AudioEndpointVolume.OnVolumeNotification += AudioEndpointVolume_OnVolumeNotification;
                PrimaryContent = volumeControl;
            }
            else { PrimaryContent = noDeviceMessageBlock; }

            PrimaryContentVisible = isVolumeFlyout || ShowVolumeControlInGSMTCFlyout;

            if (_isinit)
            {
                try { SetupSMTCAsync(); } catch { }
            }
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();

            client.DefaultDeviceChanged -= Client_DefaultDeviceChanged;

            if (device != null)
            {
                device.AudioEndpointVolume.OnVolumeNotification -= AudioEndpointVolume_OnVolumeNotification;
            }
            PrimaryContent = null;
            PrimaryContentVisible = false;

            try { DetachSMTC(); } catch { }

            AppDataHelper.AudioModuleEnabled = IsEnabled;
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