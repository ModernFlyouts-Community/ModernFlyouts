using NPSMLib;
using System.IO;

namespace ModernFlyouts.Standard.Classes
{
    public class MediaSession
    {
        public NowPlayingSession MediaPlayingSession { get; set; }

        public MemoryStream AppIcon { get; set; }

        public string AppName { get; set; }
    }
}
