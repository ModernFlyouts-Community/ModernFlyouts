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

            Point point;
            bool canConvert = false;
            try
            {
                point = Point.Parse(strValue);
                canConvert = true;
            }
            catch { }

            return canConvert ? new ValidationResult(true, null) : new ValidationResult(false, $"Please enter in the format 'x,y' (for e.g. 50,60)");
        }
    }
}
