using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using Windows.System.Profile;
using Windows.UI.ViewManagement;

using System.Collections.Generic;
//using ModernFlyouts.WinUI.Helpers;

namespace ModernFlyouts.Core.Helpers.Hardware
{
    /// <summary>
    /// Retrieve system information
    /// And work out what type of device we are running on - Desktop, laptop, tablet
    /// </summary>
    ///

    public static class DeviceInfo
    {
        public static DeviceFormFactorType GetDeviceFormFactorType()
        {
            switch (AnalyticsInfo.VersionInfo.DeviceFamily)
            {
                case "Windows.Mobile":
                    return DeviceFormFactorType.Phone;
                case "Windows.Desktop":
                    return UIViewSettings.GetForCurrentView().UserInteractionMode == UserInteractionMode.Mouse
                        ? DeviceFormFactorType.Desktop
                        : DeviceFormFactorType.Tablet;//win10 only
                case "Windows.Universal":
                    return DeviceFormFactorType.IoT;
                case "Windows.Team":
                    return DeviceFormFactorType.SurfaceHub;
                default:
                    return DeviceFormFactorType.Other;
            }
        }



        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetPhysicallyInstalledSystemMemory(out long TotalMemoryInKilobytes);

        /// <summary>
        /// Total installed memory in Megabyte
        /// </summary>
        /// <returns></returns>
        public static long GetTotalInstalledMemory()
        {
            GetPhysicallyInstalledSystemMemory(out long memKb);
            return (memKb / 1024);
        }




    }

    public enum DeviceFormFactorType
    {
        Phone,
        Desktop,
        Tablet,
        IoT,
        SurfaceHub,
        Other
    }

    //public static bool IsBatteryPresent
    //{
    //    get
    //    {
    //        CoreHelpers.ThrowIfNotXP();

    //        return Power.GetSystemBatteryState().BatteryPresent;
    //    }
    //}

    //[DllImport("powrprof.dll", SetLastError = true)]
    //[return: MarshalAs(UnmanagedType.U1)]
    //static extern bool GetPwrCapabilities(out SYSTEM_POWER_CAPABILITIES systemPowerCapabilites);

    //static SYSTEM_POWER_CAPABILITIES systemPowerCapabilites;
    //static PowerStatus()
    //{
    //    GetPwrCapabilities(out SYSTEM_POWER_CAPABILITIES);
    //}


    /// <summary>
    /// Check out if there is a lid switch present in the system
    /// </summary>
    /// <returns> true if there is a lid switch </returns>
    //public bool LidPresent()
    //{
    //    return systemPowerCapabilites.LidPresent;
    //}


    //if(SystemInformation.PowerStatus.BatteryChargeStatus ==BatteryChargeStatus.NoSystemBattery){
    //  //desktop 
    //}
    //else
    //{
    //    //laptop
    //}


    //public SYSTEM_POWER_CAPABILITIES getSystemPowerCapabilites()
    //{
    //    {
    //        SYSTEM_POWER_CAPABILITIES systemPowerCapabilites;
    //        GetPwrCapabilities(out systemPowerCapabilites);
    //        return systemPowerCapabilites;
    //    }

    //    getSystemPowerCapabilites().LidPresent;

    //}

    ////Is bluetooth present

}
