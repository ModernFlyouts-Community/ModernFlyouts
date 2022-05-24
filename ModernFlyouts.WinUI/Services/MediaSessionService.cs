using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media.Imaging;
using NPSMLib;
using ModernFlyouts.WinUI.Helpers;
using System.IO;

namespace ModernFlyouts.WinUI.Services
{
    public static class MediaSessionService
    {
        public async static Task<BitmapImage> GetMusicThumbnail(NowPlayingSession session) => await StreamHelper.StreamToImageSource(session.ActivateMediaPlaybackDataSource().GetThumbnailStream());

        public async static Task<BitmapImage> ConvertIconToBitmap(Stream stream) => await StreamHelper.StreamToImageSource(stream);
    }
}
