using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ModernFlyouts
{
    internal class SourceDesktopAppInfo : SourceAppInfo
    {
        public SourceDesktopAppInfo(string appId)
        {
            AppId = appId;
            FetchInfos();
        }

        public override event EventHandler InfoFetched;

        public override void Activate()
        {
            ActivateAsync();
        }

        private async void ActivateAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    var processName = AppId.Remove(AppId.Length - 5, 4);
                    var processes = Process.GetProcessesByName(processName);

                    if (processes?.Length > 0)
                    {
                        var hWnd = processes[0].MainWindowHandle;
                        if (hWnd != null && hWnd != IntPtr.Zero)
                        {
                            NativeMethods.ShowWindowAsync(hWnd, (int)NativeMethods.ShowWindowCommands.ShowNA);
                            NativeMethods.SetForegroundWindow(hWnd);
                        }
                    }
                }
                catch { }
            });
        }

        private async void FetchInfos()
        {
            Bitmap bitmap = null;

            await Task.Run(() =>
            {
                var processName = AppId.Remove(AppId.Length - 5, 4);
                var processes = Process.GetProcessesByName(processName);

                if (processes?.Length > 0)
                {
                    try
                    {
                        AppName = processes[0].MainModule.FileVersionInfo.FileDescription;
                    } catch { }

                    try
                    {
                        var path = processes[0].MainModule.FileName;
                        var ie = new IconExtractor(path);
                        var icon = ie.GetIcon(0);
                        bitmap = icon.ToBitmap();
                    } catch { }
                }
            });

            if (bitmap != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    AppImage = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                }, System.Windows.Threading.DispatcherPriority.Send);
            }

            InfoFetched?.Invoke(this, null);
        }
    }
}
