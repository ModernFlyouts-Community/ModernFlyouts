using System;
using System.Management;
using CommunityToolkit.Mvvm.ComponentModel;
using ModernFlyouts.Core.Interop;

namespace ModernFlyouts.Core.Display
{
    public abstract class BrightnessController : ObservableObject, IDisposable
    {
        #region Properties

        private DisplayMonitor associatedDisplayMonitor;

        public DisplayMonitor AssociatedDisplayMonitor
        {
            get => associatedDisplayMonitor;
            internal set => SetProperty(ref associatedDisplayMonitor, value);
        }

        private double minimum = 0.0;

        public double Minimum
        {
            get => minimum;
            protected set => SetProperty(ref minimum, value);
        }

        private double maximum = 100.0;

        public double Maximum
        {
            get => maximum;
            protected set => SetProperty(ref maximum, value);
        }

        private double brightness = 0.0;

        public double Brightness
        {
            get => brightness;
            set
            {
                value = Math.Min(Math.Max(minimum, value), maximum);
                if (SetProperty(ref brightness, value))
                {
                    SetBrightness(value);
                }
            }
        }

        #endregion

        public BrightnessController()
        {
            Brightness = GetBrightness();
        }

        internal abstract double GetBrightness();

        internal abstract void SetBrightness(double value);

        internal void UpdateBrightness(double value)
        {
            value = Math.Min(Math.Max(minimum, value), maximum);
            SetProperty(ref brightness, value, nameof(Brightness));
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        ~BrightnessController()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    public class MockBrightnessController : BrightnessController
    {
        internal override double GetBrightness()
        {
            return new Random().NextDouble() * 100.0;
        }

        internal override void SetBrightness(double value)
        {
        }
    }

    public class BuiltInDisplayBrightnessController : BrightnessController
    {
        internal BuiltInDisplayBrightnessController(byte[] levels)
        {
            Maximum = levels.Length - 1;
            Minimum = levels[0];
        }

        internal override double GetBrightness()
        {
            try
            {
                var s = new ManagementScope("root\\WMI");
                var q = new SelectQuery("WmiMonitorBrightness");
                var mos = new ManagementObjectSearcher(s, q);
                var moc = mos.Get();

                foreach (var managementBaseObject in moc)
                {
                    return Convert.ToDouble(managementBaseObject.GetPropertyValue("CurrentBrightness"));
                }

                moc.Dispose();
                mos.Dispose();
            }
            catch { }

            return 0.0;
        }

        internal override void SetBrightness(double value)
        {
            try
            {
                var s = new ManagementScope("root\\WMI");
                var q = new SelectQuery("WmiMonitorBrightnessMethods");
                var mos = new ManagementObjectSearcher(s, q);
                var moc = mos.Get();

                foreach (var managementBaseObject in moc)
                {
                    var o = (ManagementObject)managementBaseObject;
                    o.InvokeMethod("WmiSetBrightness", new object[]
                    {
                        uint.MaxValue,
                        (int)Math.Truncate(value)
                    });
                }

                moc.Dispose();
                mos.Dispose();
            }
            catch { }
        }
    }

    public class ExternalDisplayBrightnessController : BrightnessController
    {
        private IntPtr hPhysicalMonitor;
        private double currentValue;

        internal ExternalDisplayBrightnessController(MonitorInfo info, DisplayMonitor displayMonitor)
        {
            Maximum = info.MaxValue;
            Minimum = info.MinValue;
            currentValue = info.CurrentValue;
            hPhysicalMonitor = info.Handle;
            Brightness = currentValue;

            AssociatedDisplayMonitor = displayMonitor;
        }

        internal override double GetBrightness()
        {
            return currentValue;
        }

        internal override void SetBrightness(double value)
        {
            if (NativeMethods.SetMonitorBrightness(hPhysicalMonitor, (uint)Math.Truncate(value)))
            {
                currentValue = value;
            }
        }

        private bool disposedValue;

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposedValue)
            {
                AssociatedDisplayMonitor = null;
                NativeMethods.DestroyPhysicalMonitor(hPhysicalMonitor);

                disposedValue = true;
            }
        }
    }

    internal class MonitorInfo
    {
        public uint MinValue { get; set; }
        public uint MaxValue { get; set; }
        public IntPtr Handle { get; set; }
        public uint CurrentValue { get; set; }
    }
}
