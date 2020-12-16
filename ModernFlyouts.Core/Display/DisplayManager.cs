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
            WndProcHookManager.RegisterHookHandlerForMessage(WM_SETTINGCHANGE, this);
        }

        public override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if ((msg == WM_SETTINGCHANGE) && (wParam == ((IntPtr)SPI_SETWORKAREA)))
            {
                handled = true;

                DisplayUpdated?.Invoke(this, EventArgs.Empty);
            }
            return IntPtr.Zero;
        }
    }
}
