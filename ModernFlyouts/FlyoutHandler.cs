using Microsoft.Toolkit.Mvvm.ComponentModel;
using ModernFlyouts.AppLifecycle;
using ModernFlyouts.Core.Interop;
using ModernFlyouts.Helpers;
using ModernFlyouts.Interop;
using ModernFlyouts.UI;
using ModernFlyouts.UI.Media;
using ModernFlyouts.Workarounds;
using System;
using System.Windows.Interop;
using static ModernFlyouts.Core.Interop.NativeMethods;

namespace ModernFlyouts
{
    public class FlyoutHandler : ObservableObject
    {
        public static event EventHandler Initialized;

        #region Properties

        public static FlyoutHandler Instance { get; set; }

        public static bool HasInitialized { get; private set; }

        public KeyboardHook KeyboardHook { get; private set; }

        public FlyoutWindow FlyoutWindow { get; set; }

        public SettingsWindow SettingsWindow { get; set; }

        public UIManager UIManager { get; set; }

        public AudioFlyoutHelper AudioFlyoutHelper { get; set; }

        public AirplaneModeFlyoutHelper AirplaneModeFlyoutHelper { get; set; }

        public LockKeysFlyoutHelper LockKeysFlyoutHelper { get; set; }

        public BrightnessFlyoutHelper BrightnessFlyoutHelper { get; set; }

        private DefaultFlyout defaultFlyout = DefaultValuesStore.PreferredDefaultFlyout;

        public DefaultFlyout DefaultFlyout
        {
            get => defaultFlyout;
            set
            {
                if (SetProperty(ref defaultFlyout, value))
                {
                    OnDefaultFlyoutChanged();
                }
            }
        }

        private BindablePoint defaultFlyoutPosition;

        public BindablePoint DefaultFlyoutPosition
        {
            get => defaultFlyoutPosition;
            private set
            {
                defaultFlyoutPosition = value;
                defaultFlyoutPosition.ValueChanged += (s, _) =>
                {
                    AppDataHelper.SavePropertyValue(s.ToString(), nameof(AppDataHelper.DefaultFlyoutPosition));
                };
            }
        }

        private bool runAtStartup;

        public bool RunAtStartup
        {
            get => runAtStartup;
            set
            {
                if (SetProperty(ref runAtStartup, value))
                {
                    OnRunAtStartupChanged();
                }
            }
        }

        #endregion

        public void Initialize()
        {
            FlyoutWindow = new FlyoutWindow();
            CreateWndProc();

            RenderLoopFix.Initialize();

            UIManager = new UIManager();
            UIManager.Initialize(FlyoutWindow);

            NativeFlyoutHandler.Instance.NativeFlyoutShown += (_, _) => OnNativeFlyoutShown();
            KeyboardHook = new KeyboardHook();

            #region App Data

            var adEnabled = AppDataHelper.AudioModuleEnabled;
            var apmdEnabled = AppDataHelper.AirplaneModeModuleEnabled;
            var lkkyEnabled = AppDataHelper.LockKeysModuleEnabled;
            var brEnabled = AppDataHelper.BrightnessModuleEnabled;

            DefaultFlyout = AppDataHelper.DefaultFlyout;

            DefaultFlyoutPosition = AppDataHelper.DefaultFlyoutPosition;

            async void getStartupStatus()
            {
                RunAtStartup = await StartupHelper.GetRunAtStartupEnabled();
            }

            getStartupStatus();

            #endregion

            #region Initiate Helpers

            AudioFlyoutHelper = new AudioFlyoutHelper() { IsEnabled = adEnabled };
            AirplaneModeFlyoutHelper = new AirplaneModeFlyoutHelper() { IsEnabled = apmdEnabled };
            LockKeysFlyoutHelper = new LockKeysFlyoutHelper() { IsEnabled = lkkyEnabled };
            BrightnessFlyoutHelper = new BrightnessFlyoutHelper() { IsEnabled = brEnabled };

            AudioFlyoutHelper.ShowFlyoutRequested += ShowFlyout;
            AirplaneModeFlyoutHelper.ShowFlyoutRequested += ShowFlyout;
            LockKeysFlyoutHelper.ShowFlyoutRequested += ShowFlyout;
            BrightnessFlyoutHelper.ShowFlyoutRequested += ShowFlyout;

            #endregion

            HasInitialized = true;
            Initialized?.Invoke(null, null);
        }

