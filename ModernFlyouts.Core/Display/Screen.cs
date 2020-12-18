using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using static ModernFlyouts.Core.Interop.NativeMethods;

namespace ModernFlyouts.Core.Display
{
    public class Screen
    {
        private readonly IntPtr hmonitor;

        private const int PRIMARY_MONITOR = unchecked((int)0xBAADF00D);

        private const int MONITORINFOF_PRIMARY = 0x00000001;
        private const int MONITOR_DEFAULTTONEAREST = 0x00000002;

        private static bool multiMonitorSupport;

        static Screen()
        {
            multiMonitorSupport = GetSystemMetrics(SM_CMONITORS) != 0;
        }

        private Screen(IntPtr monitor)
            : this(monitor, IntPtr.Zero)
        {
        }

        private Screen(IntPtr monitor, IntPtr hdc)
        {
            if (!multiMonitorSupport || monitor == (IntPtr)PRIMARY_MONITOR)
            {
                Bounds = VirtualScreen;
                IsPrimary = true;
                DeviceName = "DISPLAY";
            }
            else
            {
                var info = new MONITORINFOEX();

                GetMonitorInfo(new HandleRef(null, monitor), info);

                Bounds = new(
                    info.rcMonitor.Left, info.rcMonitor.Top,
                    info.rcMonitor.Right - info.rcMonitor.Left,
                    info.rcMonitor.Bottom - info.rcMonitor.Top);

                IsPrimary = ((info.dwFlags & MONITORINFOF_PRIMARY) != 0);

                DeviceName = new string(info.szDevice).TrimEnd((char)0);
            }
            hmonitor = monitor;
        }

        public static IEnumerable<Screen> AllScreens
        {
            get
            {
                if (multiMonitorSupport)
                {
                    var closure = new MonitorEnumCallback();
                    var proc = new MonitorEnumProc(closure.Callback);
                    EnumDisplayMonitors(new HandleRef(null, IntPtr.Zero), null, proc, IntPtr.Zero);

                    if (closure.Screens.Count > 0)
                    {
                        return closure.Screens.Cast<Screen>();
                    }
                }
                return new[] { new Screen((IntPtr)PRIMARY_MONITOR) };
            }
        }

        public Rect Bounds { get; private set; }

        public string DeviceName { get; private set; }

        public bool IsPrimary { get; private set; }

        public static Screen PrimaryScreen
        {
            get
            {
                if (multiMonitorSupport)
                {
                    return AllScreens.FirstOrDefault(t => t.IsPrimary);
                }
                return new Screen((IntPtr)PRIMARY_MONITOR);
            }
        }

        public Rect WorkingArea
        {
            get
            {
                if (!multiMonitorSupport || hmonitor == (IntPtr)PRIMARY_MONITOR)
                {
                    RECT rc = new();
                    SystemParametersInfo(SPI_GETWORKAREA, 0, ref rc, 0);
                    return new Rect(rc.Left, rc.Top,
                                    rc.Right - rc.Left,
                                    rc.Bottom - rc.Top);
                }

                var info = new MONITORINFOEX();
                GetMonitorInfo(new HandleRef(null, hmonitor), info);

                return new Rect(
                    info.rcWork.Left, info.rcWork.Top,
                    info.rcWork.Right - info.rcWork.Left,
                    info.rcWork.Bottom - info.rcWork.Top);
            }
        }

        public static Screen FromWindow(IntPtr hwnd)
        {
            if (multiMonitorSupport)
            {
                return new Screen(MonitorFromWindow(new HandleRef(null, hwnd), 2));
            }
            return new Screen((IntPtr)PRIMARY_MONITOR);
        }

        public static Screen FromPoint(Point point)
        {
            if (multiMonitorSupport)
            {
                var pt = new POINTSTRUCT((int)point.X, (int)point.Y);
                return new Screen(MonitorFromPoint(pt, MONITOR_DEFAULTTONEAREST));
            }
            return new Screen((IntPtr)PRIMARY_MONITOR);
        }

        public override bool Equals(object obj)
        {
            if (obj is Screen screen)
            {
                if (hmonitor == screen.hmonitor)
                {
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (int)hmonitor;
        }

        private class MonitorEnumCallback
        {
            public ArrayList Screens { get; private set; }

            public MonitorEnumCallback()
            {
                Screens = new ArrayList();
            }

            public bool Callback(IntPtr monitor, IntPtr hdc, IntPtr lprcMonitor, IntPtr lparam)
            {
                Screens.Add(new Screen(monitor, hdc));
                return true;
            }
        }

        private static Rect VirtualScreen
        {
            get
            {
                Size size = new(GetSystemMetrics(SM_CXSCREEN), GetSystemMetrics(SM_CYSCREEN));
                return new Rect(0, 0, size.Width, size.Height);
            }
        }
    }
}
