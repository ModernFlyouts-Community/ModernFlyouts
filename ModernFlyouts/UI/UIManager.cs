using ModernWpf;
using System;
using System.ComponentModel;
using System.Reflection;
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

        #region Properties

        private double flyoutBackgroundOpacity = 90.0;

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

            ListenToSystemColorChanges();

            SystemTheme.SystemThemeChanged += OnSystemThemeChange;
            SystemTheme.Initialize();

            FlyoutBackgroundOpacity = AppDataHelper.FlyoutBackgroundOpacity;
            UseColoredTrayIcon = AppDataHelper.UseColoredTrayIcon;
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
                UpdateFlyoutBackgroundOpacity();
                UpdateTrayIcon();
            });
        }

        private ResourceDictionary resourceDictionary;

        private void UpdateFlyoutBackgroundOpacity()
        {
            //var brush = App.Current.Resources["FlyoutBackground"] as Brush;
            //if (resourceDictionary != null && App.Current.Resources.MergedDictionaries.Contains(resourceDictionary))
            //{
            //    App.Current.Resources.MergedDictionaries.Remove(resourceDictionary);
            //    resourceDictionary = null;
            //}
            //resourceDictionary = new ResourceDictionary();
            //brush = brush.Clone();
            //brush.Opacity = 0;
            //resourceDictionary.Add("FlyoutBackground", brush);
            //App.Current.Resources.MergedDictionaries.Add(resourceDictionary);
        }

        private void UpdateTrayIcon()
        {
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

        // Temporary workaround for https://github.com/ShankarBUS/ModernFlyouts/issues/38
        private static void ListenToSystemColorChanges()
        {
            Type ColorsHelperType = Type.GetType("ModernWpf.ColorsHelper, ModernWpf");
            PropertyInfo CurrentProperty = ColorsHelperType.GetProperty("Current", BindingFlags.Static | BindingFlags.Public);
            object colorsHelperInstance = CurrentProperty.GetValue(null);
            MethodInfo ListenToSystemColorChangesMethod = ColorsHelperType.GetMethod("ListenToSystemColorChanges",
                BindingFlags.Instance | BindingFlags.NonPublic);
            ListenToSystemColorChangesMethod.Invoke(colorsHelperInstance, null);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
