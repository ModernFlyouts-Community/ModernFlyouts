using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace ModernFlyouts.Converters
{
    public class VisibleIfTrue : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool) value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
