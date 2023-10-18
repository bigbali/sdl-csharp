using System;
using System.Globalization;
using System.Windows.Data;

namespace sdl_csharp.Converter
{
    class ObjectToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                null => false,
                false => false,
                "" => false,
                0 => false,
                _ => true
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
