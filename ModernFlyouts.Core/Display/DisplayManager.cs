using CommunityToolkit.Mvvm.ComponentModel;
using ModernFlyouts.Core.Interop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Windows;
using static ModernFlyouts.Core.Interop.NativeMethods;

namespace ModernFlyouts.Core.Display
{
    public class DisplayManager : ObservableObject, IWndProcHookHandler
    {
        private const int PRIMARY_MONITOR = unchecked((int)0xBAADF00D);

        private const int MONITORINFOF_PRIMARY = 0x00000001;
        private const int MONITOR_DEFAULTTONEAREST = 0x00000002;

        private static bool multiMonitorSupport;
        private const string defaultDisplayDeviceName = "DISPLAY";

        public static DisplayManager Instance { get; private set; }

        public event EventHandler DisplayUpdated;

        public ObservableCollection<DisplayMonitor> DisplayMonitors { get; } = new();

        private Rect virtualScreenBounds = Rect.Empty;

        public Rect VirtualScreenBounds
        {
            get => virtualScreenBounds;
            private set => SetProperty(ref virtualScreenBounds, value);
        }

        public DisplayMonitor PrimaryDisplayMonitor => DisplayMonitors
            .FirstOrDefault(x => x.IsPrimary);

        private DisplayManager()
        {
            RefreshDisplayMonitorList();
        }

        public static void Initialize()
        {
            Instance = new();
        }

        public uint OnHwndCreated(IntPtr hWnd, out bool register)
        {
            register = false;
            return 0;
        }

        public IntPtr OnWndProc(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            if (msg == (uint)WindowMessage.WM_DISPLAYCHANGE
                || (msg == (uint)WindowMessage.WM_SETTINGCHANGE && wParam == ((IntPtr)SPI_SETWORKAREA)))
            {
                RefreshDisplayMonitorList();
            }
            return IntPtr.Zero;
        }

        public DisplayMonitor GetDisplayMonitorFromHWnd(IntPtr hWnd)
        {
            IntPtr hMonitor = multiMonitorSupport
                ? MonitorFromWindow(new HandleRef(null, hWnd), MONITOR_DEFAULTTONEAREST)
                : (IntPtr)PRIMARY_MONITOR;

            return GetDisplayMonitorFromHMonitor(hMonitor);
        }

        public DisplayMonitor GetDisplayMonitorFromPoint(Point point)
        {
            IntPtr hMonitor;
            if (multiMonitorSupport)
            {
                var pt = new POINTSTRUCT(
                    (int)Math.Round(point.X),
                    (int)Math.Round(point.Y));
                hMonitor = MonitorFromPoint(pt, MONITOR_DEFAULTTONEAREST);
            }
            else
                hMonitor = (IntPtr)PRIMARY_MONITOR;

            return GetDisplayMonitorFromHMonitor(hMonitor);
        }

        private void RefreshDisplayMonitorList()
        {
            multiMonitorSupport = GetSystemMetrics(SM_CMONITORS) != 0;

            var hMonitors = GetHMonitors();

            foreach (var displayMonitor in DisplayMonitors)
            {
                displayMonitor.isStale = true;
            }

            for (int i = 0; i < hMonitors.Count; i++)
            {
                var displayMonitor = GetDisplayMonitorFromHMonitor(hMonitors[i]);
                displayMonitor.Index = i + 1;
            }

            var staleDisplayMonitors = DisplayMonitors
                .Where(x => x.isStale).ToList();
            foreach (var displayMonitor in staleDisplayMonitors)
            {
                displayMonitor.Dispose();
                DisplayMonitors.Remove(displayMonitor);
            }

            staleDisplayMonitors.Clear();
            staleDisplayMonitors = null;

            VirtualScreenBounds = GetVirtualScreenBounds();

            DisplayUpdated?.Invoke(this, EventArgs.Empty);
        }

        private DisplayMonitor GetDisplayMonitorFromHMonitor(IntPtr hMonitor)
        {
            DisplayMonitor displayMonitor = null;

            if (!multiMonitorSupport || hMonitor == (IntPtr)PRIMARY_MONITOR)
            {
                displayMonitor = GetDisplayMonitorByDeviceName(defaultDisplayDeviceName);

                if (displayMonitor == null)
                {
                    displayMonitor = new(defaultDisplayDeviceName);
                    DisplayMonitors.Add(displayMonitor);
                }

                displayMonitor.Bounds = GetVirtualScreenBounds();
                displayMonitor.DeviceId = GetDefaultDisplayDeviceId();
                displayMonitor.DisplayName = "Display";
                displayMonitor.HMonitor = hMonitor;
                displayMonitor.IsPrimary = true;
                displayMonitor.WorkingArea = GetWorkingArea();

                displayMonitor.isStale = false;
                displayMonitor.isDefault = true;
            }
            else
            {
                var info = new MONITORINFOEX();
                GetMonitorInfo(new HandleRef(null, hMonitor), info);

                string deviceName = new string(info.szDevice).TrimEnd((char)0);

                displayMonitor = GetDisplayMonitorByDeviceName(deviceName);

                displayMonitor ??= CreateDisplayMonitorFromMonitorInfo(deviceName);

                displayMonitor.HMonitor = hMonitor;

                UpdateDisplayMonitor(displayMonitor, info);
            }

            return displayMonitor;
        }

