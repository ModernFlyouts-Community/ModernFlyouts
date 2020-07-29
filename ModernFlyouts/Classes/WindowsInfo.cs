using System;
using System.Collections.Generic;
using System.Text;

namespace ModernFlyouts.Classes
{
    public class WindowsInfo
    {
        public static bool IsWindows8OrLater()
        {
            var osVer = Environment.OSVersion;
            if (osVer.Version.Major == 10 || (osVer.Version.Major == 6 && osVer.Version.Minor > 1))
                return true;

            return false;
        }

        public static bool IsWindows10()
        {
            var osVer = Environment.OSVersion;
            return osVer.Version.Major == 10;
        }
    }
}
