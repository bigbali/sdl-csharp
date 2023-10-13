using sdl_csharp.Model.Entry;
using sdl_csharp.Utility;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static sdl_csharp.Model.Entry.Entry;

namespace sdl_csharp.ViewModel
{
    public class EntryViewModel : NotifyPropertyChanged
    {
        private object data;
        public static ICommand AddEntryCommand { get; set; }

        public object Data
        {
            get => data;
            set => Set(ref data, value);
        }

        static EntryViewModel()
        {
            AddEntryCommand = new RelayCommand(AddEntry);
        }

        public static void AddEntry(object arg)
        {
            var urlInput = (TextBox) arg;
            string newUrl = urlInput.Text;

            Logger.Log(newUrl);
            urlInput.Clear();

            if (newUrl.Contains("list=RDMM") || newUrl.Contains("list=RDGM"))
            {
                MessageBox.Show("This playlist cannot be downloaded because it's personalised (is bound to your YouTube profile).",
                                "EntryDataPlaylist is not valid",
                                MessageBoxButton.OK);
                return;
            }

            if (newUrl != string.Empty
                && (newUrl.StartsWith("https://youtu.be") || newUrl.StartsWith("https://www.youtube.com")))
            {
                Entry urlEntry = new(newUrl);
                SettingsViewModel.Instance.Entries.Add(urlEntry);

                return;
            }

            MessageBox.Show("This URL appears to be invalid.\n" +
                            "A valid URL must link to a YouTube video or playlist.",
                            "URL is invalid",
                            MessageBoxButton.OK);
        }
    }

    public class EntrySelector : DataTemplateSelector
    {
        public DataTemplate SingleTemplate { get; set; }
        public DataTemplate MemberTemplate { get; set; }
        public DataTemplate PlaylistTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            IEntryData type = (item as Entry).Data;

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
