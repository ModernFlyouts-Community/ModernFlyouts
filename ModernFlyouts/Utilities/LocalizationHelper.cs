using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace ModernFlyouts.Utilities
{
    public class LocalizationHelper
    {
        public static string GetLocalisedEnumValue(Enum value)
        {
            var resourceId = $"Enums.{value.GetType().Name}.{value}";
            Debug.WriteLine(resourceId);
            try
            {
                return Properties.Strings.ResourceManager.GetString(resourceId, Properties.Strings.Culture);
            }
            catch
            {
                return value.ToString();
            }
        }
    }

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
