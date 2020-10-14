using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using static ModernFlyouts.NativeMethods;

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
            Bitmap bitmap = new Bitmap(16, 16);

            await Task.Run(() =>
            {
                var processName = AppId.Substring(0, AppId.Length - 4);
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

        #region Window Activation things

        public WindowSizeState GetWindowSizeState(IntPtr hWnd)
        {
            GetWindowPlacement(hWnd, out WINDOWPLACEMENT placement);

            switch (placement.ShowCmd)
            {
                case ShowWindowCommands.Normal:
                    return WindowSizeState.Normal;
                case ShowWindowCommands.Minimize:
                case ShowWindowCommands.ShowMinimized:
                    return WindowSizeState.Minimized;
                case ShowWindowCommands.Maximize:
                    return WindowSizeState.Maximized;
                default:
                    return WindowSizeState.Unknown;
            }
        }

        public enum WindowSizeState
        {
            Normal,
            Minimized,
            Maximized,
            Unknown,
        }

        #endregion
    }
}
