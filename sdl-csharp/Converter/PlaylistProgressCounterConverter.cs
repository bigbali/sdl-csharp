using sdl_csharp.ViewModel;
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace sdl_csharp.Converter
{
    public class PlaylistProgressCounterConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int index = (int)value;
            EntryViewModel vm = ((TextBox)parameter).DataContext as EntryViewModel;

            return $"{Math.Max(index, 1)}/{vm.Data.PlaylistMemberCount}";
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
