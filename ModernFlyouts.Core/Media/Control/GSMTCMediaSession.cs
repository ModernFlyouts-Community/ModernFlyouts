using CommunityToolkit.Mvvm.Input;
using ModernFlyouts.Core.AppInformation;
using ModernFlyouts.Core.Helpers;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
            sourceAppInfo = SourceAppInfo.FromAppUserModelId(GSMTCSession.SourceAppUserModelId);
            if (sourceAppInfo != null)
            {
                sourceAppInfo.InfoFetched += SourceAppInfo_InfoFetched;
                sourceAppInfo.FetchInfosAsync();
                ActivateMediaSourceCommand = new RelayCommand(sourceAppInfo.Activate, () => sourceAppInfo != null);
            }

            GSMTCSession.MediaPropertiesChanged += GSMTCSession_MediaPropertiesChanged;
            GSMTCSession.PlaybackInfoChanged += GSMTCSession_PlaybackInfoChanged;
            GSMTCSession.TimelinePropertiesChanged += GSMTCSession_TimelinePropertiesChanged;

            UpdateSessionInfo(GSMTCSession);
        }

        public override void Disconnect()
        {
            if (GSMTCSession != null)
            {
                GSMTCSession.MediaPropertiesChanged -= GSMTCSession_MediaPropertiesChanged;
                GSMTCSession.PlaybackInfoChanged -= GSMTCSession_PlaybackInfoChanged;
                GSMTCSession.TimelinePropertiesChanged -= GSMTCSession_TimelinePropertiesChanged;
            }
            GSMTCSession = null;

            sourceAppInfo.Dispose();
            sourceAppInfo = null;
        }

        #region Hooking-up Events

        private void GSMTCSession_MediaPropertiesChanged(GlobalSystemMediaTransportControlsSession session, MediaPropertiesChangedEventArgs args)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                UpdateSessionInfo(session);
            });
        }

        private void GSMTCSession_PlaybackInfoChanged(GlobalSystemMediaTransportControlsSession session, PlaybackInfoChangedEventArgs args)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                UpdatePlaybackInfo(session);
            });
        }

        private void GSMTCSession_TimelinePropertiesChanged(GlobalSystemMediaTransportControlsSession session, TimelinePropertiesChangedEventArgs args)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                UpdateTimelineInfo(session);
            });
        }

        #endregion

        private void SourceAppInfo_InfoFetched(object sender, EventArgs e)
        {
            sourceAppInfo.InfoFetched -= SourceAppInfo_InfoFetched;
            MediaSourceName = sourceAppInfo.DisplayName;

            if (BitmapHelper.TryCreateBitmapImageFromStream(sourceAppInfo.LogoStream, out var bitmap))
            {
                MediaSourceIcon = bitmap;
            }
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
                            _ => MediaPlaybackTrackChangeDirection.Unknown
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
                    IsPlaybackPositionEnabled = playbackControls.IsPlaybackPositionEnabled;

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

        private const string SpotifyPackagedAUMID = "SpotifyAB.SpotifyMusic_zpdnekdrzrea0!Spotify";
        private const string SpotifyUnpackagedAUMID = "Spotify.exe";

        private bool IsSourceAppSpotify() => string.Equals(GSMTCSession.SourceAppUserModelId, SpotifyPackagedAUMID)
            || string.Equals(GSMTCSession.SourceAppUserModelId, SpotifyUnpackagedAUMID);

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
                        return GetModifiedThumbnail(BitmapFrame.Create(nstream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad));
                    }
                }
            }

            return null;
        }

        private BitmapSource GetModifiedThumbnail(BitmapSource bitmapSource)
        {
            if (IsSourceAppSpotify())
            {
                return GetModifiedThumbnailForSpotify(bitmapSource);
            }

            return bitmapSource;
        }

        /// <summary>
        /// Gets a modified version of the original thumbnail provided by Spotify.
        /// It crops and removes extra padding and Spotify branding from the original thumbnail provided by Spotify.
        /// </summary>
        /// <param name="bitmapSource">The original thumbnail provided by the source.</param>
        /// <returns>A cropped variant of the original thumbnail with extra padding and Spotify branding removed.</returns>
        /// <remarks>F*** you Spotify!</remarks>
        private BitmapSource GetModifiedThumbnailForSpotify(BitmapSource bitmapSource)
        {
            CroppedBitmap croppedBitmap = null;

            Application.Current.Dispatcher.Invoke(() =>
            {
                croppedBitmap = new CroppedBitmap(bitmapSource, new Int32Rect(33, 0, 234, 234));
            });

            return croppedBitmap;
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

        protected override async void PlaybackPositionChanged(TimeSpan playbackPosition)
        {
            try
            {
                await GSMTCSession?.TryChangePlaybackPositionAsync(playbackPosition.Ticks);
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
