using sdl_csharp.Model;
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
            int index = Math.Max((int)value, 1);
            EntryViewModel vm = ((TextBox)parameter).DataContext as EntryViewModel;

            int maxCount = ((IEntryPlaylistData)vm.Data).PlaylistMemberCount;

            // make sure we don't go over PlaylistMemberCount
            int count = Math.Min(index, maxCount);

            return $"{count}/{maxCount}";
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
