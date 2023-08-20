using CommunityToolkit.Mvvm.ComponentModel;
using ModernFlyouts.AppLifecycle;
using ModernFlyouts.Assets;
using ModernFlyouts.Controls;
using ModernFlyouts.Core.Display;
using ModernFlyouts.Core.Interop;
using ModernFlyouts.Core.UI;
using ModernFlyouts.Core.Utilities;
using ModernFlyouts.Helpers;
using ModernFlyouts.Interop;
using ModernFlyouts.UI;
using ModernFlyouts.UI.Media;
using ModernFlyouts.Views;
using ModernFlyouts.Workarounds;
using ModernWpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static ModernFlyouts.Core.Interop.NativeMethods;

namespace ModernFlyouts
{
    public class FlyoutHandler : ObservableObject
    {
        public static event EventHandler Initialized;

        private bool _isPreferredMonitorChanging;
        private bool _savePreferredMonitor = true;

        private List<FlyoutHelperBase> flyoutHelpers = new();
        private AirplaneModeWatcher airplaneModeWatcher = new();
        private FlyoutTriggerData prevTriggerData;

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

        private Orientation onScreenFlyoutOrientation = DefaultValuesStore.FlyoutOrientation;

        public Orientation OnScreenFlyoutOrientation
        {
            get => onScreenFlyoutOrientation;
            set
            {
                if (SetProperty(ref onScreenFlyoutOrientation, value))
                {
                    OnFlyoutOrientationChanged();
                }
            }
        }

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
                    if (HasInitialized)
                    {
                        if (!_isPreferredMonitorChanging)
                            MoveFlyoutToAnotherMonitor();

                        if (_savePreferredMonitor)
                            AppDataHelper.PreferredDisplayMonitorId = value.DeviceId;
                    }
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

            onScreenFlyoutOrientation = AppDataHelper.FlyoutOrientation;
            
            OnScreenFlyoutView.ContentStackPanel.Orientation = OnScreenFlyoutOrientation switch
            {
                Orientation.Vertical => Orientation.Horizontal,
                _ => Orientation.Vertical,
            };

            if (DisplayManager.Instance.DisplayMonitors
                .Any(x => x.DeviceId == preferredDisplayMonitorId))
            {
                OnScreenFlyoutPreferredMonitor = DisplayManager.Instance.DisplayMonitors
                    .First(x => x.DeviceId == preferredDisplayMonitorId);
            }
            else
            {
                OnScreenFlyoutPreferredMonitor = DisplayManager.Instance.PrimaryDisplayMonitor;
                if (onScreenFlyoutPreferredMonitor.Bounds.Contains(flyoutPosition.ToPoint()))
                {
                    AlignFlyout(flyoutPosition);
                }
                else { AlignFlyout(); }
            }

            async void getStartupStatus()
            {
                RunAtStartup = await StartupHelper.GetRunAtStartupEnabled();
            }

            getStartupStatus();

            #endregion

            #region Initiate Helpers

            AudioFlyoutHelper = new AudioFlyoutHelper(OnScreenFlyoutOrientation) { IsEnabled = adEnabled };
            AirplaneModeFlyoutHelper = new AirplaneModeFlyoutHelper() { IsEnabled = apmdEnabled };
            LockKeysFlyoutHelper = new LockKeysFlyoutHelper() { IsEnabled = lkkyEnabled };
            BrightnessFlyoutHelper = new BrightnessFlyoutHelper() { IsEnabled = brEnabled };

            flyoutHelpers.Add(AudioFlyoutHelper);
            flyoutHelpers.Add(AirplaneModeFlyoutHelper);
            flyoutHelpers.Add(LockKeysFlyoutHelper);
            flyoutHelpers.Add(BrightnessFlyoutHelper);

            foreach (var flyoutHelper in flyoutHelpers)
            {
                flyoutHelper.ShowFlyoutRequested += ShowFlyout;
            }

            #endregion

            HasInitialized = true;
            Initialized?.Invoke(this, EventArgs.Empty);

            DisplayManager.Instance.DisplayUpdated += Instance_DisplayUpdated;
            airplaneModeWatcher.Changed += AirplaneModeWatcher_Changed;
            airplaneModeWatcher.Start();
        }

