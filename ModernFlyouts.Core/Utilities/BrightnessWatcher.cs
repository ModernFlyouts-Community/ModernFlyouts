using System;
using System.Diagnostics;
using System.Management;

namespace ModernFlyouts.Core.Utilities
{
    public class BrightnessWatcher
    {
        private readonly ManagementEventWatcher watcher;

        public event EventHandler<BrightnessChangedEventArgs> Changed;

        public BrightnessWatcher()
        {
            try
            {
                var scope = new ManagementScope("root\\WMI");
                var query = new WqlEventQuery("SELECT * FROM WmiMonitorBrightnessEvent");
                watcher = new ManagementEventWatcher(scope, query);
                watcher.EventArrived += new EventArrivedEventHandler(HandleEvent);
            }
            catch (ManagementException managementException)
            {
                Debug.WriteLine($"{nameof(BrightnessWatcher)}: " + managementException.Message);
            }
        }

        public void Start()
        {
            try
            {
                watcher.Start();
            }
            catch (ManagementException managementException)
            {
                Debug.WriteLine($"{nameof(BrightnessWatcher)}: " + managementException.Message);
            }
        }

        public void Stop()
        {
            try
            {
                watcher.Stop();
            }
            catch (ManagementException managementException)
            {
                Debug.WriteLine($"{nameof(BrightnessWatcher)}: " + managementException.Message);
            }
        }

        private void HandleEvent(object sender, EventArrivedEventArgs e)
        {
            int value = int.Parse(e.NewEvent.Properties["Brightness"].Value.ToString());
            Changed?.Invoke(this, new BrightnessChangedEventArgs(value));
        }
    }

    public class BrightnessChangedEventArgs : EventArgs
    {
        public BrightnessChangedEventArgs(int newValue)
        {
            NewValue = newValue;
        }

        public int NewValue { get; set; }
    }
}
