using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using NPSMLib;
using ModernFlyouts.Uwp.Helpers;
using System.IO;

namespace ModernFlyouts.Uwp.Services
{
    public static class MediaSessionService
    {
        public async static Task<BitmapImage> GetMusicThumbnail(NowPlayingSession session) => await StreamHelper.StreamToImageSource(session.ActivateMediaPlaybackDataSource().GetThumbnailStream());

        public async static Task<BitmapImage> ConvertIconToBitmap(Stream stream) => await StreamHelper.StreamToImageSource(stream);
    }
}
