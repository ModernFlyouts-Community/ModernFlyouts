using ModernFlyouts.Controls;
using ModernFlyouts.Core.Display;
using ModernFlyouts.Helpers;

namespace ModernFlyouts
{
    public class BrightnessFlyoutHelper : FlyoutHelperBase
    {
        private BrightnessControl brightnessControl;

        public BrightnessFlyoutHelper()
        {
            Initialize();
        }

        public void Initialize()
        {
            AlwaysHandleDefaultFlyout = true;

            BrightnessManager.Initialize();

            #region Creating Brightness Control

            brightnessControl = new BrightnessControl();
            //brightnessControl.BrightnessSlider.ValueChanged += BrightnessSlider_ValueChanged;
            //brightnessControl.BrightnessSlider.PreviewMouseWheel += BrightnessSlider_PreviewMouseWheel;

            #endregion

            PrimaryContent = brightnessControl;

            OnEnabled();
        }

        #region Brightness

        //private bool _isInCodeValueChange; //Prevents a LOOP between changing brightness

        //private void UpdateBrightness(int brightness)
        //{
        //    Application.Current.Dispatcher.Invoke(() =>
        //    {
        //        brightnessControl.BrightnessGlyph.Glyph = brightness > 50 ? CommonGlyphs.Brightness : CommonGlyphs.LowerBrightness;
        //        brightnessControl.textVal.Text = brightness.ToString("00");
        //        _isInCodeValueChange = true;
        //        brightnessControl.BrightnessSlider.Value = brightness;
        //        _isInCodeValueChange = false;
        //    });
        //}

        //private void BrightnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //    if (!_isInCodeValueChange)
        //    {
        //        var value = Math.Truncate(e.NewValue);
        //        var oldValue = Math.Truncate(e.OldValue);

        //        if (value == oldValue)
        //        {
        //            return;
        //        }

        //        SetBrightnessLevel((int)value);

        //        e.Handled = true;
        //    }
        //}

        //private void BrightnessSlider_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        //{
        //    var slider = sender as Slider;
        //    var value = Math.Truncate(slider.Value);
        //    var change = e.Delta / 120;

        //    var brightness = value + change;

        //    if (brightness > 100 || brightness < 0)
        //    {
        //        return;
        //    }

        //    SetBrightnessLevel((int)brightness);

        //    e.Handled = true;
        //}

        //private int GetBrightnessLevel()
        //{
        //    return (int)(brightnessManager.DefaultBrightnessController?.Brightness ?? 0);
        //}

        //private void SetBrightnessLevel(int brightnessLevel)
        //{
        //    if (brightnessManager.DefaultBrightnessController != null)
        //        brightnessManager.DefaultBrightnessController.Brightness = brightnessLevel;
        //}

        #endregion

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

            //if (IsEnabled)
            //{
            //    UpdateBrightness(GetBrightnessLevel());
            //}
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();

            //brightnessWatcher.Stop();
            //brightnessWatcher.Changed -= BrightnessWatcher_Changed;

            AppDataHelper.BrightnessModuleEnabled = IsEnabled;
        }
    }
}
