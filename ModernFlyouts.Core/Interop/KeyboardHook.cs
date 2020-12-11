using System;
using System.Runtime.InteropServices;
using System.Windows.Input;
using static ModernFlyouts.Core.Interop.NativeMethods;

namespace ModernFlyouts.Core.Interop
{
    public class KeyboardHook
    {
        public event KeyDownEventHandler KeyDown;

        public delegate void KeyDownEventHandler(Key key, int virtualKey);

        public event KeyUpEventHandler KeyUp;

        public delegate void KeyUpEventHandler(Key key, int virtualKey);

        private const int WH_KEYBOARD_LL = 13;
        private const int HC_ACTION = 0;
        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP = 0x101;
        private const int WM_SYSKEYDOWN = 0x104;
        private const int WM_SYSKEYUP = 0x105;

        private HookProc _procDelegate;
        private IntPtr HHookID = IntPtr.Zero;

        #region Properties

        public Key? CurrentKey { get; private set; }

        #endregion

        public KeyboardHook()
        {
            _procDelegate = new HookProc(KeyboardProc);
            var module = System.Diagnostics.Process.GetCurrentProcess().MainModule;
            IntPtr handle = GetModuleHandle(module.ModuleName);
            HHookID = (IntPtr)SetWindowsHookEx(WH_KEYBOARD_LL, _procDelegate, handle, 0);
            if (HHookID == IntPtr.Zero)
            {
                throw new Exception("Could not set keyboard hook");
            }
        }

        private int KeyboardProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode == HC_ACTION)
            {
                if (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN)
                {
                    var vkcode = (int)((KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT))).vkCode;
                    var key = KeyInterop.KeyFromVirtualKey(vkcode);
                    CurrentKey = key;
                    KeyDown?.Invoke(key, vkcode);
                }
                if (wParam == (IntPtr)WM_KEYUP || wParam == (IntPtr)WM_SYSKEYUP)
                {
                    var vkcode = (int)((KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT))).vkCode;
                    var key = KeyInterop.KeyFromVirtualKey(vkcode);
                    CurrentKey = null;
                    KeyUp?.Invoke(key, vkcode);
                }
            }

            return CallNextHookEx((int)IntPtr.Zero, nCode, wParam, lParam);
        }

        ~KeyboardHook()
        {
            if (HHookID != IntPtr.Zero)
            {
                UnhookWindowsHookEx((int)HHookID);
            }
        }
    }
}