        private void AirplaneModeWatcher_Changed(object sender, AirplaneModeChangedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                FlyoutTriggerData triggerData = new()
                {
                    TriggerType = FlyoutTriggerType.AirplaneMode,
                    Data = e.IsEnabled
                };

                ProcessFlyoutTrigger(triggerData);
            });
        }

        private void Instance_DisplayUpdated(object sender, EventArgs e)
        {
            if (!DisplayManager.Instance.DisplayMonitors.Any(x => x == onScreenFlyoutPreferredMonitor))
            {
                _savePreferredMonitor = false;
                OnScreenFlyoutPreferredMonitor = DisplayManager.Instance.PrimaryDisplayMonitor;
                AlignFlyout();
                _savePreferredMonitor = true;
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
                FlyoutWindowType = FlyoutWindowType.OnScreen,
                Offset = UIManager.FlyoutShadowMargin,
                IsTimeoutEnabled = true,
            };

            flyoutWindow.DragMoved += (s, e) =>
            {
                SaveOnScreenFlyoutPosition();
                UpdatePreferredMonitor();
            };

            OnScreenFlyoutWindow = flyoutWindow;

            BindingOperations.SetBinding(flyoutWindow, FlyoutWindow.PlacementModeProperty, new Binding()
            {
                Source = UIManager,
                Path = new PropertyPath(nameof(UIManager.OnScreenFlyoutWindowPlacementMode)),
                Mode = BindingMode.TwoWay
            });

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

            BindingOperations.SetBinding(flyoutWindow, FlyoutWindow.AlignmentProperty, new Binding()
            {
                Source = UIManager,
                Path = new PropertyPath(nameof(UIManager.OnScreenFlyoutWindowAlignment)),
                Mode = BindingMode.OneWay
            });

            BindingOperations.SetBinding(flyoutWindow, FlyoutWindow.MarginProperty, new Binding()
            {
                Source = UIManager,
                Path = new PropertyPath(nameof(UIManager.OnScreenFlyoutWindowMargin)),
                Mode = BindingMode.OneWay
            });

            BindingOperations.SetBinding(flyoutWindow, FlyoutWindow.ExpandDirectionProperty, new Binding()
            {
                Source = UIManager,
                Path = new PropertyPath(nameof(UIManager.OnScreenFlyoutWindowExpandDirection)),
                Mode = BindingMode.OneWay
            });

            BindingOperations.SetBinding(flyoutWindow, FlyoutWindow.FlyoutAnimationEnabledProperty, new Binding()
            {
                Source = UIManager,
                Path = new PropertyPath(nameof(UIManager.FlyoutAnimationEnabled)),
                Mode = BindingMode.OneWay
            });
        }

        private void OnNativeFlyoutShown()
        {
            if (DefaultFlyout == DefaultFlyout.ModernFlyouts)
            {
                if (Handled())
                    NativeFlyoutHandler.Instance.HideNativeFlyout();

                if (prevTriggerData != null
                    && !prevTriggerData.IsExpired)
                {
                    prevTriggerData.IsExpired = true;
                    return;
                }

                // When the native flyout is triggered by some factors
                // that are neither ShellHook messages or airplane mode changes
                // (that implies the native flyout is triggered by
                // either touchpad gestures or audio device buttons),
                // we show the volume flyout as a fallback.
                // Only volume flyout is shown as a fallback
                // because other triggers are always detected perfectly.
                ProcessFlyoutTrigger(new()
                {
                    TriggerType = FlyoutTriggerType.Volume,
                    IsExpired = true
                });
            }
            else if (DefaultFlyout == DefaultFlyout.None)
            {
                NativeFlyoutHandler.Instance.HideNativeFlyout();
            }
            else
            {
                NativeFlyoutHandler.Instance.ShowNativeFlyout();
            }
        }

        internal void ProcessFlyoutTrigger(FlyoutTriggerData triggerData = null)
        {
            Debug.WriteLine("Rattled!");
            triggerData ??= prevTriggerData ?? new();

            bool canHandle = false;
            FlyoutHelperBase flyoutHelper = null;

            foreach (var helper in flyoutHelpers)
            {
                canHandle = helper.CanHandleNativeOnScreenFlyout(triggerData);
                if (canHandle)
                {
                    flyoutHelper = helper;
                    break;
                }
            }

            if (canHandle && flyoutHelper != null)
            {
                ShowFlyout(flyoutHelper);
            }

            prevTriggerData = triggerData;
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

        private void OnFlyoutOrientationChanged()
        {
            AppDataHelper.FlyoutOrientation = OnScreenFlyoutOrientation;
            OnScreenFlyoutView.ContentStackPanel.Orientation = OnScreenFlyoutOrientation switch
            {
                Orientation.Vertical => Orientation.Horizontal,
                _ => Orientation.Vertical,
            };
            AudioFlyoutHelper.OnFlyoutOrientationChanged(OnScreenFlyoutOrientation);
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
