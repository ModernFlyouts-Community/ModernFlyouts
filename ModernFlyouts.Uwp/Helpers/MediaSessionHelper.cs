using NPSMLib;

namespace ModernFlyouts.Uwp.Helpers
{
    class MediaSessionHelper
    {
        public static MediaObjectInfo GetMediaInfo(NowPlayingSession session) => session.ActivateMediaPlaybackDataSource().GetMediaObjectInfo();

        public static MediaTimelineProperties GetTimelineInfo(NowPlayingSession session) => session.ActivateMediaPlaybackDataSource().GetMediaTimelineProperties();

        public static MediaPlaybackInfo GetPlaybackInfo(NowPlayingSession session) => session.ActivateMediaPlaybackDataSource().GetMediaPlaybackInfo();
    }
}
