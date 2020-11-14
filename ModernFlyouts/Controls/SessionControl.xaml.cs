using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Windows.Media;
using Windows.Media.Control;
using Windows.Storage.Streams;
using ModernFlyouts.Utilities;
using ModernWpf.Input;
using ModernWpf.Media.Animation;

namespace ModernFlyouts.Controls
{
    public partial class SessionControl : UserControl
    {
        private SourceAppInfo sourceAppInfo;

        #region Properties

        private GlobalSystemMediaTransportControlsSession _SMTCSession;

        public GlobalSystemMediaTransportControlsSession SMTCSession
        {
            get => _SMTCSession;
            set
            {
                if (value != null)
                {
                    sourceAppInfo = SourceAppInfo.FromAppId(value.SourceAppUserModelId);
                    sourceAppInfo.InfoFetched += SourceAppInfo_InfoFetched;
                    UpdateSessionInfo(value);
                    value.MediaPropertiesChanged += Session_MediaPropertiesChanged;
                    value.PlaybackInfoChanged += Session_PlaybackInfoChanged;
                    value.TimelinePropertiesChanged += Session_TimelinePropertiesChanged;
                    _SMTCSession = value;
                }
                else
                {
                    DisposeSession();
                }
            }
        }

        public static readonly DependencyProperty AlignThumbnailToRightProperty =
            DependencyProperty.Register(
                nameof(AlignThumbnailToRight),
                typeof(bool),
                typeof(SessionControl),
                new PropertyMetadata(true, OnAlignThumbnailToRightChanged));

        public bool AlignThumbnailToRight
        {
            get => (bool)GetValue(AlignThumbnailToRightProperty);
            set => SetValue(AlignThumbnailToRightProperty, value);
        }

        #endregion Properties

        public SessionControl()
        {
            InitializeComponent();
            MediaTitleBlock.Text = "";
            MediaArtistBlock.Text = "";
            PreviousButton.Click += (_, __) => PreviousTrack();
            PlayPauseButton.Click += (_, __) => PlayOrPause();
            NextButton.Click += (_, __) => NextTrack();
            ShuffleButton.Click += (_, __) => ShuffleTrack();
            RepeatButton.Click += (_, __) => ChangeRepeatMode();
            StopButton.Click += (_, __) => StopTrack();

            Loaded += SessionControl_Loaded;
            Unloaded += SessionControl_Unloaded;

            AlignThumbnailToRight = FlyoutHandler.Instance.UIManager.AlignGSMTCThumbnailToRight;
            BindingOperations.SetBinding(this, AlignThumbnailToRightProperty,
                new Binding(nameof(UI.UIManager.AlignGSMTCThumbnailToRight)) { Source = FlyoutHandler.Instance.UIManager });
        }

        public SessionControl(GlobalSystemMediaTransportControlsSession session) : this()
        {
            SMTCSession = session;
        }

        private void SourceAppInfo_InfoFetched(object sender, EventArgs e)
        {
            sourceAppInfo.InfoFetched -= SourceAppInfo_InfoFetched;
            AppNameBlock.Text = sourceAppInfo.AppName;
            AppImage.Source = sourceAppInfo.AppImage;
        }

        private void SessionControl_Loaded(object sender, RoutedEventArgs e)
        {
            InputHelper.SetIsTapEnabled(TextBlockGrid, true);
            InputHelper.AddTappedHandler(TextBlockGrid, UIElement_Tapped);
            InputHelper.SetIsTapEnabled(ThumbnailGrid, true);
            InputHelper.AddTappedHandler(ThumbnailGrid, UIElement_Tapped);
            InputHelper.SetIsTapEnabled(AppInfoPanel, true);
            InputHelper.AddTappedHandler(AppInfoPanel, UIElement_Tapped);

            FlyoutHandler.Instance.FlyoutWindow.FlyoutTimedHiding += FlyoutWindow_FlyoutTimedHiding;
            FlyoutHandler.Instance.FlyoutWindow.FlyoutHidden += FlyoutWindow_FlyoutHidden;
        }

        private void FlyoutWindow_FlyoutTimedHiding(object sender, RoutedEventArgs e)
        {
            if (TimelineInfoFlyout.IsOpen)
            {
                e.Handled = true;
            }
        }

