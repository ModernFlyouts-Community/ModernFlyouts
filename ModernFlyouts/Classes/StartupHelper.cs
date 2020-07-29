using System.Diagnostics;

namespace ModernFlyouts
{
    internal class StartupHelper
    {
        private static string appPath = "";
        private const string RunKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";

        static StartupHelper()
        {
            appPath = Process.GetCurrentProcess().MainModule.FileName;
        }

        public static bool GetRunAtStartupEnabled()
        {
            var regkey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(RunKey, false);
            if (regkey == null)
            {
                return false;
            }

            if (regkey.GetValue(App.AppName) != null && regkey.GetValue(App.AppName) is string path)
            {
                return appPath == path;
            }

            return false;
        }

        public static void SetRunAtStartupEnabled(bool value)
        {
            var regkey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(RunKey, true);
            if (regkey != null)
            {
                if (value)
                {
                    regkey.SetValue(App.AppName, appPath);
                }
                else
                {
                    regkey.DeleteValue(App.AppName);
                }
            }
        }
    }
}
