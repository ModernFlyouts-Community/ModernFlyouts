using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Windows;

namespace ModernFlyouts.Core.Display
{
    public class DisplayMonitor : ObservableObject, IDisposable
    {
        internal bool isDefault;
        internal bool isStale;
        internal string wmiId;

        #region Properties

        private Rect bounds = Rect.Empty;

        public Rect Bounds
        {
            get => bounds;
            internal set => SetProperty(ref bounds, value);
        }

        private string deviceId = string.Empty;

        public string DeviceId
        {
            get => deviceId;
            internal set => SetProperty(ref deviceId, value);
        }

        public string DeviceName { get; init; }

        private string displayName = string.Empty;

        public string DisplayName
        {
            get => displayName;
            internal set => SetProperty(ref displayName, value);
        }

        private IntPtr hMonitor = IntPtr.Zero;

        public IntPtr HMonitor
        {
            get => hMonitor;
            internal set
            {
                if (SetProperty(ref hMonitor, value))
                {
                    OnHMonitorUpdated();
                }
            }
        }

        private int index;

        public int Index
        {
            get => index;
            internal set => SetProperty(ref index, value);
        }

        private bool isPrimary;

        public bool IsPrimary
        {
            get => isPrimary;
            internal set => SetProperty(ref isPrimary, value);
        }

        private bool isInBuilt;

        public bool IsInBuilt
        {
            get => isInBuilt;
            internal set => SetProperty(ref isInBuilt, value);
        }

        private Rect workingArea = Rect.Empty;

        public Rect WorkingArea
        {
            get => workingArea;
            internal set => SetProperty(ref workingArea, value);
        }

        #endregion

        internal DisplayMonitor(string deviceName)
        {
            DeviceName = deviceName;
        }

        internal void OnHMonitorUpdated()
        {
            BrightnessManager.MakeBrightnessControllersForDisplayMonitor(this);
        }

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                BrightnessManager.DisposeBrightnessControllerForDisplayMonitor(this);

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
