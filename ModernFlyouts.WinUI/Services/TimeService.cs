using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Devices.Power;
using Windows.System.Power;
namespace ModernFlyouts.Services
{
    class TimeService
    {
        /// <summary>
        /// Gets time remaining for battery to completely discharge or charge depending on state.
        /// </summary>
        /// <param name="report">A BatteryReport object.</param>
        /// <returns>Returns time in a readable format.</returns>
        public static string GetRemaningTimeText(BatteryReport report) => FormatTime(report);

        /// <summary>
        /// Converts TimeSpan into human readable text.
        /// </summary>
        /// <param name="report">A BatteryReport object.</param>
        /// <returns>Returns a string representing TimeSpan in hh:mm format.</returns>
        public static string FormatTime(BatteryReport report)
        {
            try
            {
                Dictionary<string, string> formats = new Dictionary<string, string> { { "00:", "" }, { ":", "h " }, { "00m", "" }, { "0", "" } };
                return formats.Aggregate((CalculateTime(report).ToString(@"hh\:mm") + "m remaining"), (current, replacement) => current.Replace(replacement.Key, replacement.Value));
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Calculates time taken for battery to completely discharge or charge depending on state.
        /// </summary>
        /// <param name="report">A BatteryReport object.</param>
        /// <returns>Returns a TimeSpan object in hh:mm format.</returns>
        public static TimeSpan CalculateTime(BatteryReport report) => report.Status != BatteryStatus.Charging ? GetDischargeTime(report) : GetChargeTime(report);

        /// <summary>
        /// Gets time taken for battery to completely discharge.
        /// </summary>
        /// <param name="report">A BatteryReport object.</param>
        /// <returns>Returns a TimeSpan object..</returns>
        public static TimeSpan GetDischargeTime(BatteryReport report) => TimeSpan.FromSeconds(((((double)report.RemainingCapacityInMilliwattHours - (double)report.FullChargeCapacityInMilliwattHours) * 3600) / (double)report.ChargeRateInMilliwatts));

        /// <summary>
        /// Gets time taken for battery to completely charge.
        /// </summary>
        /// <param name="report">A BatteryReport object.</param>
        /// <returns>Returns a TimeSpan object.</returns>
        public static TimeSpan GetChargeTime(BatteryReport report) => TimeSpan.FromSeconds(((((double)report.FullChargeCapacityInMilliwattHours - (double)report.RemainingCapacityInMilliwattHours) * 3600) / (double)report.ChargeRateInMilliwatts));
    }
}
