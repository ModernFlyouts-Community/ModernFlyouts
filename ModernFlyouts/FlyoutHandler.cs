using System;
using System.Diagnostics;
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

        public AudioHelper AudioHelper { get; set; }

        public AirplaneModeHelper AirplaneModeHelper { get; set; }

        public LockKeysHelper LockKeysHelper { get; set; }

        public void Initialize()
        {
            DUIHandler.ForceFindDUIAndHide();

            FlyoutWindow = new FlyoutWindow();
            FlyoutWindow.SourceInitialized += FlyoutWindow_SourceInitialized;

            OnSystemThemeChange();

            (IntPtr Host, int processid) = DUIHandler.GetAll();
            DUIHook = new DUIHook();
            DUIHook.Hook(Host, (uint)processid);
            DUIHook.DUIShown += DUIShown;
            DUIHook.DUIDestroyed += DUIDestroyed;
            rehooktimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(2), IsEnabled = false };
            rehooktimer.Tick += (_, __) => TryRehook();

            KeyboardHook = new KeyboardHook();

            #region Initiate Helpers

            AudioHelper = new AudioHelper(); AudioHelper.ShowFlyoutRequested += (s, h) => ShowFlyout(s, h);
            AirplaneModeHelper = new AirplaneModeHelper(); AirplaneModeHelper.ShowFlyoutRequested += (s, h) => ShowFlyout(s, h);
            LockKeysHelper = new LockKeysHelper(); LockKeysHelper.ShowFlyoutRequested += (s, h) => ShowFlyout(s, h);

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

        private void ShowFlyout(object helper, bool handled)
        {
            FlyoutWindow.StopHideTimer();   

            if (handled)
            {
                DUIHandler.FindDUIAndHide();
            }

            FlyoutWindow.DataContext = helper;
            FlyoutWindow.Visible = true;
            FlyoutWindow.StartHideTimer();
        }

        private bool Handled()
        {
            return FlyoutWindow.Visible && (FlyoutWindow.DataContext == AudioHelper || FlyoutWindow.DataContext == AirplaneModeHelper);
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
        private const int WM_WININICHANGE = 0x001A;
        private const int WM_SETTINGCHANGE = WM_WININICHANGE;

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_MOUSEACTIVATE)
            {
                handled = true;
                return new IntPtr(MA_NOACTIVATE);
            }
            else if (msg == WM_SETTINGCHANGE)
            {
                if (System.Runtime.InteropServices.Marshal.PtrToStringAuto(lParam) == "ImmersiveColorSet")
                {
                    handled = true;
                    OnSystemThemeChange();
                }
            }

            return IntPtr.Zero;
        }

        private void OnSystemThemeChange()
        {
            ModernWpf.ThemeManager.SetRequestedTheme(FlyoutWindow, SystemTheme.GetIsSystemLightTheme() ? ModernWpf.ElementTheme.Light : ModernWpf.ElementTheme.Dark);
        }
    }
}