using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Windows.System.Power;

namespace ModernFlyouts.Core.Helpers.Hardware
{
    //https://docs.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.windows.system.power.powermanager?view=windows-app-sdk-1.1
    //https://github.com/microsoft/WindowsAppSDK/blob/5894b4e600bc7801c6e15a0a46f696c5200b2dc8/specs/AppLifecycle/StateNotifications/AppLifecycle%20StateNotifications.md
    internal class BatteryInfo
    {
        public enum BatteryStatus { get }
        public static EffectivePowerMode EffectivePowerMode2 { get; }
        public static EnergySaverStatus EnergySaverStatus { get; }
    }
}
