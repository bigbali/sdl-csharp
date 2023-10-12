using sdl_csharp.Utility;
using System.Windows;
using System.Windows.Controls;
using sdl_csharp.Model.Entry;
using sdl_csharp.Model;

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
            Settings.Instance.Entries.Remove(entry);
        }
    }
}
