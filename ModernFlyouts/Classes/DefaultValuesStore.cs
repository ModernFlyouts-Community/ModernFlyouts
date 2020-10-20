using System.Windows;
using System.Windows.Controls;

namespace ModernFlyouts
{
    internal class DefaultValuesStore
    {
        #region General

        public const bool AudioModuleEnabled = true;

        public const bool ShowGSMTCInVolumeFlyout = true;

        public const bool ShowVolumeControlInGSMTCFlyout = true;

        public const bool BrightnessModuleEnabled = true;

        public const bool AirplaneModeModuleEnabled = true;

        public const bool LockKeysModuleEnabled = true;

        public const DefaultFlyout PreferredFlyout = DefaultFlyout.ModernFlyouts;

        public const bool TopBarEnabled = true;

        public static readonly Point DefaultFlyoutPosition = new Point(50, 60);

        #endregion

        #region UI

        public const int FlyoutTimeout = 2750;

        public const double FlyoutBackgroundOpacity = 100.0;

        public const bool UseColoredTrayIcon = true;

        public const int MaxVerticalSessionControlsCount = 1;

        public const Orientation SessionsPanelOrientation = Orientation.Horizontal;

        #endregion
    }
}
