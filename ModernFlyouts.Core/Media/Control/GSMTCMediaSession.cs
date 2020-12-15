using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Windows.Media.Control;
using Windows.Storage.Streams;

namespace ModernFlyouts.Core.Media.Control
{
    public class GSMTCMediaSession : MediaSession
    {
        private GlobalSystemMediaTransportControlsSession GSMTCSession;
        private SourceAppInfo sourceAppInfo;
        private int currentTrackNumber;

        public GSMTCMediaSession(GlobalSystemMediaTransportControlsSession globalSystemMediaTransportControlsSession)
        {
            GSMTCSession = globalSystemMediaTransportControlsSession;
            Initialize();
        }

        private void Initialize()
        {
            sourceAppInfo = SourceAppInfo.FromAppId(GSMTCSession.SourceAppUserModelId);
            if (sourceAppInfo != null)
            {
                sourceAppInfo.InfoFetched += SourceAppInfo_InfoFetched;
            }
            ActivateMediaSourceCommand = new RelayCommand(sourceAppInfo.Activate, () => sourceAppInfo != null);

            GSMTCSession.MediaPropertiesChanged += GSMTCSession_MediaPropertiesChanged;
            GSMTCSession.PlaybackInfoChanged += GSMTCSession_PlaybackInfoChanged;
            GSMTCSession.TimelinePropertiesChanged += GSMTCSession_TimelinePropertiesChanged;

            UpdateSessionInfo(GSMTCSession);
        }

        public void Dispose()
        {
            if (GSMTCSession != null)
            {
                GSMTCSession.MediaPropertiesChanged -= GSMTCSession_MediaPropertiesChanged;
                GSMTCSession.PlaybackInfoChanged -= GSMTCSession_PlaybackInfoChanged;
                GSMTCSession.TimelinePropertiesChanged -= GSMTCSession_TimelinePropertiesChanged;
            }
            GSMTCSession = null;
        }

        #region Hooking-up Events

