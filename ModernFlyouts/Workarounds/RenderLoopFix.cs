using Microsoft.Win32;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ModernFlyouts.Workarounds
{
    /// <summary>
    /// Temporary workaround for WM_PAINT Storm issue - #12
    /// </summary>
    internal class RenderLoopFix
    {
        public static void Initialize()
        {
            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
        }

        private static void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            ApplyFix();
        }

        private static void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            ApplyFix();
        }

        public static void ApplyFix()
        {
            if (FlyoutHandler.HasInitialized)
            {
                var flyoutWindow = FlyoutHandler.Instance.OnScreenFlyoutWindow;
                flyoutWindow?.Show();

                Task.Run(() =>
                {
                    Thread.Sleep(1000);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (flyoutWindow != null && !flyoutWindow.IsOpen)
                        {
                            flyoutWindow?.Hide();
                        }
                    });
                });
            }
        }

        ~RenderLoopFix()
        {
            SystemEvents.SessionSwitch -= SystemEvents_SessionSwitch;
            SystemEvents.PowerModeChanged -= SystemEvents_PowerModeChanged;
        }
    }
}