        private DisplayMonitor GetDisplayMonitorByDeviceName(string deviceName)
        {
            return DisplayMonitors.FirstOrDefault(x => x.DeviceName == deviceName);
        }

        private DisplayMonitor CreateDisplayMonitorFromMonitorInfo(string deviceName)
        {
            DisplayMonitor displayMonitor = new(deviceName);

            var displayDevice = GetDisplayDevice(deviceName);
            displayMonitor.DeviceId = displayDevice.DeviceID;
            displayMonitor.DisplayName = displayDevice.DeviceString;
            displayMonitor.wmiId = GetWMIDeviceId(displayDevice.DeviceID);
            displayMonitor.IsInBuilt = GetIsDisplayInternal(displayMonitor.wmiId);

            DisplayMonitors.Add(displayMonitor);

            return displayMonitor;
        }

        private void UpdateDisplayMonitor(DisplayMonitor displayMonitor, MONITORINFOEX info)
        {
            displayMonitor.Bounds = new(
                info.rcMonitor.Left, info.rcMonitor.Top,
                info.rcMonitor.Right - info.rcMonitor.Left,
                info.rcMonitor.Bottom - info.rcMonitor.Top);

            displayMonitor.IsPrimary = (info.dwFlags & MONITORINFOF_PRIMARY) != 0;

            displayMonitor.WorkingArea = new(
                info.rcWork.Left, info.rcWork.Top,
                info.rcWork.Right - info.rcWork.Left,
                info.rcWork.Bottom - info.rcWork.Top);

            displayMonitor.isStale = false;
        }

        private IList<IntPtr> GetHMonitors()
        {
            if (multiMonitorSupport)
            {
                List<IntPtr> hMonitors = new();

                bool callback(IntPtr monitor, IntPtr hdc, IntPtr lprcMonitor, IntPtr lParam)
                {
                    hMonitors.Add(monitor);
                    return true;
                }

                EnumDisplayMonitors(new HandleRef(null, IntPtr.Zero), null, callback, IntPtr.Zero);

                return hMonitors;
            }

            return new[] { (IntPtr)PRIMARY_MONITOR };
        }

        private static bool GetIsDisplayInternal(string deviceId)
        {
            try
            {
                var mo = GetManagementObjectForDisplayDevice(deviceId);
                if (mo != null)
                    return Convert.ToInt64(mo.Properties["VideoOutputTechnology"].Value) == 0x80000000;
            }
            catch { }

            return false;
        }

        private static ManagementObject GetManagementObjectForDisplayDevice(string deviceId)
        {
            ManagementObjectSearcher searcher =
                    new ManagementObjectSearcher("root\\WMI",
                    "SELECT * FROM WmiMonitorConnectionParams");
            var managementObjects = searcher.Get();

            foreach (ManagementObject queryObj in managementObjects)
            {
                if (string.Equals(queryObj.Properties["InstanceName"].Value.ToString(), deviceId))
                {
                    return queryObj;
                }
            }

            return null;
        }

        private static string GetWMIDeviceId(string deviceId)
        {
            try
            {
                deviceId = deviceId.Remove(0, 4);
                string[] a = deviceId.Split('#');
                string wmiId = @$"{a[0]}\{a[1]}\{a[2]}_0";
                return wmiId;
            } catch { }

            return string.Empty;
        }

        private static DISPLAY_DEVICE GetDisplayDevice(string deviceName)
        {
            DISPLAY_DEVICE result = new();

            DISPLAY_DEVICE displayDevice = new();
            displayDevice.cb = Marshal.SizeOf(displayDevice);
            try
            {
                for (uint id = 0; EnumDisplayDevices(deviceName, id, ref displayDevice, EDD_GET_DEVICE_INTERFACE_NAME); id++)
                {
                    if (displayDevice.StateFlags.HasFlag(DisplayDeviceStateFlags.AttachedToDesktop)
                        && !displayDevice.StateFlags.HasFlag(DisplayDeviceStateFlags.MirroringDriver))
                    {
                        result = displayDevice;
                        break;
                    }

                    displayDevice.cb = Marshal.SizeOf(displayDevice);
                }
            }
            catch { }

            if (string.IsNullOrEmpty(result.DeviceID)
                || string.IsNullOrWhiteSpace(result.DeviceID))
            {
                result.DeviceID = GetDefaultDisplayDeviceId();
            }

            return result;
        }

        private static string GetDefaultDisplayDeviceId() => GetSystemMetrics(SM_REMOTESESSION) == 0 ?
                    "\\\\?\\DISPLAY#REMOTEDISPLAY#" : "\\\\?\\DISPLAY#LOCALDISPLAY#";

        private static Rect GetVirtualScreenBounds()
        {
            Point location = new(GetSystemMetrics(SM_XVIRTUALSCREEN), GetSystemMetrics(SM_YVIRTUALSCREEN));
            Size size = new(GetSystemMetrics(SM_CXVIRTUALSCREEN), GetSystemMetrics(SM_CYVIRTUALSCREEN));
            return new Rect(location, size);
        }

        private static Rect GetWorkingArea()
        {
            RECT rc = new();
            SystemParametersInfo(SPI_GETWORKAREA, 0, ref rc, 0);
            return new Rect(rc.Left, rc.Top,
                rc.Right - rc.Left, rc.Bottom - rc.Top);
        }
    }
}
