using sdl_csharp.Model;
using sdl_csharp.ViewModel;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace sdl_csharp.Converter
{
    public class PlaylistProgressCounterConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            uint index = (uint)value;
            EntryViewModel vm = ((TextBox)parameter).DataContext as EntryViewModel;

            return $"{index}/{vm.Data.PlaylistMemberCount}";
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
