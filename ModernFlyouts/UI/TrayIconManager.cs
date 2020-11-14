using Hardcodet.Wpf.TaskbarNotification;
using ModernFlyouts.Helpers;
using ModernFlyouts.Utilities;
using ModernWpf;
using ModernWpf.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ModernFlyouts.UI
{
    internal class TrayIconManager
    {
        #region Properties

        public static TaskbarIcon TaskbarIcon { get; private set; }

        public static ContextMenu TaskbarIconContextMenu { get; private set; }

        public static ToolTip TaskbarIconToolTip { get; private set; }

        #endregion Properties

        public static void SetupTrayIcon()
        {
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
        }

        /// <exception cref="T:System.InvalidOperationException">Attempted to modify a read-only dependency property, or a property on a sealed <see cref="T:System.Windows.DependencyObject" />.</exception>
        public static void UpdateTrayIconVisibility(bool isVisible)
        {
            TaskbarIcon.SetCurrentValue(UIElement.VisibilityProperty, isVisible ? Visibility.Visible : Visibility.Collapsed);
        }

        public static void UpdateTrayIconInternal(ElementTheme currentTheme, bool useColoredTrayIcon)
        {
            ThemeManager.SetRequestedTheme(TaskbarIconContextMenu, currentTheme);
            ThemeManager.SetRequestedTheme(TaskbarIconToolTip, currentTheme);

            Uri iconUri;
            if (useColoredTrayIcon)
            {
                iconUri = PackUriHelper.GetAbsoluteUri(@"Assets\Logo.ico");
            }
            else
            {
                iconUri = PackUriHelper.GetAbsoluteUri(currentTheme == ElementTheme.Light ? @"Assets\Logo_Tray_Black.ico" : @"Assets\Logo_Tray_White.ico");
            }

            TaskbarIcon.SetCurrentValue(TaskbarIcon.IconSourceProperty, BitmapFrame.Create(iconUri));
        }
    }
}