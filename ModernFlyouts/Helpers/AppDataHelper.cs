using ModernFlyouts.Controls;
using ModernFlyouts.Core.UI;
using ModernFlyouts.UI;
using ModernFlyouts.UI.Media;
using ModernWpf;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Windows.Storage;

namespace ModernFlyouts.Helpers
{
    public class AppDataHelper
    {
        #region Methods

        internal static T GetValue<T>(T defaultValue, [CallerMemberName] string propertyName = "")
        {
            try
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey(propertyName))
                {
                    string value = ApplicationData.Current.LocalSettings.Values[propertyName].ToString() ?? string.Empty;

                    if (!string.IsNullOrEmpty(value))
                    {
                        if (typeof(T) == typeof(string))
                        {
                            return (T)(object)value;
                        }
                        else if (typeof(T) == typeof(bool))
                        {
                            if (bool.TryParse(value, out bool result))
                            {
                                return (T)(object)result;
                            }
                        }
                        else if (typeof(T) == typeof(int))
                        {
                            if (int.TryParse(value, out int result))
                            {
                                return (T)(object)result;
                            }
                        }
                        else if (typeof(T) == typeof(double))
                        {
                            if (double.TryParse(value, out double result))
                            {
                                return (T)(object)result;
                            }
                        }
                        else if (typeof(T).IsEnum)
                        {
                            return (T)Enum.Parse(typeof(T), value);
                        }
                        else if (typeof(T) == typeof(BindablePoint))
                        {
                            if (BindablePoint.TryParse(value, out BindablePoint result))
                            {
                                return (T)(object)result;
                            }
                        }
                    }
                }
            }
            catch { }

