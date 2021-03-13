using ModernFlyouts.Core.Utilities;
using ModernFlyouts.Helpers;
using ModernFlyouts.Utilities;
using System.Windows;

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
            if (e.IsEnabled)
            {
                airplaneModeControl.txt.Text = Properties.Strings.AirplaneModeOn;
                airplaneModeControl.AirplaneGlyph.Glyph = CommonGlyphs.Airplane;
            }
            else
            {
                airplaneModeControl.txt.Text = Properties.Strings.AirplaneModeOff;
                airplaneModeControl.AirplaneGlyph.Glyph = CommonGlyphs.SignalBars;
            }
        }

        private void AirplaneModeWatcher_Changed(object sender, AirplaneModeChangedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
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
}
