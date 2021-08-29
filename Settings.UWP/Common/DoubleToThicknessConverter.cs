using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace AppUIBasics.Common
{
    class DoubleToThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is double?)
            {
                return new Thickness((double)value);
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
