using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace ModernFlyouts.Utilities
{
    public class PointValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string strValue = Convert.ToString(value);

            if (string.IsNullOrEmpty(strValue))
                return new ValidationResult(false, Properties.Strings.PointValidationRule_StringConvertionErrorMessage);

            Point point = default;
            try
            {
                point = Point.Parse(strValue);
            }
            catch 
            {
                return new ValidationResult(false, Properties.Strings.PointValidationRule_PointFormatInvalidMessage);
            }

            if (point.X < int.MinValue || point.X > int.MaxValue || point.Y < int.MinValue || point.Y > int.MaxValue)
            {
                return new ValidationResult(false, string.Format(Properties.Strings.PointValidationRule_PointOutOfRangeMessage, int.MinValue, int.MaxValue));
            }

            return new ValidationResult(true, null);
        }
    }
}
