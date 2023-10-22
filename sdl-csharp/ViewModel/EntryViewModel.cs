using sdl_csharp.Model;
using sdl_csharp.Model.Entry;
using sdl_csharp.Utility;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace sdl_csharp.ViewModel
{
    public class EntryViewModel : NotifyPropertyChanged
    {
        public Entry entry;
        public static ICommand AddEntryCommand { get; set; }
        public EntryType Type { get => entry.type; }
        public dynamic Data { get => entry.data; }
        public string URL { get => entry.url; }
        public EntryStatusViewModel StatusViewModel { get; set; }

        static EntryViewModel()
        {
            AddEntryCommand = new RelayCommand(AddEntry);
        }

        public EntryViewModel(string url)
        {
            entry = new Entry(url);
            StatusViewModel = new EntryStatusViewModel(entry);
            entry.PropertyChanged += SynchronizeViewModelPropertiesToModelProperties(entry, this);
        }

        public EntryViewModel(Entry entry)
        {
            this.entry = entry;
            StatusViewModel = new EntryStatusViewModel(entry);
        }

        public async Task Download() => await entry.Download();

        public void Remove() => entry.Remove();

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
                EntryViewModel urlEntry = new(newUrl);
                SettingsViewModel.Instance.EntryViewModels.Add(urlEntry);

                return;
            }

            MessageBox.Show("This URL appears to be invalid.\n" +
                            "A valid URL must link to a YouTube video or playlist.",
                            "URL is invalid",
                            MessageBoxButton.OK);
        }

        private void EntryPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            foreach (var modelProperty in sender.GetType().GetProperties())
            {
                var viewmodelProperty = GetType().GetProperty(modelProperty.Name);

                if (viewmodelProperty != null)
                {
                    // Get the value from the model
                    var modelValue = modelProperty.GetValue(sender);

                    // Initialize the value on the ViewModel
                    viewmodelProperty.SetValue(this, modelValue);
                }
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
            dynamic type = (item as EntryViewModel).Data;

            return type switch
            {
                EntrySingleData => SingleTemplate,
                EntryMemberData => MemberTemplate,
                EntryPlaylistData => PlaylistTemplate,
                _ => base.SelectTemplate(item, container)
            };
        }
    }
}




