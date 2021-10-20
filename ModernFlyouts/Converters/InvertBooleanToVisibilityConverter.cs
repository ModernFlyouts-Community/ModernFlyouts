using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ModernFlyouts.Converters
{
    public class InvertBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = false;
            if (value is bool boolean)
            {
                result = boolean;
            }
            else if (value is bool?)
            {
                bool? nullable = (bool?)value;
                result = nullable.HasValue && nullable.Value;
            }

            return result ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
