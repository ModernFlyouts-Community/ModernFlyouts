using ModernFlyouts.Standard.Classes;
using NPSMLib;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

/* 
 * This code was imported from the legacy ModernFlyouts app
 */

namespace ModernFlyouts.Core.Utilities
{
    public class DesktopAppInfoExtractor
    {
        private static Process sourceProcess;

        public static MediaSession GetDesktopAppInfo(NowPlayingSession nps)
        {
            Bitmap bitmap = new Bitmap(16, 16);
            MediaSession ms = new MediaSession();
            string DisplayName = "Desktop";

            try
            {
                sourceProcess = Process.GetProcessById((int)nps.PID);
            }
            catch
            {
                try
                {
                    var processName = nps.SourceAppId[..^4];
                    var processes = Process.GetProcessesByName(processName);

                    if (processes?.Length > 0)
                    {
                        sourceProcess = processes[0];
                    }
                }
                catch
                {

                }
            }

            try
           {
                DisplayName = sourceProcess.MainModule.FileVersionInfo.FileDescription;
           }
            catch { }

            try
            {
                var path = sourceProcess.MainModule.FileName;
                var ie = new IconExtractor(path);
                var icon = ie.GetIcon(0);
                bitmap = icon.ToBitmap();
            }
            catch { }
            MemoryStream memoryStream = new MemoryStream();
            if (bitmap != null)
            {

                bitmap.Save(memoryStream, ImageFormat.Png);
                memoryStream.Seek(0, SeekOrigin.Begin);
            }

            ms.AppName = DisplayName;
            ms.AppIcon = memoryStream;
            ms.MediaPlayingSession = nps;
            return ms;
        }
    }
}
