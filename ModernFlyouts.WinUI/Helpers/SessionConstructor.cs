using ModernFlyouts.Core.Utilities;
using ModernFlyouts.Standard.Classes;
using ModernFlyouts.Utilities;
using NPSMLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ModernFlyouts.WinUI.Helpers
{
    public class SessionConstructor
    {
        public static async Task<MediaSession> ConstructSessionAsync(NowPlayingSession session)
        {
            return (IsImmersiveProcess(Process.GetProcessById((int)session.PID).Handle) ? await ModernAppInfoExtractor.GetModernAppInfo(session) : DesktopAppInfoExtractor.GetDesktopAppInfo(session));
        }

        [DllImport("user32.dll")]
        public static extern bool IsImmersiveProcess(IntPtr hProcess);
    }
}
