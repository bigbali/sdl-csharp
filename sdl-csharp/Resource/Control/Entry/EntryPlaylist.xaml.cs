using sdl_csharp.Utility;
using sdl_csharp.ViewModel;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace sdl_csharp.Resource.Control.Entry
{
    /// <summary>
    /// Interaction logic for EntryPlaylist.xaml
    /// </summary>
    public partial class EntryPlaylist : UserControl
    {
        public EntryPlaylist()
        {
            DependencyPropertyChangedEventHandler setViewModelAsContext = (object sender, DependencyPropertyChangedEventArgs args) => {
               //DataContext ??= new EntryViewModel(args.NewValue as Model.Entry.Entry);
               if (DataContext is Model.Entry.Entry)
                {
                    DataContext = new EntryViewModel(DataContext as Model.Entry.Entry);
                }
            };

            DataContextChanged += setViewModelAsContext;
            InitializeComponent();

        }        
    }
}
