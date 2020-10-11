using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static ModernFlyouts.NativeMethods;

namespace ModernFlyouts
{
    public static class DUIHandler
    {
        public const int MAX_TRIES = 6;

        #region Properties

        public static IntPtr HWndHost { get; private set; } = IntPtr.Zero;

        public static IntPtr HWndDUI { get; private set; } = IntPtr.Zero;

        public static uint ProcessId { get; private set; } = 0;

        #endregion

        public static Point GetCoordinates()
        {
            var defaultPos = new Point(50, 60);

            if (HWndHost != IntPtr.Zero & GetWindowRect(HWndHost, out RECT rct))
            {
                Debug.WriteLine($"({rct.Left}, {rct.Top})");
                return new Point(rct.Left, rct.Top);
            }
            return defaultPos;
        }

        public static async void ForceFindDUIAndHide(bool minimizeDUI = true)
        {
            await Task.Run(() =>
            {
                int tries = 0;

                while (GetAllInfos() == false && tries++ < MAX_TRIES)
                {
                    //Sometimes the volume flyout isn't created yet, so we need to send some a key stroke.
                    keybd_event((byte)KeyInterop.VirtualKeyFromKey(Key.VolumeUp), 0, 0, 0);
                    keybd_event((byte)KeyInterop.VirtualKeyFromKey(Key.VolumeDown), 0, 0, 0);

                    FindDUIAndHide(minimizeDUI);

                    Thread.Sleep(500);
                }
            });
        }

        public static void FindDUIAndHide(bool minimizeDUI = true)
        {
            if (IsDUIAvailable())
            {
                if (minimizeDUI)
                {
                    ShowWindowAsync(HWndDUI, (int)ShowWindowCommands.Minimize);
                }
                ShowWindowAsync(HWndHost, (int)ShowWindowCommands.Hide);
            }
        }

        public static void FindDUIAndPermanentlyHide()
        {
            if (IsDUIAvailable())
            {
                ShowWindowAsync(HWndDUI, (int)ShowWindowCommands.Minimize);
                ShowWindowAsync(HWndHost, (int)ShowWindowCommands.Minimize);
                ShowWindowAsync(HWndHost, (int)ShowWindowCommands.ForceMinimize);
            }
        }

        public static void FindDUIAndShow()
        {
            if (IsDUIAvailable())
            {
                ShowWindowAsync(HWndDUI, (int)ShowWindowCommands.Restore);
            }
        }

        public static bool GetAllInfos()
        {
            IntPtr hWndHost = IntPtr.Zero;

            while ((hWndHost = FindWindowEx(IntPtr.Zero, hWndHost, "NativeHWNDHost", "")) != IntPtr.Zero)
            {
                IntPtr hWndDUI;
                if ((hWndDUI = FindWindowEx(hWndHost, IntPtr.Zero, "DirectUIHWND", "")) != IntPtr.Zero)
                {
                    GetWindowThreadProcessId(hWndHost, out int pid);
                    if (Process.GetProcessById(pid).ProcessName.ToLower() == "explorer")
                    {
                        HWndHost = hWndHost;
                        HWndDUI = hWndDUI;
                        ProcessId = (uint)pid;
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool IsDUIAvailable()
        {
            return (ProcessId != 0) && (HWndHost != IntPtr.Zero) && (HWndDUI != IntPtr.Zero);
        }

        public static void VerifyDUI()
        {
            if (!IsDUIAvailable())
            {
                GetAllInfos();
                FlyoutHandler.Instance.DUIHook.Rehook();
            }
        }
    }
}