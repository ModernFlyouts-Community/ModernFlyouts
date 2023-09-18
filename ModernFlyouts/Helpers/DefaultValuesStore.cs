using ModernFlyouts.Controls;
using ModernFlyouts.Core.UI;
using ModernFlyouts.UI;
using ModernFlyouts.UI.Media;
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

        public static BindablePoint DefaultFlyoutPosition => new(50, 60);

        #endregion

        #region Layout

        public const FlyoutWindowPlacementMode OnScreenFlyoutWindowPlacementMode = FlyoutWindowPlacementMode.Auto;

        public const FlyoutWindowAlignments OnScreenFlyoutWindowAlignment = FlyoutWindowAlignments.Top | FlyoutWindowAlignments.Left;

        public static Thickness OnScreenFlyoutWindowMargin = new(10);

        public const FlyoutWindowExpandDirection OnScreenFlyoutWindowExpandDirection = FlyoutWindowExpandDirection.Auto;

        public const StackingDirection OnScreenFlyoutContentStackingDirection = StackingDirection.Ascending;

        public const Orientation FlyoutOrientation = Orientation.Horizontal;

        public const bool UseSmallFlyout = false;

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

        #region Brightness module related

        public const bool BrightnessCompatibility = false;

        #endregion

        #endregion

        #region UI

        public const TopBarVisibility DefaultTopBarVisibility = TopBarVisibility.Visible;

        public const ElementTheme AppTheme = ElementTheme.Default;

        public const ElementTheme FlyoutTheme = ElementTheme.Default;

        public const int FlyoutTimeout = 2750;

        public static string RecommendedFlyoutTimeout = FlyoutTimeout.ToString();

        public const double FlyoutBackgroundOpacity = 100.0;

        public const bool TrayIconEnabled = true;

        public const bool UseColoredTrayIcon = true;

        public const bool FlyoutAnimationEnabled = true;

        public const bool AlignGSMTCThumbnailToRight = true;

        public const bool UseGSMTCThumbnailAsBackground = true;

        public const int MaxVerticalSessionControlsCount = 1;

        public const Orientation SessionsPanelOrientation = Orientation.Horizontal;

        #endregion
    }
}
