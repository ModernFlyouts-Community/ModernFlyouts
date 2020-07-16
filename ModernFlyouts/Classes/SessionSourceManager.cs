using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ModernFlyouts
{
    public class SessionSourceManager
    {
        public static async void ActivateSourceAppAsync(string AppId)
        {
            if (AppId == null)
            {
                return;
            }

            await Task.Run(() => 
            {
                try
                {
                    
                    if (IsAppWin32(AppId))
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
                    else
                    {
                        var args = @"shell:appsFolder\" + AppId;
                        ProcessStartInfo processStartInfo = new ProcessStartInfo() { FileName = "explorer.exe", Arguments = args, UseShellExecute = true };
                        Process.Start(processStartInfo);
                    }
                } catch { }
            });
        }

        private static bool IsAppWin32(string AppId)
        {
            return AppId.EndsWith(".exe", StringComparison.OrdinalIgnoreCase);
        }
    }
}
