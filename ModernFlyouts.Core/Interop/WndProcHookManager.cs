﻿using System;
using System.Collections.Generic;

namespace ModernFlyouts.Core.Interop
{
    public class WndProcHookManager
    {
        private Dictionary<uint, WndProc> hooks = new();

        private List<IWndProcHookHandler> hookHandlers = new();

        private static Dictionary<BandWindow, WndProcHookManager> hookManagers = new();

        internal static WndProcHookManager RegisterForBandWindow(BandWindow bandWindow)
        {
            if (bandWindow == null)
                throw new ArgumentNullException(nameof(bandWindow));

            WndProcHookManager hookManager = new();

            hookManagers.Add(bandWindow, hookManager);

            return hookManager;
        }

        public static WndProcHookManager GetForBandWindow(BandWindow bandWindow)
        {
            if (bandWindow == null)
                throw new ArgumentNullException(nameof(bandWindow));

            if (hookManagers.TryGetValue(bandWindow, out var hookManager))
            {
                return hookManager;
            }

            return null;
        }

        public void RegisterHookHandler(IWndProcHookHandler hookHandler)
        {
            if (hookHandler == null)
                throw new ArgumentNullException(nameof(hookHandler));

            hookHandlers.Add(hookHandler);
        }

        public void RegisterHookHandlerForMessage(uint msg, IWndProcHookHandler hookHandler)
        {
            if (hookHandler == null)
                throw new ArgumentNullException(nameof(hookHandler));

            hooks.Add(msg, hookHandler.OnWndProc);
        }

        public void RegisterCallbackForMessage(uint msg, WndProc callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            hooks.Add(msg, callback);
        }

        internal void OnHwndCreated(IntPtr hWnd)
        {
            foreach (var hookHandler in hookHandlers)
            {
                uint msg = hookHandler.OnHwndCreated(hWnd, out bool register);
                if (register)
                {
                    RegisterHookHandlerForMessage(msg, hookHandler);
                }
            }
        }

        internal IntPtr TryHandleWindowMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam, out bool handled)
        {
            handled = false;

            if (hooks.TryGetValue(msg, out var hook))
            {
                handled = true;
                return hook(hWnd, msg, wParam, lParam);
            }

            return IntPtr.Zero;
        }
    }

    public interface IWndProcHookHandler
    {
        public uint OnHwndCreated(IntPtr hWnd, out bool register);

        public IntPtr OnWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
    }
}
