﻿using ModernWpf.Controls;
using System;
using System.Diagnostics;
using System.Management;

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

            #endregion

            PrimaryContent = brightnessControl;

            brightnessWatcher = new BrightnessWatcher();

            OnEnabled();
        }

        #region Brightness

        private bool _isInCodeValueChange = false; //Prevents a LOOP between changing brightness

        private void UpdateBrightness(int brightness)
        {
            brightnessControl.Dispatcher.Invoke(() =>
            {
                UpdateBrightnessGlyph(brightness);
                _isInCodeValueChange = true;
                brightnessControl.BrightnessSlider.Value = brightness;
                _isInCodeValueChange = false;
                brightnessControl.textVal.Text = brightness.ToString("00");
                if (!Classes.WindowsInfo.IsWindows8OrLater())
                    ShowFlyoutRequested?.Invoke(this);
            });
        }

        private void UpdateBrightnessGlyph(int brightness)
        {
            if (brightness > 90.0)
            {
                brightnessControl.BrightnessGlyph.SetResourceReference(IconElement.ForegroundProperty, "SystemControlErrorTextForegroundBrush");
            }
            else
            {
                brightnessControl.BrightnessGlyph.SetResourceReference(IconElement.ForegroundProperty, "SystemControlForegroundBaseHighBrush");
            }
        }

        private void BrightnessSlider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
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

        private void BrightnessWatcher_Changed(object sender, BrightnessChangedEventArgs e)
        {
            UpdateBrightness(e.NewValue);
        }

        private int GetBrightnessLevel()
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
            catch (Exception ex) { Debug.WriteLine(ex.Message); }

            return 0;
        }

        private void SetBrightnessLevel(int brightnessLevel)
        {
            if (brightnessLevel < 0 ||
                brightnessLevel > 100)
                throw new ArgumentOutOfRangeException("brightnessLevel");

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
            catch (Exception ex) { Debug.WriteLine(ex.ToString()); }
        }

        #endregion

        //This is if it was "updated" using other methods (e.g. Shellhook)
        public void OnExternalUpdated() => ShowFlyoutRequested?.Invoke(this);


        protected override void OnEnabled()
        {
            base.OnEnabled();

            Properties.Settings.Default.BrightnessModuleEnabled = IsEnabled;
            Properties.Settings.Default.Save();

            if (IsEnabled)
            {
                brightnessWatcher.Changed += BrightnessWatcher_Changed;
                brightnessWatcher.Start();

                _isInCodeValueChange = true;
                brightnessControl.BrightnessSlider.Value = GetBrightnessLevel();
                _isInCodeValueChange = false;
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
}
