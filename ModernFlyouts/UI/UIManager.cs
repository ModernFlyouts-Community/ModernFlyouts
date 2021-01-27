using Microsoft.Toolkit.Mvvm.ComponentModel;
using ModernFlyouts.Controls;
using ModernFlyouts.Core.UI;
using ModernFlyouts.Helpers;
using ModernWpf;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModernFlyouts.UI
{
    public class UIManager : ObservableObject
    {
        public const double FlyoutWidth = 354;

        public const double DefaultSessionControlHeight = 206;

        public const double DefaultSessionsPanelVerticalSpacing = 8;

        public const double FlyoutShadowDepth = 32;

        public static Thickness FlyoutShadowMargin = GetFlyoutShadowMargin(FlyoutShadowDepth);

        private ElementTheme currentSystemTheme = ElementTheme.Dark;
        private ThemeResources themeResources;
        private ResourceDictionary lightResources;
        private ResourceDictionary darkResources;

        private bool _isThemeUpdated;

        #region Properties

        private bool restartRequired;

        public bool RestartRequired
        {
            get => restartRequired;
            set => SetProperty(ref restartRequired, value);
        }

        #region General

        private TopBarVisibility topBarVisibility = TopBarVisibility.Visible;

        public TopBarVisibility TopBarVisibility
        {
            get => topBarVisibility;
            set
            {
                if (SetProperty(ref topBarVisibility, value))
                {
                    AppDataHelper.TopBarVisibility = value;
                }
            }
        }

        private ElementTheme appTheme = DefaultValuesStore.AppTheme;

        public ElementTheme AppTheme
        {
            get => appTheme;
            set
            {
                if (SetProperty(ref appTheme, value))
                {
                    UpdateAppTheme();
                    AppDataHelper.AppTheme = value;
                }
            }
        }

        private ElementTheme flyoutTheme = DefaultValuesStore.FlyoutTheme;

        public ElementTheme FlyoutTheme
        {
            get => flyoutTheme;
            set
            {
                if (SetProperty(ref flyoutTheme, value))
                {
                    UpdateTheme();
                    AppDataHelper.FlyoutTheme = value;
                }
            }
        }

        private ElementTheme actualFlyoutTheme = ElementTheme.Dark;

        public ElementTheme ActualFlyoutTheme
        {
            get => actualFlyoutTheme;
            private set => SetProperty(ref actualFlyoutTheme, value);
        }

        private int flyoutTimeout = DefaultValuesStore.FlyoutTimeout;

        public int FlyoutTimeout
        {
            get => flyoutTimeout;
            set
            {
                if (SetProperty(ref flyoutTimeout, value))
                {
                    AppDataHelper.FlyoutTimeout = flyoutTimeout;
                }
            }
        }

        private double flyoutBackgroundOpacity = DefaultValuesStore.FlyoutBackgroundOpacity;

        public double FlyoutBackgroundOpacity
        {
            get => flyoutBackgroundOpacity;
            set
            {
                if (SetProperty(ref flyoutBackgroundOpacity, value))
                {
                    OnFlyoutBackgroundOpacityChanged();
                }
            }
        }

        private bool trayIconEnabled = DefaultValuesStore.TrayIconEnabled;

        public bool TrayIconEnabled
        {
            get => trayIconEnabled;
            set
            {
                if (SetProperty(ref trayIconEnabled, value))
                {
                    OnTrayIconEnabledChanged();
                }
            }
        }

        private bool useColoredTrayIcon = DefaultValuesStore.UseColoredTrayIcon;

        public bool UseColoredTrayIcon
        {
            get => useColoredTrayIcon;
            set
            {
                if (SetProperty(ref useColoredTrayIcon, value))
                {
                    OnUseColoredTrayIconChanged();
                }
            }
        }

        #endregion

        #region Layout

        private FlyoutWindowPlacementMode onScreenFlyoutWindowPlacementMode;

        public FlyoutWindowPlacementMode OnScreenFlyoutWindowPlacementMode
        {
            get => onScreenFlyoutWindowPlacementMode;
            set
            {
                if (SetProperty(ref onScreenFlyoutWindowPlacementMode, value))
                {
                    AppDataHelper.OnScreenFlyoutWindowPlacementMode = value;
                }
            }
        }

        private FlyoutWindowAlignments onScreenFlyoutWindowAlignment;

        public FlyoutWindowAlignments OnScreenFlyoutWindowAlignment
        {
            get => onScreenFlyoutWindowAlignment;
            set
            {
                if (SetProperty(ref onScreenFlyoutWindowAlignment, value))
                {
                    AppDataHelper.OnScreenFlyoutWindowAlignment = value;
                }
            }
        }

        private Thickness onScreenFlyoutWindowMargin;

        public Thickness OnScreenFlyoutWindowMargin
        {
            get => onScreenFlyoutWindowMargin;
            set
            {
                if (SetProperty(ref onScreenFlyoutWindowMargin, value))
                {
                    AppDataHelper.OnScreenFlyoutWindowMargin = value;
                }
            }
        }

        private FlyoutWindowExpandDirection onScreenFlyoutWindowExpandDirection;

        public FlyoutWindowExpandDirection OnScreenFlyoutWindowExpandDirection
        {
            get => onScreenFlyoutWindowExpandDirection;
            set
            {
                if (SetProperty(ref onScreenFlyoutWindowExpandDirection, value))
                {
                    AppDataHelper.OnScreenFlyoutWindowExpandDirection = value;
                }
            }
        }

        private StackingDirection onScreenFlyoutContentStackingDirection;

        public StackingDirection OnScreenFlyoutContentStackingDirection
        {
            get => onScreenFlyoutContentStackingDirection;
            set
            {
                if (SetProperty(ref onScreenFlyoutContentStackingDirection, value))
                {
                    AppDataHelper.OnScreenFlyoutContentStackingDirection = value;
                }
            }
        }

        #endregion

        #region Media Controls

        private bool alignGSMTCThumbnailToRight = DefaultValuesStore.AlignGSMTCThumbnailToRight;

        public bool AlignGSMTCThumbnailToRight
        {
            get => alignGSMTCThumbnailToRight;
            set
            {
                if (SetProperty(ref alignGSMTCThumbnailToRight, value))
                {
                    AppDataHelper.AlignGSMTCThumbnailToRight = value;
                }
            }
        }

        private bool useGSMTCThumbnailAsBackground = DefaultValuesStore.UseGSMTCThumbnailAsBackground;

        public bool UseGSMTCThumbnailAsBackground
        {
            get => useGSMTCThumbnailAsBackground;
            set
            {
                if (SetProperty(ref useGSMTCThumbnailAsBackground, value))
                {
                    AppDataHelper.UseGSMTCThumbnailAsBackground = value;
                }
            }
        }

        private Orientation sessionsPanelOrientation = DefaultValuesStore.SessionsPanelOrientation;

        public Orientation SessionsPanelOrientation
        {
            get => sessionsPanelOrientation;
            set
            {
                if (SetProperty(ref sessionsPanelOrientation, value))
                {
                    OnSessionsPanelOrientation();
                }
            }
        }

        private int maxVerticalSessionControlsCount = DefaultValuesStore.MaxVerticalSessionControlsCount;

        public int MaxVerticalSessionControlsCount
        {
            get => maxVerticalSessionControlsCount;
            set
            {
                if (SetProperty(ref maxVerticalSessionControlsCount, value))
                {
                    OnMaxVerticalSessionControlsCount();
                }
            }
        }

        private double calculatedSessionsPanelMaxHeight = DefaultSessionControlHeight;

        public double CalculatedSessionsPanelMaxHeight
        {
            get => calculatedSessionsPanelMaxHeight;
            private set => SetProperty(ref calculatedSessionsPanelMaxHeight, value);
        }

        private double calculatedSessionsPanelSpacing;

        public double CalculatedSessionsPanelSpacing
        {
            get => calculatedSessionsPanelSpacing;
            private set => SetProperty(ref calculatedSessionsPanelSpacing, value);
        }

        #endregion

        #endregion

        public void Initialize()
        {
            OnScreenFlyoutWindowPlacementMode = AppDataHelper.OnScreenFlyoutWindowPlacementMode;
            OnScreenFlyoutWindowAlignment = AppDataHelper.OnScreenFlyoutWindowAlignment;
            OnScreenFlyoutWindowMargin = AppDataHelper.OnScreenFlyoutWindowMargin;
            OnScreenFlyoutWindowExpandDirection = AppDataHelper.OnScreenFlyoutWindowExpandDirection;
            OnScreenFlyoutContentStackingDirection = AppDataHelper.OnScreenFlyoutContentStackingDirection;

            TopBarVisibility = AppDataHelper.TopBarVisibility;
            FlyoutTimeout = AppDataHelper.FlyoutTimeout;
            AlignGSMTCThumbnailToRight = AppDataHelper.AlignGSMTCThumbnailToRight;
            UseGSMTCThumbnailAsBackground = AppDataHelper.UseGSMTCThumbnailAsBackground;
            MaxVerticalSessionControlsCount = AppDataHelper.MaxVerticalSessionControlsCount;
            SessionsPanelOrientation = AppDataHelper.SessionsPanelOrientation;

            themeResources = (ThemeResources)Application.Current.Resources
                .MergedDictionaries.FirstOrDefault(x => x is ThemeResources);
            lightResources = themeResources.ThemeDictionaries["Light"];
            darkResources = themeResources.ThemeDictionaries["Dark"];

            FlyoutBackgroundOpacity = AppDataHelper.FlyoutBackgroundOpacity;

            TrayIconManager.SetupTrayIcon();

            TrayIconEnabled = AppDataHelper.TrayIconEnabled;
            UseColoredTrayIcon = AppDataHelper.UseColoredTrayIcon;

            FlyoutTheme = AppDataHelper.FlyoutTheme;
            AppTheme = AppDataHelper.AppTheme;

            SystemTheme.SystemThemeChanged += OnSystemThemeChanged;
            SystemTheme.Initialize();
        }

        private void OnFlyoutBackgroundOpacityChanged()
        {
            UpdateFlyoutBackgroundOpacity();
            AppDataHelper.FlyoutBackgroundOpacity = flyoutBackgroundOpacity;
        }

        private void OnTrayIconEnabledChanged()
        {
            TrayIconManager.UpdateTrayIconVisibility(trayIconEnabled);
            AppDataHelper.TrayIconEnabled = TrayIconEnabled;
        }

        private void OnUseColoredTrayIconChanged()
        {
            UpdateTrayIcon();
            AppDataHelper.UseColoredTrayIcon = useColoredTrayIcon;
        }

        private void OnSystemThemeChanged(object sender, SystemThemeChangedEventArgs args)
        {
            currentSystemTheme = args.IsSystemLightTheme ? ElementTheme.Light : ElementTheme.Dark;
            UpdateTheme();
        }

        private void UpdateAppTheme()
        {
            ThemeManager.Current.ApplicationTheme = appTheme switch
            {
                ElementTheme.Default => null,
                ElementTheme.Light => ApplicationTheme.Light,
                ElementTheme.Dark => ApplicationTheme.Dark,
                _ => null,
            };
        }

        private void UpdateTheme()
        {
            ActualFlyoutTheme = flyoutTheme == ElementTheme.Default ? currentSystemTheme : flyoutTheme;

            if (!_isThemeUpdated)
            {
                _isThemeUpdated = true;
            }

            UpdateFlyoutBackgroundOpacity();
            UpdateTrayIcon();
        }

        private void UpdateFlyoutBackgroundOpacity()
        {
            if (!_isThemeUpdated) return;

            var themeResource = actualFlyoutTheme == ElementTheme.Light ? lightResources : darkResources;
            var brush = themeResource["FlyoutBackground"] as Brush;
            brush = brush.Clone();
            brush.Opacity = flyoutBackgroundOpacity * 0.01;
            themeResource["FlyoutBackground"] = brush;
        }

        private void UpdateTrayIcon()
        {
            if (!_isThemeUpdated) return;

            TrayIconManager.UpdateTrayIconInternal(currentSystemTheme, useColoredTrayIcon);
        }

        private void OnMaxVerticalSessionControlsCount()
        {
            UpdateCalculatedSessionsPanelMaxHeight();
            AppDataHelper.MaxVerticalSessionControlsCount = maxVerticalSessionControlsCount;
        }

        private void OnSessionsPanelOrientation()
        {
            UpdateCalculatedSessionsPanelMaxHeight();
            AppDataHelper.SessionsPanelOrientation = sessionsPanelOrientation;
        }

        private void UpdateCalculatedSessionsPanelMaxHeight()
        {
            if (sessionsPanelOrientation == Orientation.Vertical)
            {
                var n = maxVerticalSessionControlsCount;
                CalculatedSessionsPanelMaxHeight = (DefaultSessionControlHeight * n) + (DefaultSessionsPanelVerticalSpacing * (n - 1));
                CalculatedSessionsPanelSpacing = DefaultSessionsPanelVerticalSpacing;
            }
            else
            {
                CalculatedSessionsPanelMaxHeight = DefaultSessionControlHeight;
                CalculatedSessionsPanelSpacing = 0;
            }
        }

        internal static Thickness GetFlyoutShadowMargin(double depth)
        {
            double radius = 0.9 * depth;
            double offset = 0.4 * depth;

            return new Thickness(
                radius,
                radius,
                radius,
                radius + offset);
        }
    }

    public enum TopBarVisibility
    {
        Visible = 0,
        AutoHide = 1,
        Collapsed = 2
    }
}
