using Hardcodet.Wpf.TaskbarNotification;
using ModernFlyouts.Utilities;
using ModernWpf;
using ModernWpf.Controls;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ModernFlyouts
{
    public class UIManager : INotifyPropertyChanged
    {
        public const double FlyoutWidth = 354;

        public const double DefaultSessionControlHeight = 138;

        public const double DefaultSessionsPanelVerticalSpacing = 8;

        private FlyoutWindow _flyoutWindow;
        private ElementTheme currentTheme = ElementTheme.Dark;
        private ThemeResources themeResources;
        private ResourceDictionary lightResources;
        private ResourceDictionary darkResources;

        private bool _isThemeUpdated = false;

        #region Properties

        #region General

        private int flyoutTimeout = DefaultValuesStore.FlyoutTimeout;

        public int FlyoutTimeout
        {
            get { return flyoutTimeout; }
            set
            {
                if (flyoutTimeout != value)
                {
                    flyoutTimeout = value;
                    OnPropertyChanged();
                    OnFlyoutTimeoutChanged();
                }
            }
        }

        private double flyoutBackgroundOpacity = DefaultValuesStore.FlyoutBackgroundOpacity;

        public double FlyoutBackgroundOpacity
        {
            get { return flyoutBackgroundOpacity; }
            set
            {
                if (flyoutBackgroundOpacity != value)
                {
                    flyoutBackgroundOpacity = value;
                    OnPropertyChanged();
                    OnFlyoutBackgroundOpacityChanged();
                }
            }
        }

        private bool useColoredTrayIcon = DefaultValuesStore.UseColoredTrayIcon;

        public bool UseColoredTrayIcon
        {
            get { return useColoredTrayIcon; }
            set
            {
                if (useColoredTrayIcon != value)
                {
                    useColoredTrayIcon = value;
                    OnPropertyChanged();
                    OnUseColoredTrayIconChanged();
                }
            }
        }

        #endregion

        #region Media Controls

        private Orientation sessionsPanelOrientation = DefaultValuesStore.SessionsPanelOrientation;

        public Orientation SessionsPanelOrientation
        {
            get { return sessionsPanelOrientation; }
            set
            {
                if (sessionsPanelOrientation != value)
                {
                    sessionsPanelOrientation = value;
                    OnPropertyChanged();
                    OnSessionsPanelOrientation();
                }
            }
        }

        private int maxVerticalSessionControlsCount = DefaultValuesStore.MaxVerticalSessionControlsCount;

        public int MaxVerticalSessionControlsCount
        {
            get { return maxVerticalSessionControlsCount; }
            set
            {
                if (maxVerticalSessionControlsCount != value)
                {
                    maxVerticalSessionControlsCount = value;
                    OnPropertyChanged();
                    OnMaxVerticalSessionControlsCount();
                }
            }
        }

        private double calculatedSessionsPanelMaxHeight = DefaultSessionControlHeight;

        public double CalculatedSessionsPanelMaxHeight
        {
            get { return calculatedSessionsPanelMaxHeight; }
            private set
            {
                if (calculatedSessionsPanelMaxHeight != value)
                {
                    calculatedSessionsPanelMaxHeight = value;
                    OnPropertyChanged();
                }
            }
        }

        private double calculatedSessionsPanelSpacing = 0;

        public double CalculatedSessionsPanelSpacing
        {
            get { return calculatedSessionsPanelSpacing; }
            private set
            {
                if (calculatedSessionsPanelSpacing != value)
                {
                    calculatedSessionsPanelSpacing = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Tray Icon

        public TaskbarIcon TaskbarIcon { get; private set; }

        public ContextMenu TaskbarIconContextMenu { get; private set; }

        public ToolTip TaskbarIconToolTip { get; private set; }

        #endregion

        #endregion

        public void Initialize(FlyoutWindow flyoutWindow)
        {
            _flyoutWindow = flyoutWindow;

            FlyoutTimeout = AppDataHelper.FlyoutTimeout;
            MaxVerticalSessionControlsCount = AppDataHelper.MaxVerticalSessionControlsCount;
            SessionsPanelOrientation = AppDataHelper.SessionsPanelOrientation;

            themeResources = (ThemeResources)App.Current.Resources.MergedDictionaries.FirstOrDefault(x => x is ThemeResources);
            lightResources = themeResources.ThemeDictionaries["Light"];
            darkResources = themeResources.ThemeDictionaries["Dark"];

            FlyoutBackgroundOpacity = AppDataHelper.FlyoutBackgroundOpacity;

            #region Setup TaskbarIcon

            var settingsItem = new MenuItem()
            {
                Header = Properties.Strings.SettingsItem,
                ToolTip = Properties.Strings.SettingsItemDescription,
                Icon = new SymbolIcon() { Symbol = Symbol.Setting }
            };
            settingsItem.Click += (_, __) => FlyoutHandler.ShowSettingsWindow();

            var exitItem = new MenuItem()
            {
                Header = Properties.Strings.ExitItem,
                ToolTip = Properties.Strings.ExitItemDescription,
                Icon = new FontIcon() { Glyph = CommonGlyphs.PowerButton }
            };
            exitItem.Click += (_, __) => FlyoutHandler.SafelyExitApplication();

            TaskbarIconContextMenu = new ContextMenu()
            {
                Items = { settingsItem, exitItem }
            };

            TaskbarIconToolTip = new ToolTip() { Content = Program.AppName };

            TaskbarIcon = new TaskbarIcon()
            {
                TrayToolTip = TaskbarIconToolTip,
                ContextMenu = TaskbarIconContextMenu
            };
            TaskbarIcon.TrayMouseDoubleClick += (_, __) => FlyoutHandler.ShowSettingsWindow();

            #endregion

            UseColoredTrayIcon = AppDataHelper.UseColoredTrayIcon;

            SystemTheme.SystemThemeChanged += OnSystemThemeChange;
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

        private void OnUseColoredTrayIconChanged()
        {
            UpdateTrayIcon();
            AppDataHelper.UseColoredTrayIcon = useColoredTrayIcon;
        }

        private void OnSystemThemeChange(object sender, SystemThemeChangedEventArgs args)
        {
            _flyoutWindow.Dispatcher.Invoke(() =>
            {
                currentTheme = args.IsSystemLightTheme ? ElementTheme.Light : ElementTheme.Dark;
                ThemeManager.SetRequestedTheme(_flyoutWindow, currentTheme);
                ThemeManager.SetRequestedTheme(TaskbarIconContextMenu, currentTheme);
                ThemeManager.SetRequestedTheme(TaskbarIconToolTip, currentTheme);

                if (!_isThemeUpdated)
                {
                    _isThemeUpdated = true;
                }

                UpdateFlyoutBackgroundOpacity();
                UpdateTrayIcon();
            });
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

            Uri iconUri;
            if (useColoredTrayIcon)
            {
                iconUri = PackUriHelper.GetAbsoluteUri(@"Assets\Logo.ico");
            }
            else
            {
                iconUri = PackUriHelper.GetAbsoluteUri(currentTheme == ElementTheme.Light ? @"Assets\Logo_Tray_Black.ico" : @"Assets\Logo_Tray_White.ico");
            }

            TaskbarIcon.IconSource = BitmapFrame.Create(iconUri);
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
