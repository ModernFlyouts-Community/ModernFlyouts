using System.Windows;
using Windows.Storage;

namespace ModernFlyouts
{
    public class AppDataHelper
    {
        private static bool GetBool(string propertyName, bool defaultValue)
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
            } catch { }

            return defaultValue;
        }

        private static void SetBool(string propertyName, bool value)
        {
            try
            {
                ApplicationData.Current.LocalSettings.Values[propertyName] = value.ToString();
            }
            catch { }
        }

        private static string GetString(string propertyName)
        {
            try
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey(propertyName))
                {
                    object value = ApplicationData.Current.LocalSettings.Values[propertyName];
                    return value.ToString();
                }
            } catch { }

            return string.Empty;
        }

        private static void SetString(string propertyName, string value)
        {
            try
            {
                ApplicationData.Current.LocalSettings.Values[propertyName] = value;
            } catch { }
        }

        private static Point GetPoint(string propertyName, Point defaultPoint)
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

        private static void SetPoint(string propertyName, Point value)
        {
            try
            {
                ApplicationData.Current.LocalSettings.Values[propertyName] = value.ToString();
            }
            catch { }
        }

        public static bool AudioModuleEnabled
        {
            get => GetBool(nameof(AudioModuleEnabled), true);
            set => SetBool(nameof(AudioModuleEnabled), value);
        }
        
        public static bool AirplaneModeModuleEnabled
        {
            get => GetBool(nameof(AirplaneModeModuleEnabled), true);
            set => SetBool(nameof(AirplaneModeModuleEnabled), value);
        }
        
        public static bool LockKeysModuleEnabled
        {
            get => GetBool(nameof(LockKeysModuleEnabled), true);
            set => SetBool(nameof(LockKeysModuleEnabled), value);
        }
        
        public static bool BrightnessModuleEnabled
        {
            get => GetBool(nameof(BrightnessModuleEnabled), true);
            set => SetBool(nameof(BrightnessModuleEnabled), value);
        }

        public static string DefaultFlyout
        {
            get => GetString(nameof(DefaultFlyout));
            set => SetString(nameof(DefaultFlyout), value);
        }

        public static bool TopBarEnabled
        {
            get => GetBool(nameof(TopBarEnabled), true);
            set => SetBool(nameof(TopBarEnabled), value);
        }

        private static Point defaultFlyoutPosition = new Point(50, 60);

        public static Point DefaultFlyoutPosition
        {
            get => GetPoint(nameof(DefaultFlyoutPosition), defaultFlyoutPosition);
            set => SetPoint(nameof(DefaultFlyoutPosition), value);
        }

        public static Point FlyoutPosition
        {
            get => GetPoint(nameof(FlyoutPosition), defaultFlyoutPosition);
            set => SetPoint(nameof(FlyoutPosition), value);
        }
    }
}
