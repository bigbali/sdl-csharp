using Microsoft.VisualBasic.Logging;
using sdl_csharp.Utility;
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
            var entryvm = (EntryViewModel)button.DataContext;
            _ = entryvm.Download();
        }

        private void RemoveEntry(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            EntryViewModel entryvm = (EntryViewModel)button.DataContext;
            entryvm.Remove();
        }
    }
}
