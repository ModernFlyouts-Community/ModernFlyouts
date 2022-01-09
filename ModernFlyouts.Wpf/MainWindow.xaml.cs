using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace ModernFlyouts.Wpf
{
    public partial class MainWindow : Window
    {
        /* TO DO:
         * ADD SYSTEM VOLUME TRAY FLYOUT FROM FLUENTFLYOUTS.TRAY
         * MOVE THIS CODE INTO A TRAYHELPER.CS OR SERVICE MANAGER
         * MOVE VOLUME TRAY TO BE THE NEW MEDIA FLYOUT INSTEAD OF TRAY FLYOUT
         * ADD COMMENTS AFTER THIS IS DONE
         * ASSET TRAY ICON MANAGER
        */

        private VolumeFlyoutWindow _volumeflyoutwindow;
        private NotifyIcon _volumenotifyIcon;

        private BatteryFlyoutWindow _batteryflyoutwindow;
        private NotifyIcon _notifyIcon;

        public MainWindow()
        {
            InitializeComponent();
            GenerateBatteryTray();
            GenerateVolumeTray();
        }

        private void GenerateVolumeTray()
        {
            _volumeflyoutwindow = new VolumeFlyoutWindow();
            _volumeflyoutwindow.Show();
            _volumeflyoutwindow.Visibility = Visibility.Visible;
            _volumenotifyIcon = new NotifyIcon();
            _volumenotifyIcon.Visible = true;
            _volumenotifyIcon.Icon = new System.Drawing.Icon(Path.Combine(Environment.CurrentDirectory, @"Assets\Battery524.ico"));
            _volumenotifyIcon.MouseDown += (s, args) => ShowVolumeWindow(s, args);
        }

        private void ShowVolumeWindow(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            _volumeflyoutwindow.ShowFlyout();
            var screenWidth = SystemParameters.PrimaryScreenWidth;
            var screenHeight = SystemParameters.PrimaryScreenHeight;
            var windowWidth = _volumeflyoutwindow.Width;
            var windowHeight = _volumeflyoutwindow.Height;

            _volumeflyoutwindow.Left = (screenWidth / 1.17) - (windowWidth / 2);
            _volumeflyoutwindow.Top = (screenHeight / 1.058) - (windowHeight / 2);
        }

        private void GenerateBatteryTray()
        {
            _batteryflyoutwindow = new BatteryFlyoutWindow();
            _batteryflyoutwindow.Show();
            _batteryflyoutwindow.Visibility = Visibility.Visible;
            _notifyIcon = new NotifyIcon();
            _notifyIcon.Visible = true;
            _notifyIcon.Icon = new System.Drawing.Icon(Path.Combine(Environment.CurrentDirectory, @"Assets\Battery524.ico"));
            _notifyIcon.MouseDown += (s, args) => ShowBatteryWindow(s, args);
        }

        private void ShowBatteryWindow(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            _batteryflyoutwindow.ShowFlyout();
            var screenWidth = SystemParameters.PrimaryScreenWidth;
            var screenHeight = SystemParameters.PrimaryScreenHeight;
            var windowWidth = _batteryflyoutwindow.Width;
            var windowHeight = _batteryflyoutwindow.Height;

            _batteryflyoutwindow.Left = (screenWidth / 1.17) - (windowWidth / 2);
            _batteryflyoutwindow.Top = (screenHeight / 1.058) - (windowHeight / 2);
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
  }
}