        private void OnNativeFlyoutShown()
        {
            if ((DefaultFlyout == DefaultFlyout.ModernFlyouts && Handled()) || DefaultFlyout == DefaultFlyout.None)
            {
                NativeFlyoutHandler.Instance.HideNativeFlyout();
            }
            else
            {
                NativeFlyoutHandler.Instance.ShowNativeFlyout();
            }
        }

        private void ShowFlyout(FlyoutHelperBase helper)
        {
            if (DefaultFlyout != DefaultFlyout.ModernFlyouts || !helper.IsEnabled)
            {
                return;
            }

            FlyoutWindow.StopHideTimer();

            NativeFlyoutHandler.Instance.VerifyNativeFlyoutCreated();

            if (helper.AlwaysHandleDefaultFlyout)
            {
                NativeFlyoutHandler.Instance.HideNativeFlyout();
            }

            FlyoutWindow.FlyoutHelper = helper;
            FlyoutWindow.Visible = true;
            FlyoutWindow.StartHideTimer();
        }

        private bool Handled()
        {
            if (FlyoutWindow.FlyoutHelper is FlyoutHelperBase helper)
            {
                bool canHandle = helper.AlwaysHandleDefaultFlyout && helper.IsEnabled;
                bool shouldHandle = FlyoutWindow.Visible;
                return canHandle && shouldHandle;
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
                NativeFlyoutHandler.Instance.HideNativeFlyout();
            }

            AppDataHelper.DefaultFlyout = defaultFlyout;
        }

        private void OnRunAtStartupChanged()
        {
            StartupHelper.SetRunAtStartupEnabled(runAtStartup);
        }

        private const int MA_NOACTIVATE = 3;

        private void CreateWndProc()
        {
            ShellMessageHookHandler _ = new();

            var wih = new WindowInteropHelper(FlyoutWindow);
            var hWnd = wih.EnsureHandle();

            WndProcHookManager.OnHwndCreated(hWnd);

            WndProcHookManager.RegisterCallbackForMessage(WM_MOUSEACTIVATE,
                (IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) =>
                {
                    handled = true;

                    return new IntPtr(MA_NOACTIVATE);
                });

            WndProcHookManager.RegisterCallbackForMessage(WM_QUERYENDSESSION,
                (IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) =>
                {
                    RelaunchHelper.RegisterApplicationRestart(
                        JumpListHelper.arg_appupdated,
                        RelaunchHelper.RestartFlags.RESTART_NO_CRASH |
                        RelaunchHelper.RestartFlags.RESTART_NO_HANG |
                        RelaunchHelper.RestartFlags.RESTART_NO_REBOOT);

                    return IntPtr.Zero;
                });

            WndProcHookManager.RegisterCallbackForMessage(WM_EXITSIZEMOVE,
                (IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) =>
                {
                    FlyoutWindow.SaveFlyoutPosition();

                    return IntPtr.Zero;
                });

            var source = HwndSource.FromHwnd(hWnd);
            source.AddHook(WndProcHookManager.TryHandleWindowMessage);
            SetToolWindow(hWnd);
        }

        public static void SafelyExitApplication()
        {
            NativeFlyoutHandler.Instance.ShowNativeFlyout();
            Environment.Exit(0);
        }

        public static void ShowSettingsWindow()
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                Instance.SettingsWindow ??= new SettingsWindow();
                Instance.SettingsWindow.Show();
                Instance.SettingsWindow.Activate();
                Instance.SettingsWindow.Focus();
            });
        }
    }

    public enum DefaultFlyout
    {
        WindowsDefault = 0,
        ModernFlyouts = 1,
        None = 2
    }
}
