using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace ModernFlyouts.Utilities
{
    public class ScrollButtonVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Any(o => o == DependencyProperty.UnsetValue || o == null)) return Visibility.Visible;
            var value = (double)values[0];
            var req = (double)values[1];
            var op = parameter.ToString();

            return op switch
            {
                ">" => value > req ? Visibility.Visible : Visibility.Collapsed,
                "<" => value < req ? Visibility.Visible : Visibility.Collapsed,
                _ => Visibility.Visible,
            };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
