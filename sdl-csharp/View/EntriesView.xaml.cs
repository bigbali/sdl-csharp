using sdl_csharp.ViewModel;
using System.Windows.Controls;

namespace sdl_csharp.View
{
    /// <summary>
    /// Interaction logic for EntriesView.xaml
    /// </summary>
    public partial class EntriesView : UserControl
    {
        public EntriesView()
        {
            InitializeComponent();
            DataContext = SettingsViewModel.Instance.Entries;
        }
    }
}