        private void FlyoutWindow_FlyoutHidden(object sender, RoutedEventArgs e)
        {
            TimelineInfoFlyout.Hide();
        }

        private void SessionControl_Unloaded(object sender, RoutedEventArgs e)
        {
            TimelineInfoFlyout.Hide();

            InputHelper.RemoveTappedHandler(TextBlockGrid, UIElement_Tapped);
            InputHelper.SetIsTapEnabled(TextBlockGrid, false);
            InputHelper.RemoveTappedHandler(ThumbnailGrid, UIElement_Tapped);
            InputHelper.SetIsTapEnabled(ThumbnailGrid, false);
            InputHelper.RemoveTappedHandler(AppInfoPanel, UIElement_Tapped);
            InputHelper.SetIsTapEnabled(AppInfoPanel, false);

            FlyoutHandler.Instance.FlyoutWindow.FlyoutTimedHiding -= FlyoutWindow_FlyoutTimedHiding;
            FlyoutHandler.Instance.FlyoutWindow.FlyoutHidden -= FlyoutWindow_FlyoutHidden;
        }

        private void UIElement_Tapped(object sender, TappedRoutedEventArgs e)
        {
            OpenSourceApp();
        }

        private void OpenSourceApp()
        {
            sourceAppInfo?.Activate();
        }

