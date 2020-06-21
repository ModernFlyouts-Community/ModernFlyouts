using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using static ModernFlyouts.NativeMethods;

namespace ModernFlyouts
{
    public static class DUIHandler
    {
        public const int MAX_TRIES = 6;

        public static Point GetCoordinates()
        {
            var defaultPos = new Point(50, 60);


            var hWndHost = GetHost();
            if (hWndHost != IntPtr.Zero & GetWindowRect(hWndHost, out RECT rct))
            {
                Debug.WriteLine($"({rct.Left}, {rct.Top})");
                return new Point(rct.Left, rct.Top);
            }
            Debug.WriteLine("ERROR");
            return defaultPos;
        }

        public static void ForceFindDUIAndHide()
        {
            int tries = 0;

            while (FindDUIAndHide() == false && tries++ < MAX_TRIES)
            {
                //Sometimes the volume flyout isn't created yet, so we need to send some a key stroke.
                keybd_event((byte)KeyInterop.VirtualKeyFromKey(Key.VolumeUp), 0, 0, 0);
                keybd_event((byte)KeyInterop.VirtualKeyFromKey(Key.VolumeDown), 0, 0, 0);

                Thread.Sleep(500);
            }
        }

        public static bool FindDUIAndHide()
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
                        ShowWindowAsync(hWndDUI, (int)ShowWindowCommands.Minimize);
                        ShowWindowAsync(hWndHost, (int)ShowWindowCommands.Hide);
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool FindDUIAndPermanentlyHide()
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
                        ShowWindowAsync(hWndDUI, (int)ShowWindowCommands.Minimize);
                        ShowWindowAsync(hWndHost, (int)ShowWindowCommands.Minimize);
                        ShowWindowAsync(hWndHost, (int)ShowWindowCommands.ForceMinimize);
                        return true;
                    }
                }
            }

            return false;
        }

        public static IntPtr GetHost()
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
                        return hWndHost;
                    }
                }
            }
            return IntPtr.Zero;
        }

        public static (IntPtr Host, int processid) GetAll()
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
                        return (hWndHost, pid);
                    }
                }
            }

            return (IntPtr.Zero, 0);
        }

        public static void FindDUIAndShow()
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
                        ShowWindowAsync(hWndDUI, (int)ShowWindowCommands.Restore);
                        break;
                    }
                }
            }
        }
    }
}