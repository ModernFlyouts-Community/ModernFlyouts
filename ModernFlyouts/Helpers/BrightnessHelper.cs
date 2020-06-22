using System;
using System.Diagnostics;
using System.Management;
using System.Windows.Input;

namespace ModernFlyouts
{
    public class BrightnessHelper : HelperBase
    {
        private BrightnessControl brightnessControl;
        private BrightnessWatcher brightnessWatcher;

        public override event ShowFlyoutEventHandler ShowFlyoutRequested;

        public BrightnessHelper()
        {
            Initialize();
        }

        public void Initialize()
        {
            AlwaysHandleDefaultFlyout = true;

            #region Creating Brightness Control

            brightnessControl = new BrightnessControl();
            brightnessControl.BrightnessSlider.ValueChanged += BrightnessSlider_ValueChanged;

            #endregion

            PrimaryContent = brightnessControl;

            brightnessWatcher = new BrightnessWatcher();

            OnEnabled();
        }

        private void BrightnessSlider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            
        }

        private void BrightnessWatcher_Changed(object sender, BrightnessChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                ShowFlyoutRequested?.Invoke(this);
            });
        }

        protected override void OnEnabled()
        {
            base.OnEnabled();

            Properties.Settings.Default.BrightnessModuleEnabled = IsEnabled;
            Properties.Settings.Default.Save();

            if (IsEnabled)
            {
                brightnessWatcher.Changed += BrightnessWatcher_Changed;
                brightnessWatcher.Start();
            }
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();

            brightnessWatcher.Stop();
            brightnessWatcher.Changed -= BrightnessWatcher_Changed;

            Properties.Settings.Default.BrightnessModuleEnabled = IsEnabled;
            Properties.Settings.Default.Save();
        }

        internal static double GetBrightnessLevel()
        {
            try
            {
                var s = new ManagementScope("root\\WMI");
                var q = new SelectQuery("WmiMonitorBrightness");
                var mos = new ManagementObjectSearcher(s, q);
                var moc = mos.Get();

                foreach (var managementBaseObject in moc)
                {
                    foreach (var o in managementBaseObject.Properties)
                    {
                        if (o.Name == "CurrentBrightness")
                            return Convert.ToInt32(o.Value);
                    }
                }

                moc.Dispose();
                mos.Dispose();
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }

            return 0;
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

    public class BrightnessWatcher
    {
        private ManagementEventWatcher watcher;

        public event EventHandler<BrightnessChangedEventArgs> Changed;
        public BrightnessWatcher()
        {
            try
            {
                var scope = new ManagementScope("root\\WMI");
                var query = new WqlEventQuery("SELECT * FROM WmiMonitorBrightnessEvent");
                watcher = new ManagementEventWatcher(scope, query);
                watcher.EventArrived += new EventArrivedEventHandler(HandleEvent);
                watcher.Start();
            }
            catch (ManagementException managementException)
            {
                Debug.WriteLine($"{nameof(BrightnessWatcher)}: " + managementException.Message);
            }
        }

        public void Start()
        {
            //watcher.Start();
        }

        public void Stop()
        {
            watcher.Stop();
        }

        private void HandleEvent(object sender, EventArrivedEventArgs e)
        {
            int value = int.Parse(e.NewEvent.Properties["Brightness"].Value.ToString());
            Changed?.Invoke(this, new BrightnessChangedEventArgs(value));
        }
    }
}
