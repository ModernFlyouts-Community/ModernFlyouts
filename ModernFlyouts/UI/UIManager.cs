using ModernWpf;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ModernFlyouts
{
    public class UIManager : INotifyPropertyChanged
    {
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

        #endregion

        public void Initialize(FlyoutWindow flyoutWindow)
        {
            _flyoutWindow = flyoutWindow;

            themeResources = (ThemeResources)App.Current.Resources.MergedDictionaries.FirstOrDefault(x => x is ThemeResources);
            lightResources = themeResources.ThemeDictionaries["Light"];
            darkResources = themeResources.ThemeDictionaries["Dark"];

            FlyoutBackgroundOpacity = AppDataHelper.FlyoutBackgroundOpacity;
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
                ThemeManager.SetRequestedTheme(_flyoutWindow.TrayContextMenu, currentTheme);
                ThemeManager.SetRequestedTheme(_flyoutWindow.TrayToolTip, currentTheme);

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

            _flyoutWindow.TaskbarIcon.IconSource = BitmapFrame.Create(iconUri);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
