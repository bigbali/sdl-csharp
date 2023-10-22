using sdl_csharp.Model;
using sdl_csharp.ViewModel;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace sdl_csharp.Converter
{
    public class EntryOutputConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //uint index = (uint)value;
            //TextBlock entryout = ((TextBlock)parameter);

            //entryout.Text += $"{index}{Environment.NewLine}";

            //return entryout.Text;
            return value;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
