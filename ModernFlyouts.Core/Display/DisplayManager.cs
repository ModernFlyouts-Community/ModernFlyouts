using ModernFlyouts.Core.Interop;
using System;
using static ModernFlyouts.Core.Interop.NativeMethods;

namespace ModernFlyouts.Core.Display
{
    public class DisplayManager : WndProcHookHandler
    {
        public static DisplayManager Instance { get; } = new();

        public event EventHandler DisplayUpdated;

        private DisplayManager()
        {
        }

        public override IntPtr OnWndProc(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            if ((msg == WM_SETTINGCHANGE) && (wParam == ((IntPtr)SPI_SETWORKAREA)))
            {
                DisplayUpdated?.Invoke(this, EventArgs.Empty);
            }
            return IntPtr.Zero;
        }
    }
}
