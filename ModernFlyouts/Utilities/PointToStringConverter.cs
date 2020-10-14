using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ModernFlyouts.Utilities
{
    public class PointToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var point = (Point)value;

            return point.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var point = Point.Parse(value.ToString());
                return point;
            }
            catch { }

            return default(Point);
        }
    }
}
