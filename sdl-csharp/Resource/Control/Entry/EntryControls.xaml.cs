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

        private void ForwardToInitIndividualDownload(object sender, RoutedEventArgs e)
        {
            Download.SDLWindowReference.InitIndividualDownload(sender, e);
        }

        private void ForwardToRemoveEntry(object sender, RoutedEventArgs e)
        {
            Download.SDLWindowReference.RemoveEntry(sender, e);
        }
    }
}
