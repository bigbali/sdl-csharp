using sdl_csharp.Model.Entry;
using sdl_csharp.Utility;
using System.Windows;
using System.Windows.Controls;
using static sdl_csharp.Model.Entry.Entry;

namespace sdl_csharp.ViewModel
{
    public class EntryViewModel : NotifyPropertyChanged
    {
        private object _data;

        public object Data
        {
            get { return _data; }
            set
            {
                Set(ref _data, value);
            }
        }
    }

    public class EntrySelector : DataTemplateSelector
    {
        public DataTemplate SingleTemplate { get; set; }
        public DataTemplate MemberTemplate { get; set; }
        public DataTemplate PlaylistTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            IEntryData type  = (item as Entry).Data;

            return type switch
            {
                EntryDataSingle => SingleTemplate,
                EntryDataMember => MemberTemplate,
                EntryDataPlaylist => PlaylistTemplate,
                _ => base.SelectTemplate(item, container)
            };
        }
    }
}
