using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ModernFlyouts.Converters
{
    public class IndexToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int index = (int)value;
            int reqIndex = 0;

            if (parameter != null && int.TryParse(parameter.ToString(), out int result))
            {
                reqIndex = result;
            }

            return index == reqIndex ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}