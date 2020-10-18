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

        public const double SessionControlHeight = 138;

        private FlyoutWindow _flyoutWindow;
        private ElementTheme currentTheme = ElementTheme.Dark;
        private ThemeResources themeResources;
        private ResourceDictionary lightResources;
        private ResourceDictionary darkResources;

        private bool _isThemeUpdated = false;

        #region Properties

        private double flyoutBackgroundOpacity = 100.0;

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

        private bool useColoredTrayIcon = false;

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

        public TaskbarIcon TaskbarIcon { get; private set; }

        public ContextMenu TaskbarIconContextMenu { get; private set; }

        public ToolTip TaskbarIconToolTip { get; private set; }

        #endregion

        public void Initialize(FlyoutWindow flyoutWindow)
        {
            _flyoutWindow = flyoutWindow;

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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
