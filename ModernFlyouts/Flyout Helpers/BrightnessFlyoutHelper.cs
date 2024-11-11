using ModernFlyouts.Controls;
using ModernFlyouts.Core.Display;
using ModernFlyouts.Helpers;

namespace ModernFlyouts
{
    public class BrightnessFlyoutHelper : FlyoutHelperBase
    {
        private BrightnessControl brightnessControl;

        private bool compatibilityMode = false;
        public bool CompatibilityMode
        {
            get { return compatibilityMode; }
            set { CompatibilityChanged(value); }
        }

        public BrightnessFlyoutHelper()
        {
            Initialize();
        }

        public void Initialize()
        {
            AlwaysHandleDefaultFlyout = true;

            BrightnessManager.Initialize(compatibilityMode);

            brightnessControl = new BrightnessControl();

            PrimaryContent = brightnessControl;

            OnEnabled();
        }

        public override bool CanHandleNativeOnScreenFlyout(FlyoutTriggerData triggerData)
        {
            if (triggerData.TriggerType == FlyoutTriggerType.Brightness)
                return true;

            return base.CanHandleNativeOnScreenFlyout(triggerData);
        }

        protected override void OnEnabled()
        {
            base.OnEnabled();

            AppDataHelper.BrightnessModuleEnabled = IsEnabled;

            BrightnessManager.Initialize(compatibilityMode);
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();

            BrightnessManager.Suspend();

            AppDataHelper.BrightnessModuleEnabled = IsEnabled;
        }

        public void CompatibilityChanged(bool mode)
        {
            compatibilityMode = mode;
            BrightnessManager.Instance.CompatibilityMode = compatibilityMode;
        }
    }
}
