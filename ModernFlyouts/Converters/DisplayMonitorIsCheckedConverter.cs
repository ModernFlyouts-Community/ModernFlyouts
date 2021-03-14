using ModernFlyouts.Core.Display;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace ModernFlyouts.Converters
{
    public class DisplayMonitorIsCheckedConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Any(o => o == DependencyProperty.UnsetValue || o == null))
                return null;

            DisplayMonitor value = values[0] as DisplayMonitor;
            DisplayMonitor preferredValue = values[1] as DisplayMonitor;

            return value == preferredValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
