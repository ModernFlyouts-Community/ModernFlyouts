using Windows.Devices.Power;
using Fluent.Icons;

namespace ModernFlyouts.Uwp.Helpers
{
    public class BatteryHelper
    {
        /// <summary>
        /// Converts BatterReport info into a percentage value of battery remaining.
        /// </summary>
        /// <param name="report">A BatteryReport object.</param>
        /// <returns>Returns a percentage value of the battery as a string with %.</returns>
        public static string GetPercentageText(BatteryReport report) => GetPercentage(report).ToString("F0") + "%";

        /// <summary>
        /// Gets an icon that represents current battery level.
        /// </summary>
        /// <param name="report">A BatteryReport object.</param>
        /// <returns>Returns a FluentSymbol representing battery level./returns>
        public static FluentSymbol GetPercentageIcon(BatteryReport report)
        {
            return ((int)GetPercentage(report)) switch
            {
                int i when i > 0 && i <= 10 => FluentSymbol.BatteryWarning24,
                int i when i > 10 && i <= 20 => FluentSymbol.Battery124,
                int i when i > 20 && i <= 30 => FluentSymbol.Battery224,
                int i when i > 30 && i <= 40 => FluentSymbol.Battery324,
                int i when i > 40 && i <= 50 => FluentSymbol.Battery424,
                int i when i > 50 && i <= 60 => FluentSymbol.Battery524,
                int i when i > 60 && i <= 70 => FluentSymbol.Battery624,
                int i when i > 70 && i <= 80 => FluentSymbol.Battery724,
                int i when i > 80 && i <= 90 => FluentSymbol.Battery824,
                int i when i > 90 && i < 99 => FluentSymbol.Battery924,
                _ => FluentSymbol.BatteryFull24
            };
        }

        /// <summary>
        /// Converts BatterReport info into a percentage value of battery remaining.
        /// </summary>
        /// <param name="report">A BatteryReport object.</param>
        /// <returns>Returns a percentage value of the battery as a double./returns>
        public static double GetPercentage(BatteryReport report)
        {
            return (((double)report.RemainingCapacityInMilliwattHours / (double)report.FullChargeCapacityInMilliwattHours) * 100);
        }
    }
}
