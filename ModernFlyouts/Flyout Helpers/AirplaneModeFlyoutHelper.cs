using ModernFlyouts.Helpers;
using ModernFlyouts.Utilities;
using System;
using System.Diagnostics;
using System.Management;

namespace ModernFlyouts
{
    public class AirplaneModeFlyoutHelper : FlyoutHelperBase
    {
        private AirplaneModeControl airplaneModeControl;
        private AirplaneModeWatcher airplaneModeWatcher;

        public override event ShowFlyoutEventHandler ShowFlyoutRequested;

        public AirplaneModeFlyoutHelper()
        {
            Initialize();
        }

        public void Initialize()
        {
            AlwaysHandleDefaultFlyout = true;

            airplaneModeControl = new AirplaneModeControl();

            PrimaryContent = airplaneModeControl;

            airplaneModeWatcher = new AirplaneModeWatcher();

            OnEnabled();
        }

        private void Prepare(AirplaneModeChangedEventArgs e)
        {
            if (e.NotAvailable)
            {
                airplaneModeControl.txt.SetCurrentValue(System.Windows.Controls.TextBlock.TextProperty, Properties.Strings.AirplaneMode_NotAvailable);
                airplaneModeControl.AirplaneGlyph.SetCurrentValue(ModernWpf.Controls.FontIcon.GlyphProperty, CommonGlyphs.Info);
            }
            if (e.IsEnabled)
            {
                airplaneModeControl.txt.SetCurrentValue(System.Windows.Controls.TextBlock.TextProperty, Properties.Strings.AirplaneModeOn);
                airplaneModeControl.AirplaneGlyph.SetCurrentValue(ModernWpf.Controls.FontIcon.GlyphProperty, CommonGlyphs.Airplane);
            }
            else
            {
                airplaneModeControl.txt.SetCurrentValue(System.Windows.Controls.TextBlock.TextProperty, Properties.Strings.AirplaneModeOff);
                airplaneModeControl.AirplaneGlyph.SetCurrentValue(ModernWpf.Controls.FontIcon.GlyphProperty, CommonGlyphs.MobSignal5);
            }
        }

        private void AirplaneModeWatcher_Changed(object sender, AirplaneModeChangedEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                Prepare(e);
                ShowFlyoutRequested?.Invoke(this);
            });
        }

        protected override void OnEnabled()
        {
            base.OnEnabled();

            AppDataHelper.AirplaneModeModuleEnabled = IsEnabled;

            if (IsEnabled)
            {
                airplaneModeWatcher.Changed += AirplaneModeWatcher_Changed;
                airplaneModeWatcher.Start();
            }
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();

            airplaneModeWatcher.Stop();
            airplaneModeWatcher.Changed -= AirplaneModeWatcher_Changed;

            AppDataHelper.AirplaneModeModuleEnabled = IsEnabled;
        }
    }

    public class AirplaneModeChangedEventArgs : EventArgs
    {
        public AirplaneModeChangedEventArgs(bool enabled, bool notAvail)
        {
            IsEnabled = enabled;
            NotAvailable = notAvail;
        }

        public bool IsEnabled { get; set; }

        public bool NotAvailable { get; set; }
    }

    public class AirplaneModeWatcher
    {
        private readonly ManagementEventWatcher watcher;

        public event EventHandler<AirplaneModeChangedEventArgs> Changed;

        public AirplaneModeWatcher()
        {
            try
            {
                if (GetAirplaneMode() == -1) { Debug.WriteLine("Nope!"); return; }
                WqlEventQuery query = new WqlEventQuery(
                     "SELECT * FROM RegistryValueChangeEvent WHERE " +
                     "Hive = 'HKEY_LOCAL_MACHINE'" +
                     @"AND KeyPath = 'SYSTEM\\CurrentControlSet\\Control\\RadioManagement\\SystemRadioState' AND ValueName=''");

                watcher = new ManagementEventWatcher(query);
                watcher.EventArrived += new EventArrivedEventHandler(HandleEvent);
            }
            catch (ManagementException managementException)
            {
                Debug.WriteLine($"{nameof(AirplaneModeWatcher)}: " + managementException.Message);
            }
        }

        public void Start()
        {
            watcher.Start();
        }

        public void Stop()
        {
            watcher.Stop();
        }

        public static int GetAirplaneMode()
        {
            var regkey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\RadioManagement\SystemRadioState", false);
            if (regkey == null) return -1;
            var intValue = (int)regkey.GetValue("", 1);
            return intValue;
        }

        private void HandleEvent(object sender, EventArrivedEventArgs e)
        {
            int value = GetAirplaneMode();
            Changed?.Invoke(this, new AirplaneModeChangedEventArgs(value == 1, value == -1));
        }
    }
}