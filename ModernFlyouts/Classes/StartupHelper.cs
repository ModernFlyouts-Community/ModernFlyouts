using System.Diagnostics;

namespace ModernFlyouts
{
    internal class StartupHelper
    {
        private static string appPath = "";
        private const string appName = "ModernFlyouts";
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

            if (regkey.GetValue(appName) != null && regkey.GetValue(appName) is string path)
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
                    regkey.SetValue(appName, appPath);
                }
                else
                {
                    regkey.DeleteValue(appName);
                }
            }
        }
    }
}