        private async void Session_PlaybackInfoChanged(GlobalSystemMediaTransportControlsSession session, PlaybackInfoChangedEventArgs args)
        {
            await Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                if (session != null && session.GetPlaybackInfo() != null)
                {
                    UpdatePlaybackInfo(session);
                }
            }));
        }

        private async void Session_MediaPropertiesChanged(GlobalSystemMediaTransportControlsSession session, MediaPropertiesChangedEventArgs args)
        {
            await Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
            {
                if (session != null && session.GetPlaybackInfo() != null)
                {
                    UpdateSessionInfo(session);
                }
            }));
        }

        private async void Session_TimelinePropertiesChanged(GlobalSystemMediaTransportControlsSession session, TimelinePropertiesChangedEventArgs args)
        {
            await Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                if (session != null && session.GetTimelineProperties() != null)
                {
                    UpdateTimelineInfo(session);
                }
            }));
        }

        #region Session Methods

        private async void PreviousTrack()
        {
            try
            {
                if (_SMTCSession != null)
                {
                    await _SMTCSession.TrySkipPreviousAsync();
                }
            }
            catch { }
        }

        private async void PlayOrPause()
        {
            try
            {
                if (_SMTCSession != null)
                {
                    var playback = _SMTCSession.GetPlaybackInfo();
                    if (playback != null)
                    {
                        if (playback.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing)
                        {
                            await _SMTCSession.TryPauseAsync();
                        }
                        else if (playback.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Paused
                            || playback.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Stopped
                            || playback.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Opened)
                        {
                            await _SMTCSession.TryPlayAsync();
                        }
                    }
                }
            }
            catch { }
        }

        private async void NextTrack()
        {
            try
            {
                if (_SMTCSession != null)
                {
                    await _SMTCSession.TrySkipNextAsync();
                }
            }
            catch { }
        }

        private async void ShuffleTrack()
        {
            try
            {
                if (_SMTCSession != null)
                {
                    var playback = _SMTCSession.GetPlaybackInfo();
                    if (playback != null)
                    {
                        if (playback.IsShuffleActive.HasValue)
                        {
                            if (playback.IsShuffleActive.Value)
                            {
                                await _SMTCSession.TryChangeShuffleActiveAsync(false);
                            }
                            else
                            {
                                await _SMTCSession.TryChangeShuffleActiveAsync(true);
                            }
                        }
                    }
                }
            }
            catch { }
        }

        private async void ChangeRepeatMode()
        {
            try
            {
                if (_SMTCSession != null)
                {
                    var playback = _SMTCSession.GetPlaybackInfo();
                    if (playback != null)
                    {
                        if (playback.AutoRepeatMode == MediaPlaybackAutoRepeatMode.None)
                        {
                            await _SMTCSession.TryChangeAutoRepeatModeAsync(MediaPlaybackAutoRepeatMode.List);
                        }
                        else if (playback.AutoRepeatMode == MediaPlaybackAutoRepeatMode.List)
                        {
                            await _SMTCSession.TryChangeAutoRepeatModeAsync(MediaPlaybackAutoRepeatMode.Track);
                        }
                        else if (playback.AutoRepeatMode == MediaPlaybackAutoRepeatMode.Track)
                        {
                            await _SMTCSession.TryChangeAutoRepeatModeAsync(MediaPlaybackAutoRepeatMode.None);
                        }
                    }
                }
            }
            catch { }
        }

        private async void StopTrack()
        {
            try
            {
                if (_SMTCSession != null)
                {
                    await _SMTCSession.TryStopAsync();
                }
            }
            catch { }
        }

        #endregion Session Methods

        #region Updating View

        private void UpdatePlaybackInfo(GlobalSystemMediaTransportControlsSession session)
        {
            UpdatePlayPauseButtonIcon(session);
            UpdateShuffleButton(session);
            UpdateRepeatButton(session);
        }

        private void UpdatePlayPauseButtonIcon(GlobalSystemMediaTransportControlsSession session)
        {
            try
            {
                if (session != null)
                {
                    var playback = session.GetPlaybackInfo();
                    if (playback != null)
                    {
                        if (playback.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing)
                        {
                            PlayPauseIcon.Glyph = CommonGlyphs.Pause;
                            PlayPauseButton.ToolTip = Properties.Strings.SessionControl_Pause;
                        }
                        else if (playback.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Paused
                            || playback.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Stopped
                            || playback.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Opened)
                        {
                            PlayPauseIcon.Glyph = CommonGlyphs.Play;
                            PlayPauseButton.ToolTip = Properties.Strings.SessionControl_Play;
                        }
                    }
                }
            }
            catch { }
        }

        private void UpdateShuffleButton(GlobalSystemMediaTransportControlsSession session)
        {
            try
            {
                if (session != null)
                {
                    var playback = session.GetPlaybackInfo();
                    if (playback != null)
                    {
                        if (playback.IsShuffleActive.HasValue)
                        {
                            if (playback.IsShuffleActive.Value)
                            {
                                ShuffleButton.IsChecked = true;
                                ShuffleButton.ToolTip = Properties.Strings.SessionControl_ShuffleOn;
                            }
                            else
                            {
                                ShuffleButton.IsChecked = false;
                                ShuffleButton.ToolTip = Properties.Strings.SessionControl_ShuffleOff;
                            }
                        }
                        else
                        {
                            ShuffleButton.IsChecked = false;
                        }
                    }
                }
            }
            catch { }
        }

        private void UpdateRepeatButton(GlobalSystemMediaTransportControlsSession session)
        {
            try
            {
                if (session != null)
                {
                    var playback = session.GetPlaybackInfo();
                    if (playback != null)
                    {
                        if (playback.AutoRepeatMode == MediaPlaybackAutoRepeatMode.None)
                        {
                            RepeatButton.IsChecked = false;
                            RepeatIcon.Glyph = CommonGlyphs.RepeatAll;
                            RepeatButton.ToolTip = Properties.Strings.SessionControl_RepeatOff;
                        }
                        else if (playback.AutoRepeatMode == MediaPlaybackAutoRepeatMode.Track)
                        {
                            RepeatButton.IsChecked = true;
                            RepeatIcon.Glyph = CommonGlyphs.RepeatOne;
                            RepeatButton.ToolTip = Properties.Strings.SessionControl_RepeatOne;
                        }
                        else if (playback.AutoRepeatMode == MediaPlaybackAutoRepeatMode.List)
                        {
                            RepeatButton.IsChecked = true;
                            RepeatIcon.Glyph = CommonGlyphs.RepeatAll;
                            RepeatButton.ToolTip = Properties.Strings.SessionControl_RepeatAll;
                        }
                    }
                }
            }
            catch { }
        }

        private void UpdateTimelineInfo(GlobalSystemMediaTransportControlsSession session)
        {
            try
            {
                var timeline = session.GetTimelineProperties();

                if (session.GetPlaybackInfo().Controls.IsPlaybackPositionEnabled && timeline != null)
                {
                    TimeBar.Minimum = timeline.StartTime.TotalSeconds;
                    TimeBar.Maximum = timeline.EndTime.TotalSeconds;
                    TimeBar.Value = timeline.Position.TotalSeconds;

                    CurrentTimeBlock.SetCurrentValue(TextBlock.TextProperty, timeline.Position.ToString("hh\\:mm\\:ss"));
                    TotalTimeBlock.SetCurrentValue(TextBlock.TextProperty, timeline.EndTime.ToString("hh\\:mm\\:ss"));

                    TimelineInfoButton.SetCurrentValue(VisibilityProperty, Visibility.Visible);
                }
                else
                {
                    TimeBar.Minimum = 0;
                    TimeBar.Maximum = 100;
                    TimeBar.Value = 0;

                    TimelineInfoButton.SetCurrentValue(VisibilityProperty, Visibility.Collapsed);
                }
            }
            catch
            {
                // ignored
            }
        }

        private async void UpdateSessionInfo(GlobalSystemMediaTransportControlsSession session)
        {
            BeginTrackTransition();

            try
            {
                var mediaInfo = await session.TryGetMediaPropertiesAsync();

                if (mediaInfo != null)
                {
                    MediaTitleBlock.Text = mediaInfo.Title;
                    MediaArtistBlock.Text = mediaInfo.Artist;
                }


                if (session.GetPlaybackInfo() != null)
                {
                    NextButton.SetCurrentValue(IsEnabledProperty, session.GetPlaybackInfo().Controls.IsNextEnabled);
                    PreviousButton.SetCurrentValue(IsEnabledProperty, session.GetPlaybackInfo().Controls.IsPreviousEnabled);
                    PlayPauseButton.SetCurrentValue(IsEnabledProperty, session.GetPlaybackInfo().Controls.IsPauseEnabled || session.GetPlaybackInfo().Controls.IsPlayEnabled);
                    ShuffleButton.SetCurrentValue(IsEnabledProperty, session.GetPlaybackInfo().Controls.IsShuffleEnabled);
                    RepeatButton.SetCurrentValue(IsEnabledProperty, session.GetPlaybackInfo().Controls.IsRepeatEnabled);
                    StopButton.SetCurrentValue(IsEnabledProperty, session.GetPlaybackInfo().Controls.IsStopEnabled);
                }

                MoreControlsButton.Visibility = (ShuffleButton.IsEnabled ||
                    RepeatButton.IsEnabled || StopButton.IsEnabled) ? Visibility.Visible : Visibility.Collapsed;

                UpdateTimelineInfo(session);

                UpdatePlaybackInfo(session);

                await SetThumbnailAsync(mediaInfo.Thumbnail, session.GetPlaybackInfo().PlaybackType);
            }
            catch { }

            EndTrackTransition();
        }

        #region Thumbnail

        // TODO: Re-use `.AsStream()` extension method from `System.IO.WindowsRuntimeStreamExtensions` once https://github.com/ShankarBUS/ModernFlyouts/issues/100 doesn't occur
        private async Task SetThumbnailAsync(IRandomAccessStreamReference thumbnail, MediaPlaybackType? playbackType)
        {
            if (thumbnail != null)
            {
                using var strm = await thumbnail.OpenReadAsync();
                if (strm != null && strm.Size > 0)
                {
                    using var dreader = new DataReader(strm);
                    await dreader.LoadAsync((uint)strm.Size);
                    var buffer = new byte[(int)strm.Size];
                    dreader.ReadBytes(buffer);

                    using var nstream = new MemoryStream(buffer);
                    nstream.Seek(0, SeekOrigin.Begin);
                    if (nstream != null && nstream.Length > 0)
                    {
                        ThumbnailImageBrush.ImageSource = ThumbnailBackgroundBrush.ImageSource = BitmapFrame.Create(nstream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad); ;
                        return;
                    }
                }
            }

            ThumbnailImageBrush.ImageSource = GetDefaultThumbnail(playbackType);
            ThumbnailBackgroundBrush.ImageSource = null;
        }

        private ImageSource GetDefaultThumbnail(MediaPlaybackType? playbackType)
        {
            return playbackType switch
            {
                MediaPlaybackType.Image => AudioFlyoutHelper.GetDefaultImageThumbnail(),
                MediaPlaybackType.Music => AudioFlyoutHelper.GetDefaultAudioThumbnail(),
                MediaPlaybackType.Video => AudioFlyoutHelper.GetDefaultVideoThumbnail(),
                MediaPlaybackType.Unknown => null,
                _ => null
            };
        }

        #endregion Thumbnail

        #region Transitions

        private void BeginTrackTransition()
        {
            ThumbnailBackgroundBrush.BeginAnimation(Brush.OpacityProperty, null);
            ThumbnailImageBrush.BeginAnimation(Brush.OpacityProperty, null);
            TextBlockGrid.BeginAnimation(OpacityProperty, null);
            mediaArtistBlockTranslateTransform.BeginAnimation(TranslateTransform.YProperty, null);
            mediaTitleBlockTranslateTransform.BeginAnimation(TranslateTransform.YProperty, null);

            ThumbnailBackgroundBrush.SetCurrentValue(Brush.OpacityProperty, 0.0);
            ThumbnailImageBrush.SetCurrentValue(Brush.OpacityProperty, 0.0);
            TextBlockGrid.SetCurrentValue(OpacityProperty, 0.0);
            mediaArtistBlockTranslateTransform.SetCurrentValue(TranslateTransform.YProperty, 0.0);
            mediaTitleBlockTranslateTransform.SetCurrentValue(TranslateTransform.YProperty, 0.0);
        }

        private void EndTrackTransition()
        {
            var fadeAnim = new FadeInThemeAnimation();
            ThumbnailBackgroundBrush.BeginAnimation(Brush.OpacityProperty, fadeAnim);
            ThumbnailImageBrush.BeginAnimation(Brush.OpacityProperty, fadeAnim);
            TextBlockGrid.BeginAnimation(OpacityProperty, fadeAnim);

            var YAnim1 = new DoubleAnimationUsingKeyFrames()
            {
                KeyFrames =
                {
                    new DiscreteDoubleKeyFrame(40, TimeSpan.Zero),
                    new SplineDoubleKeyFrame(0, TimeSpan.FromMilliseconds(367), new KeySpline(0.1, 0.9, 0.2, 1))
                }
            };
            mediaTitleBlockTranslateTransform.BeginAnimation(TranslateTransform.YProperty, YAnim1);

            var YAnim2 = new DoubleAnimationUsingKeyFrames()
            {
                BeginTime = TimeSpan.FromMilliseconds(100),
                KeyFrames = YAnim1.KeyFrames
            };
            mediaArtistBlockTranslateTransform.BeginAnimation(TranslateTransform.YProperty, YAnim2);
        }

        #endregion Transitions

        #endregion Updating View

        private static void OnAlignThumbnailToRightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sessionControl = d as SessionControl;
            var alignThumbnailToRight = (bool)e.NewValue;

            if (sessionControl != null)
            {
                var C0 = sessionControl.ContentGrid.ColumnDefinitions[0];
                var C2 = sessionControl.ContentGrid.ColumnDefinitions[2];

                if (alignThumbnailToRight)
                {
                    C0.Width = new GridLength(15, GridUnitType.Pixel);
                    C2.Width = new GridLength(0, GridUnitType.Auto);
                    sessionControl.TGParent.SetCurrentValue(Grid.ColumnProperty, 2);
                    sessionControl.thumbnailBGOpacityBrush.SetCurrentValue(RadialGradientBrush.GradientOriginProperty, sessionControl.thumbnailBGOpacityBrush.Center = new Point(1, 0.5));
                }
                else
                {
                    C0.Width = new GridLength(0, GridUnitType.Auto);
                    C2.Width = new GridLength(15, GridUnitType.Pixel);
                    sessionControl.TGParent.SetCurrentValue(Grid.ColumnProperty, 0);
                    sessionControl.thumbnailBGOpacityBrush.GradientOrigin = sessionControl.thumbnailBGOpacityBrush.Center = new Point(0, 0.5);
                }
            }
        }

        public void DisposeSession()
        {
            if (_SMTCSession != null)
            {
                _SMTCSession.MediaPropertiesChanged -= Session_MediaPropertiesChanged;
                _SMTCSession.PlaybackInfoChanged -= Session_PlaybackInfoChanged;
                _SMTCSession.TimelinePropertiesChanged -= Session_TimelinePropertiesChanged;
            }
            _SMTCSession = null;
        }
    }
}