using sdl_csharp.ViewModel;
using System.Windows.Controls;

namespace sdl_csharp.View
{
    /// <summary>
    /// Interaction logic for ActionsView.xaml
    /// </summary>
    public partial class ActionsView : UserControl
    {
        public ActionsView()
        {
            DataContext = SettingsViewModel.Instance;
            InitializeComponent();
        }
    }
}
