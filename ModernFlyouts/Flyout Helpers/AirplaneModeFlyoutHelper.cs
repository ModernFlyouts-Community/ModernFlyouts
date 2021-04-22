using ModernFlyouts.Helpers;
using ModernFlyouts.Utilities;

namespace ModernFlyouts
{
    public class AirplaneModeFlyoutHelper : FlyoutHelperBase
    {
        private AirplaneModeControl airplaneModeControl;

        public AirplaneModeFlyoutHelper()
        {
            Initialize();
        }

        public void Initialize()
        {
            AlwaysHandleDefaultFlyout = true;

            airplaneModeControl = new AirplaneModeControl();

            PrimaryContent = airplaneModeControl;

            OnEnabled();
        }

        private void Prepare(object data)
        {
            if (data is bool isEnabled && isEnabled)
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

        public override bool CanHandleNativeOnScreenFlyout(FlyoutTriggerData triggerData)
        {
            if (triggerData.TriggerType == FlyoutTriggerType.AirplaneMode)
            {
                Prepare(triggerData.Data);
                return true;
            }

            return base.CanHandleNativeOnScreenFlyout(triggerData);
        }

        protected override void OnEnabled()
        {
            base.OnEnabled();

            AppDataHelper.AirplaneModeModuleEnabled = IsEnabled;
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();

            AppDataHelper.AirplaneModeModuleEnabled = IsEnabled;
        }
    }
}
