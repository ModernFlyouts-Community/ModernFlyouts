using Microsoft.Toolkit.Mvvm.Input;
using ModernFlyouts.Core.AppInformation;
using ModernFlyouts.Core.Helpers;
using ModernFlyouts.Core.Threading;
using NPSMLib;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ModernFlyouts.Core.Media.Control
{
    public class NowPlayingMediaSession : MediaSession
    {
        private NowPlayingSession NPSession;
        private SourceAppInfo sourceAppInfo;
        private MediaPlaybackDataSource mediaPlaybackDataSource;
        private int currentTrackNumber;
        private string sourceAppId = string.Empty;

        public NowPlayingSession NowPlayingSession => NPSession;

        public NowPlayingMediaSession(NowPlayingSession nowPlayingSession)
        {
            NPSession = nowPlayingSession;
            Initialize();
        }

        private void Initialize()
        {
            if (NPSession == null)
                throw new NullReferenceException(nameof(NPSession));

            try
            {
                sourceAppId = NPSession.SourceAppId;
            }
            catch { }

            sourceAppInfo = SourceAppInfo.FromData(new SourceAppInfoData()
            {
                AppUserModelId = sourceAppId,
                ProcessId = NPSession.PID,
                MainWindowHandle = NPSession.Hwnd,
                DataType = SourceAppInfoDataType.FromProcessId
            });

            if (sourceAppInfo != null)
            {
                sourceAppInfo.InfoFetched += SourceAppInfo_InfoFetched;
                sourceAppInfo.FetchInfosAsync();
                ActivateMediaSourceCommand = new RelayCommand(sourceAppInfo.Activate, () => sourceAppInfo != null);
            }

            mediaPlaybackDataSource = NPSession.ActivateMediaPlaybackDataSource();

            if (mediaPlaybackDataSource == null)
                throw new InvalidOperationException(nameof(mediaPlaybackDataSource) + " should not be null");

            mediaPlaybackDataSource.MediaPlaybackDataChanged += MediaPlaybackDataSource_MediaPlaybackDataChanged;

            UpdateSessionInfo();
        }

        private void MediaPlaybackDataSource_MediaPlaybackDataChanged(object sender, MediaPlaybackDataChangedArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                switch (e.DataChangedEvent)
                {
                    case MediaPlaybackDataChangedEvent.MediaInfoChanged:
                        UpdateSessionInfo();
                        break;

                    case MediaPlaybackDataChangedEvent.PlaybackInfoChanged:
                        UpdatePlaybackInfo();
                        break;

                    case MediaPlaybackDataChangedEvent.TimelinePropertiesChanged:
                        UpdateTimelineInfo();
                        break;

                    default:
                        break;
                }
            });
        }

        public override void Disconnect()
        {
            if (mediaPlaybackDataSource != null)
            {
                try
                {
                    mediaPlaybackDataSource.MediaPlaybackDataChanged -= MediaPlaybackDataSource_MediaPlaybackDataChanged;
                }
                catch { }
            }
            mediaPlaybackDataSource = null;
            NPSession = null;

            sourceAppInfo.Dispose();
            sourceAppInfo = null;
        }

        private void SourceAppInfo_InfoFetched(object sender, EventArgs e)
        {
            if (sourceAppInfo != null)
            {
                sourceAppInfo.InfoFetched -= SourceAppInfo_InfoFetched;
                MediaSourceName = sourceAppInfo.DisplayName;

                if (BitmapHelper.TryCreateBitmapImageFromStream(sourceAppInfo.LogoStream, out var bitmap))
                {
                    MediaSourceIcon = bitmap;
                }
            }
        }

        #region Updating Properties

        private void UpdatePlaybackInfo()
        {
            if (mediaPlaybackDataSource != null)
            {
                var playback = mediaPlaybackDataSource.GetMediaPlaybackInfo();
                var valid = playback.PropsValid;
                IsPlaying = playback.PlaybackState == MediaPlaybackState.Playing;
                IsShuffleActive = valid.HasFlag(MediaPlaybackProps.ShuffleEnabled) ? playback.ShuffleEnabled : null;
                AutoRepeatMode = playback.RepeatMode switch
                {
                    MediaPlaybackRepeatMode.Unknown => MediaPlaybackAutoRepeatMode.None,
                    MediaPlaybackRepeatMode.None => MediaPlaybackAutoRepeatMode.None,
                    MediaPlaybackRepeatMode.Track => MediaPlaybackAutoRepeatMode.Track,
                    MediaPlaybackRepeatMode.List => MediaPlaybackAutoRepeatMode.List,
                    _ => throw new NotImplementedException(),
                };
            }
        }

        private void UpdateTimelineInfo()
        {
            if (mediaPlaybackDataSource != null)
            {
                var timeline = mediaPlaybackDataSource.GetMediaTimelineProperties();

                if (IsPlaybackPositionEnabled)
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
        }

        private void UpdateSessionInfo()
        {
            if (mediaPlaybackDataSource == null)
                return;

            RaiseMediaPropertiesChanging();

            var mediaInfo = mediaPlaybackDataSource.GetMediaObjectInfo();

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

            currentTrackNumber = (int)mediaInfo.TrackNumber;

            var playback = mediaPlaybackDataSource.GetMediaPlaybackInfo();

            var playbackCaps = playback.PlaybackCaps;
            IsPlayEnabled = playbackCaps.HasFlag(MediaPlaybackCapabilities.Play);
            IsPauseEnabled = playbackCaps.HasFlag(MediaPlaybackCapabilities.Pause);
            IsPlayOrPauseEnabled = IsPlayEnabled || IsPauseEnabled;
            IsPreviousEnabled = playbackCaps.HasFlag(MediaPlaybackCapabilities.Previous);
            IsNextEnabled = playbackCaps.HasFlag(MediaPlaybackCapabilities.Next);
            IsShuffleEnabled = playbackCaps.HasFlag(MediaPlaybackCapabilities.Shuffle);
            IsRepeatEnabled = playbackCaps.HasFlag(MediaPlaybackCapabilities.Repeat);
            IsStopEnabled = playbackCaps.HasFlag(MediaPlaybackCapabilities.Stop);
            IsPlaybackPositionEnabled = playbackCaps.HasFlag(MediaPlaybackCapabilities.PlaybackPosition);

            PlaybackType = playback.PlaybackMode switch
            {
                MediaPlaybackMode.Unknown => MediaPlaybackType.Unknown,
                MediaPlaybackMode.Audio => MediaPlaybackType.Music,
                MediaPlaybackMode.Video => MediaPlaybackType.Video,
                MediaPlaybackMode.Image => MediaPlaybackType.Image,
                MediaPlaybackMode.Podcast => MediaPlaybackType.Music,
                MediaPlaybackMode.AudioBook => MediaPlaybackType.Music,
                _ => throw new NotImplementedException(),
            };

            UpdateTimelineInfo();

            UpdatePlaybackInfo();

            Thumbnail = GetThumbnailImageSource();

            RaiseMediaPropertiesChanged();
        }

        #region Thumbnail fetching

        private const string SpotifyPackagedAUMID = "SpotifyAB.SpotifyMusic_zpdnekdrzrea0!Spotify";
        private const string SpotifyUnpackagedAUMID = "Spotify.exe";

        private bool IsSourceAppSpotify() => string.Equals(sourceAppId, SpotifyPackagedAUMID)
            || string.Equals(sourceAppId, SpotifyUnpackagedAUMID);

        private ImageSource GetThumbnailImageSource()
        {
            try
            {
                if (mediaPlaybackDataSource != null)
                {
                    var thumbnail = mediaPlaybackDataSource.GetThumbnailStream();
                    if (BitmapHelper.TryCreateBitmapImageFromStream(thumbnail, out var bitmap))
                    {
                        return GetModifiedThumbnail(bitmap);
                    }
                }
            }
            catch { }

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

        protected override void ChangeAutoRepeatMode()
        {
            MediaPlaybackRepeatMode repeatMode = AutoRepeatMode switch
            {
                MediaPlaybackAutoRepeatMode.None => MediaPlaybackRepeatMode.List,
                MediaPlaybackAutoRepeatMode.List => MediaPlaybackRepeatMode.Track,
                MediaPlaybackAutoRepeatMode.Track => MediaPlaybackRepeatMode.None,
                _ => throw new NotImplementedException(),
            };

            mediaPlaybackDataSource?.SendRepeatModeChangeRequest(repeatMode);
        }

        protected override void ChangeShuffleActive()
        {
            if (IsShuffleActive.HasValue)
            {
                mediaPlaybackDataSource?.SendShuffleEnabledChangeRequest(!IsShuffleActive.Value);
            }
        }

        protected override void NextTrack()
        {
            mediaPlaybackDataSource?.SendMediaPlaybackCommand(MediaPlaybackCommands.Next);
        }

        protected override void Pause()
        {
            mediaPlaybackDataSource?.SendMediaPlaybackCommand(MediaPlaybackCommands.Pause);
        }

        protected override void Play()
        {
            mediaPlaybackDataSource?.SendMediaPlaybackCommand(MediaPlaybackCommands.Play);
        }

        private DebounceDispatcher debouncer = new();

        protected override void PlaybackPositionChanged(TimeSpan playbackPosition)
        {
            debouncer.Debounce(TimeSpan.FromMilliseconds(20), () =>
            {
                mediaPlaybackDataSource?.SendPlaybackPositionChangeRequest(playbackPosition);
            });
        }

        protected override void PreviousTrack()
        {
            mediaPlaybackDataSource?.SendMediaPlaybackCommand(MediaPlaybackCommands.Previous);
        }

        protected override void Stop()
        {
            mediaPlaybackDataSource?.SendMediaPlaybackCommand(MediaPlaybackCommands.Stop);
        }

        #endregion
    }
}
