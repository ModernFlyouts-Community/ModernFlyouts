using ModernFlyouts.UI;
using ModernWpf;
using System.Windows;
using System.Windows.Controls;

namespace ModernFlyouts.Helpers
{
    internal class DefaultValuesStore
    {
        #region General

        public const bool AudioModuleEnabled = true;

        public const bool BrightnessModuleEnabled = true;

        public const bool AirplaneModeModuleEnabled = true;

        public const bool LockKeysModuleEnabled = true;

        public const DefaultFlyout PreferredDefaultFlyout = DefaultFlyout.ModernFlyouts;

        public static readonly Point DefaultFlyoutPosition = new Point(50, 60);

        #endregion

        #region Module specific

        #region Audio module related

        public const bool ShowGSMTCInVolumeFlyout = true;

        public const bool ShowVolumeControlInGSMTCFlyout = true;

        #endregion

        #region Lock keys module related

        public const bool LockKeysModule_CapsLockEnabled = true;

        public const bool LockKeysModule_NumLockEnabled = true;

        public const bool LockKeysModule_ScrollLockEnabled = true;

        public const bool LockKeysModule_InsertEnabled = true;

        #endregion

        #endregion

        #region UI

        public const TopBarVisibility DefaultTopBarVisibility = TopBarVisibility.Visible;

        public const ElementTheme FlyoutTheme = ElementTheme.Dark;

        public const int FlyoutTimeout = 2750;

        public const double FlyoutBackgroundOpacity = 100.0;

        public const bool TrayIconEnabled = true;

        public const bool UseColoredTrayIcon = true;

        public const bool AlignGSMTCThumbnailToRight = true;

        public const int MaxVerticalSessionControlsCount = 1;

        public const Orientation SessionsPanelOrientation = Orientation.Horizontal;

        #endregion
    }
}
