using ModernFlyouts.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management;
using System.Windows;
using static ModernFlyouts.Core.Interop.NativeMethods;

namespace ModernFlyouts.Core.Display
{
    public class BrightnessManager
    {
        private BrightnessWatcher brightnessWatcher;
        private byte[] validWMIBrightnessLevels;

        public BrightnessController DefaultBrightnessController { get; private set; }

        public static BrightnessManager Instance { get; private set; }

        public ObservableCollection<BrightnessController> BrightnessControllers { get; } = new();

        private BrightnessManager()
        {
            validWMIBrightnessLevels = GetValidWMIBrightnessLevels();

            // No physical monitors imply that this display could be
            // a built-in display such as for laptops/tablets.
            // So, we are using the WMI Methods to control the brightness of it.
            if (AreWMIMethodsSupported())
            {
                DefaultBrightnessController = new BuiltInDisplayBrightnessController(validWMIBrightnessLevels);
                BrightnessControllers.Add(DefaultBrightnessController);
                brightnessWatcher = new();
                brightnessWatcher.Changed += BrightnessWatcher_Changed;
                brightnessWatcher.Start();
            }

            BrightnessControllers.Add(new MockBrightnessController());
            BrightnessControllers.Add(new MockBrightnessController());
            BrightnessControllers.Add(new MockBrightnessController());
            BrightnessControllers.Add(new MockBrightnessController());
            BrightnessControllers.Add(new MockBrightnessController());
            BrightnessControllers.Add(new MockBrightnessController());
            BrightnessControllers.Add(new MockBrightnessController());
            BrightnessControllers.Add(new MockBrightnessController());
            BrightnessControllers.Add(new MockBrightnessController());
        }

        public static void Initialize()
        {
            Instance = new();
        }

        private static BrightnessController[] CreateBrightnessControllersForDisplayMonitor(DisplayMonitor displayMonitor)
        {
            List<BrightnessController> brightnessControllers = new();

            uint physicalMonitorsCount = 0;
            if (!GetNumberOfPhysicalMonitorsFromHMONITOR(displayMonitor.HMonitor, ref physicalMonitorsCount))
            {
                return null;
            }

            var physicalMonitors = new PHYSICAL_MONITOR[physicalMonitorsCount];
            if (!GetPhysicalMonitorsFromHMONITOR(displayMonitor.HMonitor, physicalMonitorsCount, physicalMonitors))
            {
                return null;
            }

            foreach (var physicalMonitor in physicalMonitors)
            {
                uint minValue = 0, currentValue = 0, maxValue = 0;
                if (!GetMonitorBrightness(physicalMonitor.hPhysicalMonitor, ref minValue, ref currentValue, ref maxValue))
                {
                    DestroyPhysicalMonitor(physicalMonitor.hPhysicalMonitor);
                    continue;
                }

                MonitorInfo info = new()
                {
                    Handle = physicalMonitor.hPhysicalMonitor,
                    MinValue = minValue,
                    CurrentValue = currentValue,
                    MaxValue = maxValue,
                };

                brightnessControllers.Add(new ExternalDisplayBrightnessController(info, displayMonitor));
            }

            if (brightnessControllers.Count == 0)
                return null;

            return brightnessControllers.ToArray();
        }

        internal static void DisposeBrightnessControllerForDisplayMonitor(DisplayMonitor displayMonitor)
        {
            if (Instance == null)
                return;

            var brightnessControllers = Instance.BrightnessControllers.Where(
                x => x is ExternalDisplayBrightnessController ex && ex.AssociatedDisplayMonitor == displayMonitor).ToList();

            if (brightnessControllers != null)
            {
                foreach (var brightnessController in brightnessControllers)
                {
                    brightnessController.Dispose();
                    Instance.BrightnessControllers.Remove(brightnessController);
                }
            }
        }

        internal static void MakeBrightnessControllersForDisplayMonitor(DisplayMonitor displayMonitor)
        {
            if (Instance == null)
                return;

            DisposeBrightnessControllerForDisplayMonitor(displayMonitor);

            var brightnessControllers = CreateBrightnessControllersForDisplayMonitor(displayMonitor);

            if (brightnessControllers != null)
            {
                foreach (var brightnessController in brightnessControllers)
                {
                    Instance.BrightnessControllers.Add(brightnessController);
                }
            }
        }

        private bool AreWMIMethodsSupported()
        {
            return validWMIBrightnessLevels.Length > 0;
        }

        private byte[] GetValidWMIBrightnessLevels()
        {
            byte[] brightnessLevels = Array.Empty<byte>();

            try
            {
                var s = new ManagementScope("root\\WMI");
                var q = new SelectQuery("WmiMonitorBrightness");
                using var mos = new ManagementObjectSearcher(s, q);
                using var moc = mos.Get();

                foreach (ManagementObject managementObject in moc)
                {
                    brightnessLevels = (byte[])managementObject.GetPropertyValue("Level");
                    break;
                }
            }
            catch { }

            return brightnessLevels;
        }

        private void BrightnessWatcher_Changed(object sender, BrightnessChangedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (DefaultBrightnessController != null)
                    DefaultBrightnessController.UpdateBrightness(e.NewValue);
            });
        }
    }
}
