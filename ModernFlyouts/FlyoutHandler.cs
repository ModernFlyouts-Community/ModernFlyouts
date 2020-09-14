using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

namespace ModernFlyouts
{
    public class FlyoutHandler : INotifyPropertyChanged
    {
        enum HookMessageEnum : uint
        {
            HOOK_MEDIA_PLAYPAUSE = 917504,
            HOOK_MEDIA_PREVIOUS = 786432,
            HOOK_MEDIA_NEXT = 720896,
            HOOK_MEDIA_STOP = 851968,
            HOOK_MEDIA_VOLPLUS = 655360,
            HOOK_MEDIA_VOLMINUS = 589824,
            HOOK_MEDIA_VOLMUTE = 524288
        }

        DispatcherTimer rehooktimer;
        uint messageShellHookId;

        #region Properties

        public static FlyoutHandler Instance { get; set; }

        public KeyboardHook KeyboardHook { get; private set; }

        public DUIHook DUIHook { get; private set; }

        public FlyoutWindow FlyoutWindow { get; set; }

        public SettingsWindow SettingsWindow { get; set; }

        public AudioFlyoutHelper AudioFlyoutHelper { get; set; }

        public AirplaneModeFlyoutHelper AirplaneModeFlyoutHelper { get; set; }

        public LockKeysFlyoutHelper LockKeysFlyoutHelper { get; set; }

        public BrightnessFlyoutHelper BrightnessFlyoutHelper { get; set; }

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


        private bool topBarEnabled = true;

        public bool TopBarEnabled
        {
            get { return topBarEnabled; }
            set
            {
                if (topBarEnabled != value)
                {
                    topBarEnabled = value;
                    OnPropertyChanged();
                    AppDataHelper.TopBarEnabled = value;
                }
            }
        }

        private bool runAtStartup = false;

        public bool RunAtStartup
        {
            get { return runAtStartup; }
            set
            {
                if (runAtStartup != value)
                {
                    runAtStartup = value;
                    OnPropertyChanged();
                    OnRunAtStartupChanged();
                }
            }
        }

        #endregion

        public void Initialize()
        {
            DUIHandler.ForceFindDUIAndHide();

            FlyoutWindow = new FlyoutWindow();
            CreateWndProc();

            SystemTheme.SystemThemeChanged += OnSystemThemeChange;
            SystemTheme.Initialize();

            DUIHook = new DUIHook();
            DUIHook.Hook();
            DUIHook.DUIShown += DUIShown;
            DUIHook.DUIDestroyed += DUIDestroyed;
            rehooktimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(3), IsEnabled = false };
            rehooktimer.Tick += (_, __) => TryRehook();

            KeyboardHook = new KeyboardHook();

            #region Load Settings

            var adEnabled = AppDataHelper.AudioModuleEnabled;
            var apmdEnabled = AppDataHelper.AirplaneModeModuleEnabled;
            var lkkyEnabled = AppDataHelper.LockKeysModuleEnabled;
            var brEnabled = AppDataHelper.BrightnessModuleEnabled;
            var defaultFlyoutString = AppDataHelper.DefaultFlyout;

            if (Enum.TryParse(defaultFlyoutString, true, out DefaultFlyout _defaultFlyout))
            {
                DefaultFlyout = _defaultFlyout;
            }
            else
            {
                AppDataHelper.DefaultFlyout = DefaultFlyout.ToString();
            }

            TopBarEnabled = AppDataHelper.TopBarEnabled;

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
        }

        private void DUIDestroyed()
        {
            rehooktimer.Start();
        }

        private void TryRehook()
        {
            if (DUIHandler.GetAllInfos())
            {
                rehooktimer.Stop();
                DUIHook.Rehook();
            }
        }

        private void DUIShown()
        {
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

        private void ShowFlyout(FlyoutHelperBase helper)
        {
            if (DefaultFlyout != DefaultFlyout.ModernFlyouts || !helper.IsEnabled)
            {
                return;
            }

            FlyoutWindow.StopHideTimer();

            DUIHandler.VerifyDUI();

            if (helper.AlwaysHandleDefaultFlyout)
            {
                DUIHandler.FindDUIAndHide();
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
                DUIHandler.FindDUIAndHide();
            }

            AppDataHelper.DefaultFlyout = defaultFlyout.ToString();
        }

        private void OnRunAtStartupChanged()
        {
            StartupHelper.SetRunAtStartupEnabled(runAtStartup);
        }

        private const int MA_NOACTIVATE = 0x3;
        private const int WM_MOUSEACTIVATE = 0x21;

        private void CreateWndProc()
        {
            var wih = new WindowInteropHelper(FlyoutWindow);
            var hWnd = wih.EnsureHandle();
            var source = HwndSource.FromHwnd(hWnd);
            source.AddHook(WndProc);
            NativeMethods.SetToolWindow(hWnd);

            NativeMethods.RegisterShellHookWindow(hWnd);
            messageShellHookId = NativeMethods.RegisterWindowMessage("SHELLHOOK");
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_MOUSEACTIVATE)
            {
                handled = true;
                return new IntPtr(MA_NOACTIVATE);
            }

            if (msg == messageShellHookId)
            {
                if (wParam == (IntPtr)55)
                {
                    //Brightness
                    BrightnessFlyoutHelper?.OnExternalUpdated();
                }
                else if (wParam == (IntPtr)12)
                {
                    //Volume
                    AudioFlyoutHelper?.OnExternalUpdated(
                        (int)lParam == (int)HookMessageEnum.HOOK_MEDIA_NEXT || 
                        (int)lParam == (int)HookMessageEnum.HOOK_MEDIA_PREVIOUS || 
                        (int)lParam == (int)HookMessageEnum.HOOK_MEDIA_PLAYPAUSE || 
                        (int)lParam == (int)HookMessageEnum.HOOK_MEDIA_STOP);
                }
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
                ModernWpf.ThemeManager.SetRequestedTheme(FlyoutWindow.TrayToolTip, theme);
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