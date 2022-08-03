using System.Windows;
using System.Windows.Controls;

namespace sdl
{
    /// <summary>
    /// Interaction logic for SDLWindow.xaml
    /// </summary>
    public partial class SDLWindow : Window
    {
        public SDLWindow()
        {
            InitializeComponent();
            //URLGroupURLList.ItemsSource = URLs;
            //URLList.ItemsSource = URLEntries.ConvertAll(new System.Converter<URLEntry, DockPanel>((e) => e.visualEntry));
            //URLList.ItemsSource = URLEntries;
            DataContext = this;
            URLList.ItemsSource = URLEntries;

            //URLList.DisplayMemberPath = "visualEntry";
        }
    }
}
