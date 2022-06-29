using System;
using Windows.Devices.Power;
using Windows.System.Power;
namespace ModernFlyouts.Services
{
    class PowerService
    {
        /// <summary>
        /// Gets power status and usage level in text.
        /// </summary>
        /// <param name="report">A BatteryReport object.</param>
        /// <returns>Returns text representing power usage, or Charging.</returns>
        public static string GetPowerUsageText(BatteryReport report) => report.Status != BatteryStatus.Charging ? CalculateUsage(report) : "Charging";

        /// <summary>
        /// Converts BatterReport info into text to represent power consumption.
        /// </summary>
        /// <param name="report">A BatteryReport object.</param>
        /// <returns>Returns text representing power usage./returns>
        public static string CalculateUsage(BatteryReport report)
        {
            return ((int)Math.Abs((decimal)report.ChargeRateInMilliwatts)) switch
            {
                int i when i > 0 && i <= 8000 => "Very low usage",
                int i when i > 8000 && i <= 12000 => "Low usage",
                int i when i > 12000 && i <= 16000 => "Medium usage",
                int i when i > 16000 && i <= 20000 => "High usage",
                _ => "Very high usage"
            };
        }
    }
}
