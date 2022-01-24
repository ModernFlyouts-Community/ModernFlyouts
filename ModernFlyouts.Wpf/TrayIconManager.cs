using Hardcodet.Wpf.TaskbarNotification;
using ModernFlyouts.Helpers;
using ModernFlyouts.Utilities;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ModernFlyouts.Wpf
{
    internal class TrayIconManager
    {
        #region Properties

        public static TaskbarIcon TaskbarIcon { get; private set; }

        public static ContextMenu TaskbarIconContextMenu { get; private set; }

        public static ToolTip TaskbarIconToolTip { get; private set; }

        #endregion

        public static void SetupTrayIcon()
        {
            var settingsItem = new MenuItem()
            {
                Header = Properties.Strings.SettingsItem,
                ToolTip = Properties.Strings.SettingsItemDescription,
                Icon = new FontIcon() { Glyph = CommonGlyphs.Settings },
                Command = CommonCommands.OpenSettingsWindowCommand
            };

            var exitItem = new MenuItem()
            {
                Header = Properties.Strings.ExitItem,
                ToolTip = Properties.Strings.ExitItemDescription,
                Icon = new FontIcon() { Glyph = CommonGlyphs.PowerButton },
                Command = CommonCommands.ExitAppCommand
            };

            TaskbarIconContextMenu = new ContextMenu()
            {
                Items = { settingsItem, exitItem }
            };

            TaskbarIconToolTip = new ToolTip() { Content = Program.AppName };

            TaskbarIcon = new TaskbarIcon()
            {
                TrayToolTip = TaskbarIconToolTip,
                ContextMenu = TaskbarIconContextMenu,
                DoubleClickCommand = CommonCommands.OpenSettingsWindowCommand
            };
        }

        public static void UpdateTrayIconVisibility(bool isVisible)
        {
            TaskbarIcon.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
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

            TaskbarIcon.IconSource = BitmapFrame.Create(iconUri);
        }
    }
}
