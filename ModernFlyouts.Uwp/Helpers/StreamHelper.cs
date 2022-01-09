using System;
using System.IO;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace ModernFlyouts.Uwp.Helpers
{
    public class StreamHelper
    {
        public async static Task<BitmapImage> StreamToImageSource(Stream stream)
        {
            BitmapImage Bitmap = new BitmapImage();
            await Bitmap.SetSourceAsync(stream.AsRandomAccessStream());
            return Bitmap;
        }
    }
}
