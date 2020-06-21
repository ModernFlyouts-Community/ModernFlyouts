using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

namespace ModernFlyouts
{
    public class FlyoutHandler : INotifyPropertyChanged
    {
        DispatcherTimer rehooktimer;

        #region Properties

        public static FlyoutHandler Instance { get; set; }

        public KeyboardHook KeyboardHook { get; private set; }

        public DUIHook DUIHook { get; private set; }

        public FlyoutWindow FlyoutWindow { get; set; }

        public SettingsWindow SettingsWindow { get; set; }

        public AudioHelper AudioHelper { get; set; }

        public AirplaneModeHelper AirplaneModeHelper { get; set; }

        public LockKeysHelper LockKeysHelper { get; set; }

        public TaskbarIcon TaskbarIcon { get; set; }

        private DefaultFlyout defaultFlyout = DefaultFlyout.ModernFlyouts;

        public DefaultFlyout DefaultFlyout
        {
            get { return defaultFlyout; }
            set
            {
                if (defaultFlyout != value)
                {
                    defaultFlyout = value;
                    OnPropertyChanged();
                    OnDefaultFlyoutChanged();
                }
            }
        }

        #endregion

        public void Initialize()
        {
            DUIHandler.ForceFindDUIAndHide();

            FlyoutWindow = new FlyoutWindow();
            FlyoutWindow.SourceInitialized += FlyoutWindow_SourceInitialized;

            SystemTheme.SystemThemeChanged += OnSystemThemeChange;
            SystemTheme.Initialize();

            (IntPtr Host, int processid) = DUIHandler.GetAll();
            DUIHook = new DUIHook();
            DUIHook.Hook(Host, (uint)processid);
            DUIHook.DUIShown += DUIShown;
            DUIHook.DUIDestroyed += DUIDestroyed;
            rehooktimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(3), IsEnabled = false };
            rehooktimer.Tick += (_, __) => TryRehook();

            KeyboardHook = new KeyboardHook();

            #region Load Settings

            var adEnabled = Properties.Settings.Default.AudioModuleEnabled;
            var apmdEnabled = Properties.Settings.Default.AirplaneModeModuleEnabled;
            var lkkyEnabled = Properties.Settings.Default.LockKeysModuleEnabled;
            var defaultFlyoutString = Properties.Settings.Default.DefaultFlyout;

            if (Enum.TryParse(defaultFlyoutString, true, out DefaultFlyout _defaultFlyout))
            {
                DefaultFlyout = _defaultFlyout;
            }
            else
            {
                Properties.Settings.Default.DefaultFlyout = DefaultFlyout.ToString();
                Properties.Settings.Default.Save();
            }

            #endregion

            #region Initiate Helpers

            AudioHelper = new AudioHelper() { IsEnabled = adEnabled };
            AirplaneModeHelper = new AirplaneModeHelper() { IsEnabled = apmdEnabled };
            LockKeysHelper = new LockKeysHelper() { IsEnabled = lkkyEnabled };

            AudioHelper.ShowFlyoutRequested += ShowFlyout;
            AirplaneModeHelper.ShowFlyoutRequested += ShowFlyout;
            LockKeysHelper.ShowFlyoutRequested += ShowFlyout;

            #endregion
        }

        private void DUIDestroyed()
        {
            rehooktimer.Start();
        }

        private void TryRehook()
        {
            (IntPtr Host, int processid) = DUIHandler.GetAll();
            if (Host != IntPtr.Zero && processid != 0)
            {
                rehooktimer.Stop();
                DUIHook.Rehook(Host, (uint)processid);
            }
        }

        private void DUIShown()
        {
            Debug.WriteLine(nameof(DUIHook) + ": DUI Shown!");

            if (DefaultFlyout == DefaultFlyout.ModernFlyouts && Handled())
            {
                DUIHandler.FindDUIAndHide();
            }
            else if (DefaultFlyout == DefaultFlyout.None)
            {
                DUIHandler.FindDUIAndHide();
            }
            else
            {
                DUIHandler.FindDUIAndShow();
            }
        }

        private void ShowFlyout(HelperBase helper)
        {
            if (DefaultFlyout != DefaultFlyout.ModernFlyouts || !helper.IsEnabled)
            {
                return;
            }

            FlyoutWindow.StopHideTimer();

            if (helper.AlwaysHandleDefaultFlyout)
            {
                DUIHandler.FindDUIAndHide();
            }

            FlyoutWindow.DataContext = helper;
            FlyoutWindow.Visible = true;
            FlyoutWindow.StartHideTimer();
        }

        private bool Handled()
        {
            if (FlyoutWindow.DataContext is HelperBase helper)
            {
                return FlyoutWindow.Visible && helper.AlwaysHandleDefaultFlyout && helper.IsEnabled;
            }
            return false;
        }

        private void OnDefaultFlyoutChanged()
        {
            if (defaultFlyout != DefaultFlyout.ModernFlyouts)
            {
                FlyoutWindow.Visible = false;
            }
            if (defaultFlyout != DefaultFlyout.WindowsDefault)
            {
                DUIHandler.FindDUIAndHide();
            }

            Properties.Settings.Default.DefaultFlyout = defaultFlyout.ToString();
            Properties.Settings.Default.Save();
        }

        private void FlyoutWindow_SourceInitialized(object sender, EventArgs e)
        {
            var wih = new WindowInteropHelper(FlyoutWindow);
            var hWnd = wih.Handle;
            var source = HwndSource.FromHwnd(hWnd);
            source.AddHook(WndProc);
            NativeMethods.SetToolWindow(hWnd);
        }

        private const int MA_NOACTIVATE = 0x3;
        private const int WM_MOUSEACTIVATE = 0x21;

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_MOUSEACTIVATE)
            {
                handled = true;
                return new IntPtr(MA_NOACTIVATE);
            }

            return IntPtr.Zero;
        }

        private void OnSystemThemeChange(object sender, SystemThemeChangedEventArgs args)
        {
            FlyoutWindow.Dispatcher.Invoke(() =>
            {
                var theme = args.IsSystemLightTheme ? ModernWpf.ElementTheme.Light : ModernWpf.ElementTheme.Dark;
                ModernWpf.ThemeManager.SetRequestedTheme(FlyoutWindow, theme);
                ModernWpf.ThemeManager.SetRequestedTheme(FlyoutWindow.TrayContextMenu, theme);
            });
        }

        public static void SafelyExitApplication()
        {
            DUIHandler.FindDUIAndShow();
            Application.Current.Shutdown();
        }

        public static void ShowSettingsWindow()
        {
            if (Instance.SettingsWindow == null)
            {
                Instance.SettingsWindow = new SettingsWindow();
                void handler(object sender, CancelEventArgs e)
                {
                    Instance.SettingsWindow.Closing -= handler;
                    Instance.SettingsWindow = null;
                }
                Instance.SettingsWindow.Closing += handler;
            }
            Instance.SettingsWindow.Show();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum DefaultFlyout
    {
        WindowsDefault = 0,
        ModernFlyouts = 1,
        None = 2
    }
}