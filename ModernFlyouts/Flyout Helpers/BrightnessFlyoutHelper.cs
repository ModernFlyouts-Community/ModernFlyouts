using ModernFlyouts.Core.Utilities;
using ModernFlyouts.Helpers;
using ModernFlyouts.Utilities;
using System;
using System.Diagnostics;
using System.Management;
using System.Windows;
using System.Windows.Controls;

namespace ModernFlyouts
{
    public class BrightnessFlyoutHelper : FlyoutHelperBase
    {
        private BrightnessControl brightnessControl;
        private BrightnessWatcher brightnessWatcher;

        public override event ShowFlyoutEventHandler ShowFlyoutRequested;

        public BrightnessFlyoutHelper()
        {
            Initialize();
        }

        public void Initialize()
        {
            AlwaysHandleDefaultFlyout = true;

            #region Creating Brightness Control

            brightnessControl = new BrightnessControl();
            brightnessControl.BrightnessSlider.ValueChanged += BrightnessSlider_ValueChanged;
            brightnessControl.BrightnessSlider.PreviewMouseWheel += BrightnessSlider_PreviewMouseWheel;

            #endregion

            PrimaryContent = brightnessControl;

            brightnessWatcher = new BrightnessWatcher();

            OnEnabled();
        }

        #region Brightness

        private bool _isInCodeValueChange; //Prevents a LOOP between changing brightness

        private void UpdateBrightness(int brightness)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                brightnessControl.BrightnessGlyph.Glyph = brightness > 50 ? CommonGlyphs.Brightness : CommonGlyphs.LowerBrightness;
                brightnessControl.textVal.Text = brightness.ToString("00");
                _isInCodeValueChange = true;
                brightnessControl.BrightnessSlider.Value = brightness;
                _isInCodeValueChange = false;
            });
        }

        private void BrightnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_isInCodeValueChange)
            {
                var value = Math.Truncate(e.NewValue);
                var oldValue = Math.Truncate(e.OldValue);

                if (value == oldValue)
                {
                    return;
                }

                SetBrightnessLevel((int)value);

                e.Handled = true;
            }
        }

        private void BrightnessSlider_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            var slider = sender as Slider;
            var value = Math.Truncate(slider.Value);
            var change = e.Delta / 120;

            var brightness = value + change;

            if (brightness > 100 || brightness < 0)
            {
                return;
            }

            SetBrightnessLevel((int)brightness);

            e.Handled = true;
        }

        private void BrightnessWatcher_Changed(object sender, BrightnessChangedEventArgs e)
        {
            UpdateBrightness(e.NewValue);
        }

        private static int GetBrightnessLevel()
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
                        {
                            return Convert.ToInt32(o.Value);
                        }
                    }
                }

                moc.Dispose();
                mos.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return 0;
        }

        private static void SetBrightnessLevel(int brightnessLevel)
        {
            if (brightnessLevel < 0 ||
                brightnessLevel > 100)
                throw new ArgumentOutOfRangeException(nameof(brightnessLevel));

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
                        brightnessLevel
                    });
                }

                moc.Dispose();
                mos.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        #endregion

        //This is if it was "updated" using other methods (e.g. Shellhook)
        public void OnExternalUpdated() => ShowFlyoutRequested?.Invoke(this);

        protected override void OnEnabled()
        {
            base.OnEnabled();

            AppDataHelper.BrightnessModuleEnabled = IsEnabled;

            if (IsEnabled)
            {
                brightnessWatcher.Changed += BrightnessWatcher_Changed;
                brightnessWatcher.Start();

                UpdateBrightness(GetBrightnessLevel());
            }
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();

            brightnessWatcher.Stop();
            brightnessWatcher.Changed -= BrightnessWatcher_Changed;

            AppDataHelper.BrightnessModuleEnabled = IsEnabled;
        }
    }
}
