using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using Windows.Storage;

namespace ModernFlyouts
{
    public class AppDataHelper
    {
        #region Methods

        private static bool GetBool(bool defaultValue, [CallerMemberName] string propertyName = "")
        {
            try
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey(propertyName))
                {
                    object value = ApplicationData.Current.LocalSettings.Values[propertyName];
                    if (bool.TryParse(value.ToString(), out bool result))
                    {
                        return result;
                    }
                }
            }
            catch { }

            return defaultValue;
        }

        private static void SetBool(bool value, [CallerMemberName] string propertyName = "")
        {
            try
            {
                ApplicationData.Current.LocalSettings.Values[propertyName] = value.ToString();
            }
            catch { }
        }

        private static int GetInteger(int defaultValue, [CallerMemberName] string propertyName = "")
        {
            try
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey(propertyName))
                {
                    object value = ApplicationData.Current.LocalSettings.Values[propertyName];
                    if (int.TryParse(value.ToString(), out int result))
                    {
                        return result;
                    }
                }
            }
            catch { }

            return defaultValue;
        }

        private static void SetInteger(int value, [CallerMemberName] string propertyName = "")
        {
            try
            {
                ApplicationData.Current.LocalSettings.Values[propertyName] = value;
            }
            catch { }
        }

        private static double GetDouble(double defaultValue, [CallerMemberName] string propertyName = "")
        {
            try
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey(propertyName))
                {
                    object value = ApplicationData.Current.LocalSettings.Values[propertyName];
                    if (double.TryParse(value.ToString(), out double result))
                    {
                        return result;
                    }
                }
            }
            catch { }

            return defaultValue;
        }

        private static void SetDouble(double value, [CallerMemberName] string propertyName = "")
        {
            try
            {
                ApplicationData.Current.LocalSettings.Values[propertyName] = value;
            }
            catch { }
        }

        private static string GetString([CallerMemberName] string propertyName = "")
        {
            try
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey(propertyName))
                {
                    object value = ApplicationData.Current.LocalSettings.Values[propertyName];
                    return value.ToString();
                }
            }
            catch { }

            return string.Empty;
        }

        private static void SetString(string value, [CallerMemberName] string propertyName = "")
        {
            try
            {
                ApplicationData.Current.LocalSettings.Values[propertyName] = value;
            }
            catch { }
        }

        private static T GetEnum<T>(T defaultValue, [CallerMemberName] string propertyName = "") where T : Enum
        {
            try
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey(propertyName))
                {
                    object value = ApplicationData.Current.LocalSettings.Values[propertyName];
                    return (T)Enum.Parse(typeof(T), value.ToString());
                }
            }
            catch { }

            return defaultValue;
        }

        private static void SetEnum<T>(T value, [CallerMemberName] string propertyName = "") where T : Enum
        {
            try
            {
                ApplicationData.Current.LocalSettings.Values[propertyName] = value.ToString();
            }
            catch { }
        }

        private static Point GetPoint(Point defaultPoint, [CallerMemberName] string propertyName = "")
        {
            try
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey(propertyName))
                {
                    object value = ApplicationData.Current.LocalSettings.Values[propertyName];
                    return Point.Parse(value.ToString());
                }
            }
            catch { }

            return defaultPoint;
        }

        private static void SetPoint(Point value, [CallerMemberName] string propertyName = "")
        {
            try
            {
                ApplicationData.Current.LocalSettings.Values[propertyName] = value.ToString();
            }
            catch { }
        }

        #endregion

        #region Properties

        public static bool AudioModuleEnabled
        {
            get => GetBool(DefaultValuesStore.AudioModuleEnabled);
            set => SetBool(value);
        }

        public static bool ShowGSMTCInVolumeFlyout
        {
            get => GetBool(DefaultValuesStore.ShowGSMTCInVolumeFlyout);
            set => SetBool(value);
        }

        public static bool ShowVolumeControlInGSMTCFlyout
        {
            get => GetBool(DefaultValuesStore.ShowVolumeControlInGSMTCFlyout);
            set => SetBool(value);
        }

        public static bool BrightnessModuleEnabled
        {
            get => GetBool(DefaultValuesStore.BrightnessModuleEnabled);
            set => SetBool(value);
        }

        public static bool AirplaneModeModuleEnabled
        {
            get => GetBool(DefaultValuesStore.AirplaneModeModuleEnabled);
            set => SetBool(value);
        }

        public static bool LockKeysModuleEnabled
        {
            get => GetBool(DefaultValuesStore.LockKeysModuleEnabled);
            set => SetBool(value);
        }

        public static DefaultFlyout DefaultFlyout
        {
            get => GetEnum(DefaultValuesStore.PreferredFlyout);
            set => SetEnum(value);
        }

        public static bool TopBarEnabled
        {
            get => GetBool(DefaultValuesStore.TopBarEnabled);
            set => SetBool(value);
        }

        public static Point DefaultFlyoutPosition
        {
            get => GetPoint(DefaultValuesStore.DefaultFlyoutPosition);
            set => SetPoint(value);
        }

        public static Point FlyoutPosition
        {
            get => GetPoint(DefaultValuesStore.DefaultFlyoutPosition);
            set => SetPoint(value);
        }

        public static string SettingsWindowPlacement
        {
            get => GetString();
            set => SetString(value);
        }

        public static int FlyoutTimeout
        {
            get => GetInteger(DefaultValuesStore.FlyoutTimeout);
            set => SetInteger(value);
        }

        public static double FlyoutBackgroundOpacity
        {
            get => GetDouble(DefaultValuesStore.FlyoutBackgroundOpacity);
            set => SetDouble(value);
        }

        public static bool UseColoredTrayIcon
        {
            get => GetBool(DefaultValuesStore.UseColoredTrayIcon);
            set => SetBool(value);
        }

        public static int MaxVerticalSessionControlsCount
        {
            get => GetInteger(DefaultValuesStore.MaxVerticalSessionControlsCount);
            set => SetInteger(value);
        }

        public static Orientation SessionsPanelOrientation
        {
            get => GetEnum(DefaultValuesStore.SessionsPanelOrientation);
            set => SetEnum(value);
        }

        public static string Language
        {
            get => GetString();
            set => SetString(value);
        }

        #endregion
    }
}
