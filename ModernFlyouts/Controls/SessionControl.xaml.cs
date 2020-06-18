using ModernFlyouts.Utilities;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Windows.Media.Control;
using Windows.Storage.Streams;

namespace ModernFlyouts
{
    public partial class SessionControl : UserControl
    {

        #region Properties

        private GlobalSystemMediaTransportControlsSession _SMTCSession;

        public GlobalSystemMediaTransportControlsSession SMTCSession
        {
            get => _SMTCSession;
            set
            {
                if (value != null)
                {
                    UpdateSessionInfo(value);
                    value.MediaPropertiesChanged += Session_MediaPropertiesChanged;
                    value.PlaybackInfoChanged += Value_PlaybackInfoChanged;
                    _SMTCSession = value;
                }
                else
                {
                    DisposeSession();
                }
            }
        }

        #endregion

        public SessionControl()
        {
            InitializeComponent();
            Back.Click += (_, __) => PreviousTrack();
            PlayPause.Click += (_, __) => PlayOrPause();
            Next.Click += (_, __) => NextTrack();
        }

        private async void Value_PlaybackInfoChanged(GlobalSystemMediaTransportControlsSession session, PlaybackInfoChangedEventArgs args)
        {
            await Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                if (session != null && session.GetPlaybackInfo() != null)
                    UpdatePlayPauseButtonIcon(session);
            }));
        }

        private async void Session_MediaPropertiesChanged(GlobalSystemMediaTransportControlsSession session, MediaPropertiesChangedEventArgs args)
        {
            await Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
            {
                if (session != null && session.GetPlaybackInfo() != null)
                    UpdateSessionInfo(session);

            }));
        }

        private async void PreviousTrack()
        {
            try
            {
                if (_SMTCSession != null)
                    await _SMTCSession.TrySkipPreviousAsync();
            } catch { }
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
                            await _SMTCSession.TryPauseAsync();
                        else if (playback.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Paused)
                            await _SMTCSession.TryPlayAsync();
                    }
                }
            } catch { }
        }

        private async void NextTrack()
        {
            try
            {
                if (_SMTCSession != null)
                    await _SMTCSession.TrySkipNextAsync();
            } catch { }
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
                            PlayPauseIcon.Glyph = CommonGlyphs.Pause;
                        else if (playback.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Paused)
                            PlayPauseIcon.Glyph = CommonGlyphs.Play;
                    }
                }
            } catch { }
        }

        private async void UpdateSessionInfo(GlobalSystemMediaTransportControlsSession session)
        {
            try
            {
                var mediaInfo = await session.TryGetMediaPropertiesAsync();
                SongName.Text = mediaInfo.Title;
                SongArtist.Text = mediaInfo.Artist;

                var playback = session.GetPlaybackInfo();

                if (playback != null)
                {
                    Next.IsEnabled = playback.Controls.IsNextEnabled;
                    Back.IsEnabled = playback.Controls.IsPreviousEnabled;
                    PlayPause.IsEnabled = playback.Controls.IsPauseEnabled || playback.Controls.IsPlayEnabled;
                }

                UpdatePlayPauseButtonIcon(session);

                await SetThumbnailAsync(mediaInfo.Thumbnail, playback.PlaybackType);

            } catch { }
        }

        private async Task SetThumbnailAsync(IRandomAccessStreamReference thumbnail, Windows.Media.MediaPlaybackType? playbackType)
        {
            if (thumbnail != null)
            {
                using var strm = await thumbnail.OpenReadAsync();
                if (strm != null)
                {
                    using var nstream = strm.AsStream();
                    if (nstream != null && nstream.Length > 0)
                    {
                        thumb.ImageSource = BitmapFrame.Create(nstream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                        return;
                    }
                }
            }
            thumb.ImageSource = GetDefaultThumbnail(playbackType);
        }

        private ImageSource GetDefaultThumbnail(Windows.Media.MediaPlaybackType? playbackType)
        {
            return playbackType switch
            {
                Windows.Media.MediaPlaybackType.Image => AudioHelper.GetDefaultImageThumbnail(),
                Windows.Media.MediaPlaybackType.Music => AudioHelper.GetDefaultAudioThumbnail(),
                Windows.Media.MediaPlaybackType.Video => AudioHelper.GetDefaultVideoThumbnail(),
                Windows.Media.MediaPlaybackType.Unknown => null,
                _ => null
            };
        }

        public void DisposeSession()
        {
            if (_SMTCSession != null)
            {
                _SMTCSession.MediaPropertiesChanged -= Session_MediaPropertiesChanged;
                _SMTCSession.PlaybackInfoChanged -= Value_PlaybackInfoChanged;
            }
            _SMTCSession = null;
        }
    }
}