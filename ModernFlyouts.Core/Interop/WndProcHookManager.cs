using System;
using System.Collections.Generic;
using System.Windows.Interop;

namespace ModernFlyouts.Core.Interop
{
    public class WndProcHookManager
    {
        private static Dictionary<int, HwndSourceHook> hooks = new();

        private static List<WndProcHookHandler> hookHandlers = new();

        public static void RegisterHookHandler(WndProcHookHandler hookHandler)
        {
            if (hookHandler == null)
                throw new ArgumentNullException(nameof(hookHandler));

            hookHandlers.Add(hookHandler);
        }

        public static void RegisterHookHandlerForMessage(int msg, WndProcHookHandler hookHandler)
        {
            if (hookHandler == null)
                throw new ArgumentNullException(nameof(hookHandler));

            hooks.Add(msg, hookHandler.WndProc);
        }

        public static void RegisterCallbackForMessage(int msg, HwndSourceHook callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            hooks.Add(msg, callback);
        }

        public static void OnHwndCreated(IntPtr hWnd)
        {
            foreach (var hookHandler in hookHandlers)
            {
                hookHandler.OnHwndCreated(hWnd);
            }
        }

        public static IntPtr TryHandleWindowMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (hooks.TryGetValue(msg, out var hook))
            {
                return hook(hWnd, msg, wParam, lParam, ref handled);
            }

            return IntPtr.Zero;
        }
    }

    public abstract class WndProcHookHandler
    {
        public virtual void OnHwndCreated(IntPtr hWnd)
        {
        }

        public abstract IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled);
    }
}
