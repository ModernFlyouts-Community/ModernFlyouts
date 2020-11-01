﻿using Microsoft.Win32;
using System.Threading;
using System.Threading.Tasks;

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
                var flyoutWindow = FlyoutHandler.Instance.FlyoutWindow;
                flyoutWindow.Show();

                Task.Run(() =>
                {
                    Thread.Sleep(1000);
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        if (!flyoutWindow.Visible)
                        {
                            flyoutWindow.Hide();
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
