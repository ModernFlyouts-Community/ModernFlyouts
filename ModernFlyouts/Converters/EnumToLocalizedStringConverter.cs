using ModernFlyouts.Helpers;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ModernFlyouts.Converters
{
    public class EnumToLocalizedStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Enum @enum)
            {
                return LocalizationHelper.GetLocalisedEnumValue(@enum);
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
