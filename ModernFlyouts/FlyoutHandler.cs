using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

namespace ModernFlyouts
{
    public class FlyoutHandler
    {
        public static FlyoutHandler Instance { get; set; }

        public KeyboardHook KeyboardHook { get; private set; }

        public DUIHook DUIHook { get; private set; }

        public FlyoutWindow FlyoutWindow { get; set; }

        public SettingsWindow SettingsWindow { get; set; }

        public AudioHelper AudioHelper { get; set; }

        public AirplaneModeHelper AirplaneModeHelper { get; set; }

        public LockKeysHelper LockKeysHelper { get; set; }

        public TaskbarIcon TaskbarIcon { get; set; }

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

            #region Initiate Helpers

            AudioHelper = new AudioHelper();
            AirplaneModeHelper = new AirplaneModeHelper();
            LockKeysHelper = new LockKeysHelper();

            AudioHelper.ShowFlyoutRequested += ShowFlyout;
            AirplaneModeHelper.ShowFlyoutRequested += ShowFlyout;
            LockKeysHelper.ShowFlyoutRequested += ShowFlyout;

            #endregion
        }

        DispatcherTimer rehooktimer;

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
            if (Handled()) { DUIHandler.FindDUIAndHide(); }
            else { DUIHandler.FindDUIAndShow(); }
        }

        private void ShowFlyout(HelperBase helper)
        {
            if (!helper.IsEnabled)
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
    }
}