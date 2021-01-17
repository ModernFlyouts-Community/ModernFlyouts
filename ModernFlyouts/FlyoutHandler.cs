using Microsoft.Toolkit.Mvvm.ComponentModel;
using ModernFlyouts.AppLifecycle;
using ModernFlyouts.Core.Display;
using ModernFlyouts.Core.Interop;
using ModernFlyouts.Core.UI;
using ModernFlyouts.Helpers;
using ModernFlyouts.Interop;
using ModernFlyouts.UI;
using ModernFlyouts.UI.Media;
using ModernFlyouts.Views;
using ModernFlyouts.Workarounds;
using ModernWpf;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using static ModernFlyouts.Core.Interop.NativeMethods;

namespace ModernFlyouts
{
    public class FlyoutHandler : ObservableObject
    {
        public static event EventHandler Initialized;

        private bool _isPreferredMonitorChanging;

        #region Properties

        public static FlyoutHandler Instance { get; set; }

        public static bool HasInitialized { get; private set; }

        public KeyboardHook KeyboardHook { get; private set; }

        public FlyoutWindow OnScreenFlyoutWindow { get; set; }

        public FlyoutView OnScreenFlyoutView { get; set; }

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

        private DisplayMonitor onScreenFlyoutPreferredMonitor;

        public DisplayMonitor OnScreenFlyoutPreferredMonitor
        {
            get => onScreenFlyoutPreferredMonitor;
            set
            {
                if (SetProperty(ref onScreenFlyoutPreferredMonitor, value))
                {
                    if (!_isPreferredMonitorChanging && HasInitialized)
                    {
                        MoveFlyoutToAnotherMonitor();
                    }

                    AppDataHelper.PreferredDisplayMonitorId = value.DeviceId;
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
            DisplayManager.Initialize();
            var preferredDisplayMonitorId = AppDataHelper.PreferredDisplayMonitorId;

            UIManager = new UIManager();
            UIManager.Initialize();

            CreateOnScreenFlyoutWindow();

            CreateWndProc();

            RenderLoopFix.Initialize();

            NativeFlyoutHandler.Instance.NativeFlyoutShown += (_, _) => OnNativeFlyoutShown();
            KeyboardHook = new KeyboardHook();

            #region App Data

            var adEnabled = AppDataHelper.AudioModuleEnabled;
            var apmdEnabled = AppDataHelper.AirplaneModeModuleEnabled;
            var lkkyEnabled = AppDataHelper.LockKeysModuleEnabled;
            var brEnabled = AppDataHelper.BrightnessModuleEnabled;

            DefaultFlyout = AppDataHelper.DefaultFlyout;

            DefaultFlyoutPosition = AppDataHelper.DefaultFlyoutPosition;

            if (DisplayManager.Instance.DisplayMonitors
                .Any(x => x.DeviceId == preferredDisplayMonitorId))
            {
                OnScreenFlyoutPreferredMonitor = DisplayManager.Instance.DisplayMonitors
                    .First(x => x.DeviceId == preferredDisplayMonitorId);
            }
            else
            {
                OnScreenFlyoutPreferredMonitor = DisplayManager.Instance.PrimaryDisplayMonitor;
                AlignFlyout();
            }

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
            Initialized?.Invoke(this, EventArgs.Empty);

            DisplayManager.Instance.DisplayUpdated += Instance_DisplayUpdated;
        }

        private void Instance_DisplayUpdated(object sender, EventArgs e)
        {
            if (!DisplayManager.Instance.DisplayMonitors.Any(x => x == onScreenFlyoutPreferredMonitor))
            {
                OnScreenFlyoutPreferredMonitor = DisplayManager.Instance.PrimaryDisplayMonitor;
                AlignFlyout();
            }
        }

        private void CreateOnScreenFlyoutWindow()
        {
            OnScreenFlyoutView = new()
            {
                FlyoutTopBar = new()
            };

            ZBandID zbid = ZBandID.Default;

            using (var proc = Process.GetCurrentProcess())
            {
                var isImmersive = IsImmersiveProcess(proc.Handle);
                var hasUiAccess = HasUiAccessProcess(proc.Handle);

                zbid = isImmersive ? ZBandID.AboveLockUX : (hasUiAccess ? ZBandID.UIAccess : ZBandID.Desktop);
            }

            var flyoutWindow = new FlyoutWindow()
            {
                Activatable = false,
                Content = OnScreenFlyoutView,
                ZBandID = zbid,
                Alignment = FlyoutWindowAlignments.Bottom | FlyoutWindowAlignments.Right,
                FlyoutWindowType = FlyoutWindowType.OnScreen,
                PlacementMode = FlyoutWindowPlacementMode.Manual,
                Margin = new(10),
                Offset = new(20, 2, 20, 30),
                IsTimeoutEnabled = true
            };

            flyoutWindow.DragMoved += (s, e) =>
            {
                SaveOnScreenFlyoutPosition();
                UpdatePreferredMonitor();
            };

            OnScreenFlyoutWindow = flyoutWindow;

            flyoutPosition = AppDataHelper.FlyoutPosition;

            AlignFlyout(flyoutPosition);

            BindingOperations.SetBinding(flyoutWindow, FlyoutWindow.PreferredDisplayMonitorProperty, new Binding()
            {
                Source = this,
                Path = new PropertyPath(nameof(OnScreenFlyoutPreferredMonitor)),
                Mode = BindingMode.OneWay
            });

            BindingOperations.SetBinding(flyoutWindow, ThemeManager.RequestedThemeProperty, new Binding()
            {
                Source = UIManager,
                Path = new PropertyPath(nameof(UIManager.ActualFlyoutTheme)),
                Mode = BindingMode.OneWay
            });

            BindingOperations.SetBinding(flyoutWindow, FlyoutWindow.TimeoutProperty, new Binding()
            {
                Source = UIManager,
                Path = new PropertyPath(nameof(UIManager.FlyoutTimeout)),
                Mode = BindingMode.OneWay
            });
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

            OnScreenFlyoutWindow.StopCloseTimer();

            NativeFlyoutHandler.Instance.VerifyNativeFlyoutCreated();

            if (helper.AlwaysHandleDefaultFlyout)
            {
                NativeFlyoutHandler.Instance.HideNativeFlyout();
            }

            OnScreenFlyoutView.FlyoutHelper = helper;
            OnScreenFlyoutWindow.IsOpen = true;
            OnScreenFlyoutWindow.StartCloseTimer();
        }

        private bool Handled()
        {
            if (OnScreenFlyoutView.FlyoutHelper is FlyoutHelperBase helper)
            {
                bool canHandle = helper.AlwaysHandleDefaultFlyout && helper.IsEnabled;
                bool shouldHandle = OnScreenFlyoutWindow.IsOpen;
                return canHandle && shouldHandle;
            }
            return false;
        }

        private void OnDefaultFlyoutChanged()
        {
            if (defaultFlyout != DefaultFlyout.ModernFlyouts)
            {
                OnScreenFlyoutWindow.IsOpen = false;
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

        private void CreateWndProc()
        {
            ShellMessageHookHandler shellHook = new();
            var hookManager = WndProcHookManager.GetForBandWindow(OnScreenFlyoutWindow);
            hookManager.RegisterHookHandler(shellHook);

            var displayManager = DisplayManager.Instance;
            hookManager.RegisterHookHandlerForMessage((uint)WindowMessage.WM_SETTINGCHANGE, displayManager);
            hookManager.RegisterHookHandlerForMessage((uint)WindowMessage.WM_DISPLAYCHANGE, displayManager);

            OnScreenFlyoutWindow.CreateWindow();

            hookManager.RegisterCallbackForMessage((uint)WindowMessage.WM_QUERYENDSESSION,
                (_, _, _, _) =>
                {
                    RelaunchHelper.RegisterApplicationRestart(
                        JumpListHelper.arg_appupdated,
                        RelaunchHelper.RestartFlags.RESTART_NO_CRASH |
                        RelaunchHelper.RestartFlags.RESTART_NO_HANG |
                        RelaunchHelper.RestartFlags.RESTART_NO_REBOOT);

                    AppLifecycleManager.PrepareToDie();
                    return IntPtr.Zero;
                });
        }

        private void MoveFlyoutToAnotherMonitor()
        {
            if (OnScreenFlyoutWindow.PlacementMode != FlyoutWindowPlacementMode.Manual)
                return;

            var currentMonitor = DisplayManager.Instance
                .GetDisplayMonitorFromPoint(flyoutPosition.ToPoint());
            var currentPos = flyoutPosition.ToPoint();
            var currentMonitorPos = currentMonitor.Bounds.TopLeft;
            currentPos.Offset(-currentMonitorPos.X, -currentMonitorPos.Y);
            var toMonitorPos = onScreenFlyoutPreferredMonitor.Bounds.TopLeft;
            currentPos.Offset(toMonitorPos.X, toMonitorPos.Y);

            AlignFlyout(currentPos, true, false);
        }

        private BindablePoint flyoutPosition;

        public void AlignFlyout(BindablePoint toPos = null)
        {
            toPos ??= DefaultFlyoutPosition;

            AlignFlyout(toPos.ToPoint(), toPos != flyoutPosition);
        }

        private void AlignFlyout(Point toPos, bool savePos = true, bool updateMonitor = true)
        {
            OnScreenFlyoutWindow.Left = toPos.X;
            OnScreenFlyoutWindow.Top = toPos.Y;

            if (OnScreenFlyoutWindow.PlacementMode == FlyoutWindowPlacementMode.Manual)
                OnScreenFlyoutWindow.AlignToPosition();

            if (savePos)
            {
                SaveOnScreenFlyoutPosition();
            }
            if (updateMonitor)
            {
                UpdatePreferredMonitor();
            }
        }

        private void SaveOnScreenFlyoutPosition()
        {
            flyoutPosition.X = OnScreenFlyoutWindow.Left;
            flyoutPosition.Y = OnScreenFlyoutWindow.Top;

            AppDataHelper.SavePropertyValue(flyoutPosition.ToString(), nameof(AppDataHelper.FlyoutPosition));
        }

        private void UpdatePreferredMonitor()
        {
            if (OnScreenFlyoutWindow.PlacementMode != FlyoutWindowPlacementMode.Manual)
                return;

            _isPreferredMonitorChanging = true;
            OnScreenFlyoutPreferredMonitor = DisplayManager.Instance
                .GetDisplayMonitorFromPoint(flyoutPosition.ToPoint());
            _isPreferredMonitorChanging = false;
        }

        public static void SafelyExitApplication()
        {
            NativeFlyoutHandler.Instance.ShowNativeFlyout();
            Environment.Exit(0);
        }

        public static void ShowSettingsWindow()
        {
            Application.Current.Dispatcher.Invoke(() =>
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