            return defaultValue;
        }

        internal static void SetValue<T>(T value, [CallerMemberName] string propertyName = "")
        {
            try
            {
                ApplicationData.Current.LocalSettings.Values[propertyName] = value.ToString();
            }
            catch { }
        }

        internal static async Task ClearAppDataAsync()
        {
            try
            {
                await ApplicationData.Current.ClearAsync();
            }
            catch { }
        }

        internal static void SavePropertyValue(string value, string propertyName = "")
        {
            try
            {
                ApplicationData.Current.LocalSettings.Values[propertyName] = value;
            }
            catch { }
        }

        #endregion

        #region Properties

        #region General

        public static string Language
        {
            get => GetValue(string.Empty);
            set => SetValue(value);
        }

        public static bool AudioModuleEnabled
        {
            get => GetValue(DefaultValuesStore.AudioModuleEnabled);
            set => SetValue(value);
        }

        public static bool BrightnessModuleEnabled
        {
            get => GetValue(DefaultValuesStore.BrightnessModuleEnabled);
            set => SetValue(value);
        }

        public static bool AirplaneModeModuleEnabled
        {
            get => GetValue(DefaultValuesStore.AirplaneModeModuleEnabled);
            set => SetValue(value);
        }

        public static bool LockKeysModuleEnabled
        {
            get => GetValue(DefaultValuesStore.LockKeysModuleEnabled);
            set => SetValue(value);
        }

        public static DefaultFlyout DefaultFlyout
        {
            get => GetValue(DefaultValuesStore.PreferredDefaultFlyout);
            set => SetValue(value);
        }

        public static BindablePoint DefaultFlyoutPosition
        {
            get => GetValue(DefaultValuesStore.DefaultFlyoutPosition);
        }

        public static BindablePoint FlyoutPosition
        {
            get => GetValue(DefaultValuesStore.DefaultFlyoutPosition);
        }

        public static string PreferredDisplayMonitorId
        {
            get => GetValue(string.Empty);
            set => SetValue(value);
        }

        public static string SettingsWindowPlacement
        {
            get => GetValue(string.Empty);
            set => SetValue(value);
        }

        #endregion

        #region Layout

        public static FlyoutWindowPlacementMode OnScreenFlyoutWindowPlacementMode
        {
            get => GetValue(DefaultValuesStore.OnScreenFlyoutWindowPlacementMode);
            set => SetValue(value);
        }

        public static FlyoutWindowAlignments OnScreenFlyoutWindowAlignment
        {
            get => GetValue(DefaultValuesStore.OnScreenFlyoutWindowAlignment);
            set => SetValue(value);
        }

        public static Thickness OnScreenFlyoutWindowMargin
        {
            get => GetValue(DefaultValuesStore.OnScreenFlyoutWindowMargin);
            set => SetValue(value);
        }

        public static FlyoutWindowExpandDirection OnScreenFlyoutWindowExpandDirection
        {
            get => GetValue(DefaultValuesStore.OnScreenFlyoutWindowExpandDirection);
            set => SetValue(value);
        }

        public static StackingDirection OnScreenFlyoutContentStackingDirection
        {
            get => GetValue(DefaultValuesStore.OnScreenFlyoutContentStackingDirection);
            set => SetValue(value);
        }

        public static Orientation FlyoutOrientation
        {
            get => GetValue(DefaultValuesStore.FlyoutOrientation);
            set => SetValue(value);
        }

        public static bool UseSmallFlyout
        {
            get => GetValue(DefaultValuesStore.UseSmallFlyout);
            set => SetValue(value);
        }

        #endregion

        #region Module Specific

        #region Audio module related

        public static bool ShowGSMTCInVolumeFlyout
        {
            get => GetValue(DefaultValuesStore.ShowGSMTCInVolumeFlyout);
            set => SetValue(value);
        }

        public static bool ShowVolumeControlInGSMTCFlyout
        {
            get => GetValue(DefaultValuesStore.ShowVolumeControlInGSMTCFlyout);
            set => SetValue(value);
        }

        #endregion

        #region Lock keys module related

        public static bool LockKeysModule_CapsLockEnabled
        {
            get => GetValue(DefaultValuesStore.LockKeysModule_CapsLockEnabled);
            set => SetValue(value);
        }

        public static bool LockKeysModule_NumLockEnabled
        {
            get => GetValue(DefaultValuesStore.LockKeysModule_NumLockEnabled);
            set => SetValue(value);
        }

        public static bool LockKeysModule_ScrollLockEnabled
        {
            get => GetValue(DefaultValuesStore.LockKeysModule_ScrollLockEnabled);
            set => SetValue(value);
        }

        public static bool LockKeysModule_InsertEnabled
        {
            get => GetValue(DefaultValuesStore.LockKeysModule_InsertEnabled);
            set => SetValue(value);
        }

        #endregion

        #endregion

        #region UI

        public static TopBarVisibility TopBarVisibility
        {
            get => GetValue(DefaultValuesStore.DefaultTopBarVisibility);
            set => SetValue(value);
        }

        public static ElementTheme AppTheme
        {
            get => GetValue(DefaultValuesStore.AppTheme);
            set => SetValue(value);
        }

        public static ElementTheme FlyoutTheme
        {
            get => GetValue(DefaultValuesStore.FlyoutTheme);
            set => SetValue(value);
        }

        public static int FlyoutTimeout
        {
            get => GetValue(DefaultValuesStore.FlyoutTimeout);
            set => SetValue(value);
        }

        public static double FlyoutBackgroundOpacity
        {
            get => GetValue(DefaultValuesStore.FlyoutBackgroundOpacity);
            set => SetValue(value);
        }

        public static bool TrayIconEnabled
        {
            get => GetValue(DefaultValuesStore.TrayIconEnabled);
            set => SetValue(value);
        }

        public static bool UseColoredTrayIcon
        {
            get => GetValue(DefaultValuesStore.UseColoredTrayIcon);
            set => SetValue(value);
        }

        public static bool FlyoutAnimationEnabled
        {
            get => GetValue(DefaultValuesStore.FlyoutAnimationEnabled);
            set => SetValue(value);
        }

        public static bool AlignGSMTCThumbnailToRight
        {
            get => GetValue(DefaultValuesStore.AlignGSMTCThumbnailToRight);
            set => SetValue(value);
        }

        public static bool UseGSMTCThumbnailAsBackground
        {
            get => GetValue(DefaultValuesStore.UseGSMTCThumbnailAsBackground);
            set => SetValue(value);
        }

        public static int MaxVerticalSessionControlsCount
        {
            get => GetValue(DefaultValuesStore.MaxVerticalSessionControlsCount);
            set => SetValue(value);
        }

        public static Orientation SessionsPanelOrientation
        {
            get => GetValue(DefaultValuesStore.SessionsPanelOrientation);
            set => SetValue(value);
        }

        #endregion

        #endregion
    }
}
