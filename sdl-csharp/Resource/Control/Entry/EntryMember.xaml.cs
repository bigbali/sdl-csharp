using sdl_csharp.ViewModel;
using System;
using System.Linq;
using System.Windows.Controls;

namespace sdl_csharp.Resource.Control.Entry
{
    /// <summary>
    /// Interaction logic for EntryMember.xaml
    /// </summary>
    public partial class EntryMember : UserControl
    {
        public EntryMember()
        {
            InitializeComponent();
            //DataContext = new EntryViewModel(DataContext as Model.Entry.Entry);
        }
    }
}
