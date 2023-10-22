using sdl_csharp.Model;
using sdl_csharp.ViewModel;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace sdl_csharp.Converter
{
    public class PlaylistProgressVisibilityConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            EntryViewModel entry = (EntryViewModel)value;

            if (entry.Type is Model.Entry.EntryType.SINGLE) return false;

            EntryPlaylistData data = (entry.Data as EntryPlaylistData);

            return true;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
