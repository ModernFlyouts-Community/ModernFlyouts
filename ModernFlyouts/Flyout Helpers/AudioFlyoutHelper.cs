using ModernFlyouts.Core.Media.Control;
using ModernFlyouts.Core.Utilities;
using ModernFlyouts.Helpers;
using ModernFlyouts.Utilities;
using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
        private List<MediaSessionManagerBase> mediaSessionManagers = new();
        private bool isVolumeFlyout = true;

        public override event ShowFlyoutEventHandler ShowFlyoutRequested;

        #region Properties

        public CompositeCollection AllMediaSessions { get; } = new();

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

            #region Volume control sub-module initialization

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

            #region Media Session sub-module initialization

            sessionsPanel = new SessionsPanel(AllMediaSessions);
            SecondaryContent = sessionsPanel;

            SetupMediaSessionManagers();

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
        }

        public void OnExternalUpdated(bool isMediaKey)
        {
            var mediaSessionsAvailable = AnyMediaSessionsAvailable();

            if ((!isMediaKey && device != null) || (isMediaKey && mediaSessionsAvailable))
            {
                isVolumeFlyout = !isMediaKey;

                if (isVolumeFlyout)
                {
                    PrimaryContentVisible = true;
                    SecondaryContentVisible = mediaSessionsAvailable && ShowGSMTCInVolumeFlyout;
                }
                else
                {
                    PrimaryContentVisible = ShowVolumeControlInGSMTCFlyout;
                    SecondaryContentVisible = mediaSessionsAvailable;
                }

                ShowFlyoutRequested?.Invoke(this);
            }
        }

        private void OnShowGSMTCInVolumeFlyoutChanged()
        {
            var mediaSessionsAvailable = AnyMediaSessionsAvailable();
            if (isVolumeFlyout)
            {
                SecondaryContentVisible = mediaSessionsAvailable && showGSMTCInVolumeFlyout;
            }
            else
            {
                SecondaryContentVisible = mediaSessionsAvailable;
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
                if (volume >= 66)
                    volumeControl.VolumeGlyph.Glyph = CommonGlyphs.Volume3;
                else if (volume < 1)
                    volumeControl.VolumeGlyph.Glyph = CommonGlyphs.Volume0;
                else if (volume < 33)
                    volumeControl.VolumeGlyph.Glyph = CommonGlyphs.Volume1;
                else if (volume < 66)
                    volumeControl.VolumeGlyph.Glyph = CommonGlyphs.Volume2;

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
                var value = e.NewValue;
                var oldValue = e.OldValue;

                if (value == oldValue)
                {
                    return;
                }

                if (oldValue != value && device != null)
                {
                    device.AudioEndpointVolume.MasterVolumeLevelScalar = (float)(value / 100);
                    e.Handled = true;
                }
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
            var change = e.Delta / 120.0;

            var volume = Math.Min(Math.Max(slider.Value + change, 0.0), 100.0);

            if (device != null)
            {
                device.AudioEndpointVolume.MasterVolumeLevelScalar = (float)(volume / 100.0);
                e.Handled = true;
            }
        }

        #endregion

        #region Media Control

        private void SetupMediaSessionManagers()
        {
            var gsmtcMediaSessionManager = new GSMTCMediaSessionManager();

            AllMediaSessions.Add(new CollectionContainer()
            {
                Collection = gsmtcMediaSessionManager.MediaSessions
            });
            mediaSessionManagers.Add(gsmtcMediaSessionManager);
        }

        private bool AnyMediaSessionsAvailable() => mediaSessionManagers.Any(x => x.ContainsAnySession());

        #endregion

        #region Media Session Fallback Thumbnails

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

            foreach (var mediaSessionManager in mediaSessionManagers)
            {
                mediaSessionManager.OnEnabled();
            }

            System.Diagnostics.Debug.WriteLine("Media Sessions: " + AllMediaSessions.Count);
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

            foreach (var mediaSessionManager in mediaSessionManagers)
            {
                mediaSessionManager.OnEnabled();
            }

            AppDataHelper.AudioModuleEnabled = IsEnabled;
        }
    }
}
