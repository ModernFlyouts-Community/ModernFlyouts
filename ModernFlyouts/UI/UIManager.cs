﻿using Microsoft.Toolkit.Mvvm.ComponentModel;
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

        public const double DefaultSessionControlHeight = 138;

        public const double DefaultSessionsPanelVerticalSpacing = 8;

        private FlyoutWindow _flyoutWindow;
        private ElementTheme currentSystemTheme = ElementTheme.Dark;
        private ElementTheme currentTheme = ElementTheme.Dark;
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
                    _flyoutWindow.OnTopBarVisibilityChanged(value);
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

        private int flyoutTimeout = DefaultValuesStore.FlyoutTimeout;

        public int FlyoutTimeout
        {
            get => flyoutTimeout;
            set
            {
                if (SetProperty(ref flyoutTimeout, value))
                {
                    OnFlyoutTimeoutChanged();
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
        
        private bool showGSMTCThumbnailInBackground = DefaultValuesStore.ShowGSMTCThumbnailInBackground;

        public bool ShowGSMTCThumbnailInBackground
        {
            get => showGSMTCThumbnailInBackground;
            set
            {
                if (SetProperty(ref showGSMTCThumbnailInBackground, value))
                {
                    AppDataHelper.ShowGSMTCThumbnailInBackground = value;
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

        public void Initialize(FlyoutWindow flyoutWindow)
        {
            _flyoutWindow = flyoutWindow;

            TopBarVisibility = AppDataHelper.TopBarVisibility;
            FlyoutTimeout = AppDataHelper.FlyoutTimeout;
            AlignGSMTCThumbnailToRight = AppDataHelper.AlignGSMTCThumbnailToRight;
            ShowGSMTCThumbnailInBackground = AppDataHelper.ShowGSMTCThumbnailInBackground;
            MaxVerticalSessionControlsCount = AppDataHelper.MaxVerticalSessionControlsCount;
            SessionsPanelOrientation = AppDataHelper.SessionsPanelOrientation;

            themeResources = (ThemeResources)App.Current.Resources.MergedDictionaries.FirstOrDefault(x => x is ThemeResources);
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

        private void OnFlyoutTimeoutChanged()
        {
            _flyoutWindow.UpdateHideTimerInterval(flyoutTimeout);
            AppDataHelper.FlyoutTimeout = flyoutTimeout;
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
            _flyoutWindow.Dispatcher.Invoke(() =>
            {
                currentSystemTheme = args.IsSystemLightTheme ? ElementTheme.Light : ElementTheme.Dark;
                UpdateTheme();
            });
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
            currentTheme = flyoutTheme == ElementTheme.Default ? currentSystemTheme : flyoutTheme;
            ThemeManager.SetRequestedTheme(_flyoutWindow, currentTheme);

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

            var themeResource = currentTheme == ElementTheme.Light ? lightResources : darkResources;
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
    }

    public enum TopBarVisibility
    {
        Visible = 0,
        AutoHide = 1,
        Collapsed = 2
    }
}
