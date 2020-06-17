using ModernFlyouts.Utilities;
using System;
using System.Diagnostics;
using System.Management;

namespace ModernFlyouts
{
    public class AirplaneModeHelper : HelperBase
    {
        private AirplaneModeControl airplaneModeControl;

        public override event ShowFlyoutEventHandler ShowFlyoutRequested;

        public AirplaneModeHelper()
        {
            Initialize();
        }

        public void Initialize()
        {
            PrimaryContent = null;
            PrimaryContentVisible = false;

            airplaneModeControl = new AirplaneModeControl();
            
            SecondaryContent = airplaneModeControl;
            SecondaryContentVisible = true;

            var AirplaneModeWatcher = new AirplaneModeWatcher();
            AirplaneModeWatcher.Changed += AirplaneModeWatcher_Changed;
        }

        private void Prepare(AirplaneModeChangedEventArgs e)
        {
            if (e.NotAvailable) 
            {
                airplaneModeControl.txt.Text = "Airplane Mode is not available :(";
                airplaneModeControl.AirplaneGlyph.Glyph = CommonGlyphs.Info;
            }
            if (e.IsEnabled)
            {
                airplaneModeControl.txt.Text = "Airplane mode on";
                airplaneModeControl.AirplaneGlyph.Glyph = CommonGlyphs.Airplane;
            }
            else
            {
                airplaneModeControl.txt.Text = "Airplane mode off";
                airplaneModeControl.AirplaneGlyph.Glyph = CommonGlyphs.SignalBars5;
            }
        }
        private void AirplaneModeWatcher_Changed(object sender, AirplaneModeChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                Prepare(e);
                ShowFlyoutRequested?.Invoke(this, true);
            });
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

                ManagementEventWatcher watcher = new ManagementEventWatcher(query);

                // Set up the delegate that will handle the change event.
                watcher.EventArrived += new EventArrivedEventHandler(HandleEvent);

                // Start listening for events.
                watcher.Start();
            }
            catch (ManagementException managementException)
            {
                Debug.WriteLine($"{nameof(AirplaneModeWatcher)}: " + managementException.Message);
            }
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