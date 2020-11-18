#if Screenshots

using System;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ModernFlyouts.Private
{
    internal class ScreenshotHelper
    {
        internal static void Initialize()
        {
            FlyoutHandler.Instance.KeyboardHook.KeyDown += KeyboardHook_KeyDown;
        }

        private static void KeyboardHook_KeyDown(Key key, int virtualKey)
        {
            if (key == Key.S && Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt))
            {
                RenderAndSaveScreenshot();
            }
        }

        internal static void RenderAndSaveScreenshot()
        {
            var content = FlyoutHandler.Instance.FlyoutWindow;
            int width = (int)Math.Ceiling(content.ActualWidth);
            int height = (int)Math.Ceiling(content.ActualHeight);

            var renderTargetBitmap = new RenderTargetBitmap(
                    width * 2, height * 2,
                    96 * 2, 96 * 2, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(content);

            var filename = @$".\Temp\{DateTime.Now.ToFileTime()}.png";
            using var fileStream = new FileStream(filename, FileMode.Create);
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
            encoder.Save(fileStream);
        }
    }
}

#endif