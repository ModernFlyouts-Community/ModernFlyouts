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
                return new ValidationResult(false, $"Value cannot be coverted to string.");

            Point point = default;
            try
            {
                point = Point.Parse(strValue);
            }
            catch 
            {
                return new ValidationResult(false, $"Please enter the point in the format 'X,Y' (for e.g. 50,60).");
            }

            if (point.X < int.MinValue || point.X > int.MaxValue || point.Y < int.MinValue || point.Y > int.MaxValue)
            {
                return new ValidationResult(false, $"Please enter X,Y values in the range {int.MinValue} - {int.MaxValue}.");
            }

            return new ValidationResult(true, null);
        }
    }
}
