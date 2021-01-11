using ModernFlyouts.Core.Interop;
using ModernFlyouts.Core.Utilities;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using static ModernFlyouts.Core.Interop.NativeMethods;

namespace ModernFlyouts.Core.AppInformation
{
    internal class SourceDesktopAppInfo : SourceAppInfo
    {
        public SourceDesktopAppInfo(SourceAppInfoData data)
        {
            Data = data;
        }

        public override event EventHandler InfoFetched;

        private Process sourceProcess;

        public override void Activate()
        {
            if (sourceProcess == null)
                return;

            IntPtr hWnd = IsWindow(Data.MainWindowHandle)
                ? Data.MainWindowHandle : sourceProcess.MainWindowHandle;

            ActivateWindow(hWnd);
        }

        public override async void FetchInfosAsync()
        {
            Bitmap bitmap = new(16, 16);

            await Task.Run(() =>
            {
                try
                {
                    if (Data.DataType == SourceAppInfoDataType.FromAppUserModelId)
                    {
                        var processName = Data.AppUserModelId[..^4];
                        var processes = Process.GetProcessesByName(processName);

                        if (processes?.Length > 0)
                        {
                            sourceProcess = processes[0];
                        }
                    }
                    else if (Data.DataType == SourceAppInfoDataType.FromProcessId)
                    {
                        sourceProcess = Process.GetProcessById((int)Data.ProcessId);
                    }
                }
                catch { }

                if (sourceProcess == null)
                    return;

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

                if (bitmap != null)
                {
                    MemoryStream memoryStream = new();
                    bitmap.Save(memoryStream, ImageFormat.Png);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    LogoStream = memoryStream;
                }
            });

            InfoFetched?.Invoke(this, null);
        }

        protected override void Disconnect()
        {
            base.Disconnect();

            sourceProcess.Dispose();
            sourceProcess = null;
        }

        #region Window activation things

        internal static void ActivateWindow(IntPtr hWnd)
        {
            if (hWnd != IntPtr.Zero)
            {
                if (GetWindowSizeState(hWnd) == WindowSizeState.Minimized)
                {
                    ShowWindowAsync(hWnd, ShowWindowCommands.Restore);
                }

                SetForegroundWindow(hWnd);
                FlashWindow(hWnd, true);
            }
        }

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
