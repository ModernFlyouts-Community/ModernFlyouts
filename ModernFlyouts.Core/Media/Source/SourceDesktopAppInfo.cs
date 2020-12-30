using ModernFlyouts.Core.Interop;
using ModernFlyouts.Core.Utilities;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using static ModernFlyouts.Core.Interop.NativeMethods;

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

        private Process sourceProcess;

        public override void Activate()
        {
            try
            {
                if (sourceProcess == null)
                    return;

                var hWnd = sourceProcess.MainWindowHandle;
                if (hWnd != IntPtr.Zero)
                {
                    if (GetWindowSizeState(hWnd) == WindowSizeState.Minimized)
                    {
                        ShowWindowAsync(hWnd, (int)ShowWindowCommands.Restore);
                    }
                    else
                    {
                        SetForegroundWindow(hWnd);
                    }

                    FlashWindow(hWnd, true);
                }
            }
            catch { }
        }

        private async void FetchInfos()
        {
            Bitmap bitmap = new(16, 16);

            await Task.Run(() =>
            {
                var processName = AppId[..^4];
                var processes = Process.GetProcessesByName(processName);

                if (processes?.Length > 0)
                {
                    sourceProcess = processes[0];
                    try
                    {
                        AppName = sourceProcess.MainModule.FileVersionInfo.FileDescription;
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

        #region Window activation things

        private static WindowSizeState GetWindowSizeState(IntPtr hWnd)
        {
            GetWindowPlacement(hWnd, out WINDOWPLACEMENT placement);

            return placement.ShowCmd switch
            {
                ShowWindowCommands.Normal => WindowSizeState.Normal,
                ShowWindowCommands.Minimize or ShowWindowCommands.ShowMinimized => WindowSizeState.Minimized,
                ShowWindowCommands.Maximize => WindowSizeState.Maximized,
                _ => WindowSizeState.Unknown,
            };
        }

        private enum WindowSizeState
        {
            Normal,
            Minimized,
            Maximized,
            Unknown,
        }

        #endregion
    }
}
