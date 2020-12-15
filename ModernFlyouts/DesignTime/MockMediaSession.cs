using ModernFlyouts.Core.Media;
using ModernFlyouts.Core.Media.Control;
using ModernFlyouts.Helpers;
using System;
using System.Diagnostics;
using System.Windows.Media.Imaging;

namespace ModernFlyouts.DesignTime
{
    internal class MockMediaSession : MediaSession
    {
        public MockMediaSession()
        {
            IsPlayEnabled = true;
            IsPauseEnabled = true;
            IsPlayOrPauseEnabled = true;
            IsPreviousEnabled = true;
            IsNextEnabled = true;
            IsShuffleEnabled = true;
            IsRepeatEnabled = true;
            IsStopEnabled = true;

            IsTimelinePropertiesEnabled = true;
            TimelineStartTime = TimeSpan.Zero;
            TimelineEndTime = TimeSpan.FromMinutes(3);
            PlaybackPosition = TimeSpan.FromMinutes(1.5);

            IsPlaying = false;
            IsShuffleActive = false;
            AutoRepeatMode = MediaPlaybackAutoRepeatMode.None;
            PlaybackType = MediaPlaybackType.Image;


            MediaSourceName = "TestApp";
            MediaSourceIcon = new BitmapImage(PackUriHelper.GetAbsoluteUri("/Assets/Images/ModernFlyouts_16.png"));
            Thumbnail = AudioFlyoutHelper.GetDefaultImageThumbnail();

            Title = "This is a lengthy title of a songggggggggggggggggggggggggg";
            Artist = "Why not me?";
        }

        protected override void ChangeAutoRepeatMode()
        {
            AutoRepeatMode = AutoRepeatMode switch
            {
                MediaPlaybackAutoRepeatMode.None => MediaPlaybackAutoRepeatMode.List,
                MediaPlaybackAutoRepeatMode.List => MediaPlaybackAutoRepeatMode.Track,
                MediaPlaybackAutoRepeatMode.Track => MediaPlaybackAutoRepeatMode.None,
                _ => throw new NotImplementedException()
            };
        }

        protected override void ChangeShuffleActive()
        {
            IsShuffleActive = !IsShuffleActive;
        }

        protected override void NextTrack()
        {
            Debug.WriteLine(nameof(MockMediaSession) + ": Next track");
        }

        protected override void Pause()
        {
            IsPlaying = false;
        }

        protected override void Play()
        {
            IsPlaying = true;
        }

        protected override void PlaybackPositionChanged(TimeSpan playbackPosition)
        {
            Debug.WriteLine(nameof(MockMediaSession) + ": Playback Position - " + PlaybackPosition.ToString("hh\\:mm\\:ss"));
        }

        protected override void PreviousTrack()
        {
            Debug.WriteLine(nameof(MockMediaSession) + ": Previous track");
        }

        protected override void Stop()
        {
            IsPlaying = false;
        }
    }
}
