namespace ModernFlyouts.Core.Media
{
    /// <summary>
    /// Specifies the auto repeat mode for media playback.
    /// </summary>
    public enum MediaPlaybackAutoRepeatMode
    {
        /// <summary>
        /// No repeating.
        /// </summary>
        None = 0,

        /// <summary>
        /// Repeat the current track.
        /// </summary>
        Track = 1,

        /// <summary>
        /// Repeat the current list of tracks.
        /// </summary>
        List = 2
    }
}
