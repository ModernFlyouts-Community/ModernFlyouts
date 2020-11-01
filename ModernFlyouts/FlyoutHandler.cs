using Microsoft.Toolkit.Mvvm.ComponentModel;
using ModernFlyouts.Helpers;
using ModernFlyouts.Interop;
using ModernFlyouts.UI;
using ModernFlyouts.Utilities;
using ModernFlyouts.Workarounds;
using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

namespace ModernFlyouts
{
    public class FlyoutHandler : ObservableObject
    {

        public static event EventHandler Initialized;

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

        public static bool HasInitialized;

        public KeyboardHook KeyboardHook { get; private set; }

        public DUIHook DUIHook { get; private set; }

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

        private Point defaultFlyoutPosition = DefaultValuesStore.DefaultFlyoutPosition;

        public Point DefaultFlyoutPosition
        {
            get => defaultFlyoutPosition;
            set
            {
                if (SetProperty(ref defaultFlyoutPosition, value))
                {
                    AppDataHelper.DefaultFlyoutPosition = value;
                }
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

            DUIHook = new DUIHook();
            DUIHook.Hook();
            DUIHook.DUIShown += DUIShown;
            DUIHook.DUIDestroyed += DUIDestroyed;
            rehooktimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(3), IsEnabled = false };
            rehooktimer.Tick += (_, __) => TryRehook();

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

            AppDataHelper.DefaultFlyout = defaultFlyout;
        }

        private void OnRunAtStartupChanged()
        {
            StartupHelper.SetRunAtStartupEnabled(runAtStartup);
        }

        private const int MA_NOACTIVATE = 0x3;
        private const int WM_MOUSEACTIVATE = 0x21;
        private const int WM_EXITSIZEMOVE = 0x0232;

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
                    switch ((long)lParam)
                    {
                        case (long)HookMessageEnum.HOOK_MEDIA_NEXT:
                        case (long)HookMessageEnum.HOOK_MEDIA_PREVIOUS:
                        case (long)HookMessageEnum.HOOK_MEDIA_PLAYPAUSE:
                        case (long)HookMessageEnum.HOOK_MEDIA_STOP:
                            //Media
                            AudioFlyoutHelper?.OnExternalUpdated(true);
                            break;
                        case (long)HookMessageEnum.HOOK_MEDIA_VOLMINUS:
                        case (long)HookMessageEnum.HOOK_MEDIA_VOLMUTE:
                        case (long)HookMessageEnum.HOOK_MEDIA_VOLPLUS:
                            //Volume
                            AudioFlyoutHelper?.OnExternalUpdated(false);
                            break;
                        default:
                            //Ignore mouse side buttons and other keyboard special keys
                            break;
                    }
                }
            }

            if (msg == WM_EXITSIZEMOVE)
            {
                FlyoutWindow.SaveFlyoutPosition();
            }

            return IntPtr.Zero;
        }

        public static void SafelyExitApplication()
        {
            DUIHandler.FindDUIAndShow();
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