        private async void GSMTCSession_MediaPropertiesChanged(GlobalSystemMediaTransportControlsSession session, MediaPropertiesChangedEventArgs args)
        {
            await Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
            {
                UpdateSessionInfo(session);
            }));
        }

        private async void GSMTCSession_PlaybackInfoChanged(GlobalSystemMediaTransportControlsSession session, PlaybackInfoChangedEventArgs args)
        {
            await Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                UpdatePlaybackInfo(session);
            }));
        }

        private async void GSMTCSession_TimelinePropertiesChanged(GlobalSystemMediaTransportControlsSession session, TimelinePropertiesChangedEventArgs args)
        {
            await Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                UpdateTimelineInfo(session);
            }));
        }

        #endregion

        private void SourceAppInfo_InfoFetched(object sender, EventArgs e)
        {
            sourceAppInfo.InfoFetched -= SourceAppInfo_InfoFetched;
            MediaSourceName = sourceAppInfo.AppName;
            MediaSourceIcon = sourceAppInfo.AppImage;
        }

        #region Updating Properties

        private void UpdatePlaybackInfo(GlobalSystemMediaTransportControlsSession session)
        {
            try
            {
                if (session != null)
                {
                    var playback = session.GetPlaybackInfo();
                    if (playback != null)
                    {
                        IsPlaying = playback.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing;
                        IsShuffleActive = playback.IsShuffleActive;
                        AutoRepeatMode = playback.AutoRepeatMode switch
                        {
                            Windows.Media.MediaPlaybackAutoRepeatMode.None => MediaPlaybackAutoRepeatMode.None,
                            Windows.Media.MediaPlaybackAutoRepeatMode.Track => MediaPlaybackAutoRepeatMode.Track,
                            Windows.Media.MediaPlaybackAutoRepeatMode.List => MediaPlaybackAutoRepeatMode.List,
                            _ => throw new NotImplementedException()
                        };
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
                    TimelineStartTime = timeline.StartTime;
                    TimelineEndTime = timeline.EndTime;
                    SetPlaybackPosition(timeline.Position);

                    IsTimelinePropertiesEnabled = true;
                }
                else
                {
                    TimelineStartTime = TimeSpan.Zero;
                    TimelineEndTime = TimeSpan.Zero;
                    PlaybackPosition = TimeSpan.Zero;

                    IsTimelinePropertiesEnabled = false;
                }
            }
            catch { }
        }

        private async void UpdateSessionInfo(GlobalSystemMediaTransportControlsSession session)
        {
            RaiseMediaPropertiesChanging();

            try
            {
                var mediaInfo = await session.TryGetMediaPropertiesAsync();

                if (mediaInfo != null)
                {
                    Title = mediaInfo.Title;
                    Artist = mediaInfo.Artist;
                    if (currentTrackNumber != mediaInfo.TrackNumber)
                    {
                        TrackChangeDirection = (mediaInfo.TrackNumber - currentTrackNumber) switch
                        {
                            > 0 => MediaPlaybackTrackChangeDirection.Forward,
                            < 0 => MediaPlaybackTrackChangeDirection.Backward,
                            _ => throw new NotImplementedException()
                        };
                    }

                    currentTrackNumber = mediaInfo.TrackNumber;

                    Debug.WriteLine(nameof(mediaInfo.Subtitle) + ": " + mediaInfo.Subtitle);
                }

                var playback = session.GetPlaybackInfo();

                if (playback != null)
                {
                    var playbackControls = playback.Controls;
                    IsPlayEnabled = playbackControls.IsPlayEnabled;
                    IsPauseEnabled = playbackControls.IsPauseEnabled;
                    IsPlayOrPauseEnabled = IsPlayEnabled || IsPauseEnabled;
                    IsPreviousEnabled = playbackControls.IsPreviousEnabled;
                    IsNextEnabled = playbackControls.IsNextEnabled;
                    IsShuffleEnabled = playbackControls.IsShuffleEnabled;
                    IsRepeatEnabled = playbackControls.IsRepeatEnabled;
                    IsStopEnabled = playbackControls.IsStopEnabled;

                    PlaybackType = playback.PlaybackType switch
                    {
                        Windows.Media.MediaPlaybackType.Unknown => MediaPlaybackType.Unknown,
                        Windows.Media.MediaPlaybackType.Music => MediaPlaybackType.Music,
                        Windows.Media.MediaPlaybackType.Video => MediaPlaybackType.Video,
                        Windows.Media.MediaPlaybackType.Image => MediaPlaybackType.Image,
                        _ => throw new NotImplementedException()
                    };
                }

                UpdateTimelineInfo(session);

                UpdatePlaybackInfo(session);

                Thumbnail = await GetThumbnailImageSourceAsync(mediaInfo?.Thumbnail);
            }
            catch { }

            RaiseMediaPropertiesChanged();
        }

        #region Thumbnail fetching

        private async Task<ImageSource> GetThumbnailImageSourceAsync(IRandomAccessStreamReference thumbnail)
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
                        return BitmapFrame.Create(nstream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    }
                }
            }

            return null;
        }

        #endregion

        #endregion

        #region Media Controlling

        protected override async void ChangeAutoRepeatMode()
        {
            try
            {
                Windows.Media.MediaPlaybackAutoRepeatMode autoRepeatMode = AutoRepeatMode switch
                {
                    MediaPlaybackAutoRepeatMode.None => Windows.Media.MediaPlaybackAutoRepeatMode.List,
                    MediaPlaybackAutoRepeatMode.List => Windows.Media.MediaPlaybackAutoRepeatMode.Track,
                    MediaPlaybackAutoRepeatMode.Track => Windows.Media.MediaPlaybackAutoRepeatMode.None,
                    _ => throw new NotImplementedException()
                };

                await GSMTCSession?.TryChangeAutoRepeatModeAsync(autoRepeatMode);
            }
            catch { }
        }

        protected override async void ChangeShuffleActive()
        {
            try
            {
                if (IsShuffleActive.HasValue)
                {
                    await GSMTCSession?.TryChangeShuffleActiveAsync(!IsShuffleActive.Value);
                }
            }
            catch { }
        }

        protected override async void NextTrack()
        {
            try
            {
                await GSMTCSession?.TrySkipNextAsync();
            }
            catch { }
        }

        protected override async void Pause()
        {
            try
            {
                await GSMTCSession?.TryPauseAsync();
            }
            catch { }
        }

        protected override async void Play()
        {
            try
            {
                await GSMTCSession?.TryPlayAsync();
            }
            catch { }
        }

        /// <summary>
        /// This function won't work properly since the <see cref="GlobalSystemMediaTransportControlsSession.TryChangePlaybackPositionAsync(long)"/>
        /// is un-reliable. But implemented it anyways thinking it could work in the future.
        /// </summary>
        /// <param name="playbackPosition"></param>
        protected override async void PlaybackPositionChanged(TimeSpan playbackPosition)
        {
            try
            {
                await GSMTCSession?.TryChangePlaybackPositionAsync((long)TimelineEndTime.TotalMilliseconds);
            }
            catch { }
        }

        protected override async void PreviousTrack()
        {
            try
            {
                await GSMTCSession?.TrySkipPreviousAsync();
            }
            catch { }
        }

        protected override async void Stop()
        {
            try
            {
                await GSMTCSession?.TryStopAsync();
            }
            catch { }
        }

        #endregion
    }
}
