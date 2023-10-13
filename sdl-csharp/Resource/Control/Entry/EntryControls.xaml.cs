using sdl_csharp.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace sdl_csharp.Resource.Control.Entry
{
    /// <summary>
    /// Interaction logic for EntryControls.xaml
    /// </summary>
    public partial class EntryControls : UserControl
    {
        public EntryControls()
        {
            InitializeComponent();
        }

        private void DownloadEntry(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            var entry = (Model.Entry.Entry)button.DataContext;
            _ = entry.Download();
        }

        private void RemoveEntry(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            var entry = (Model.Entry.Entry)button.DataContext;
            SettingsViewModel.Instance.Entries.Remove(entry);
        }
    }
}
