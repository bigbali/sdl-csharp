using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;

namespace sdl
{
    /// <summary>
    /// Interaction logic for SDLWindow.xaml
    /// </summary>
    public partial class SDLWindow : Window
    {
        public struct InputPlaceholders
        {
            public string URLInput { set; get; }
            public string FolderPathInput { set; get; }

            public InputPlaceholders(string url, string folder)
            {
                URLInput = url;
                FolderPathInput = folder;
            }
        }

        public struct URLEntry
        {
            public string Entry { get; set; }
            public URLEntry(string urlString)
            {
                Entry = urlString;
            }
        };

        public static SynchronizationContext UIContext = SynchronizationContext.Current;
        public static ObservableCollection<URLEntry> URLEntries = new();
        public static string FolderPath { get; set; }
        public static bool IsPlaylist { get; set; } = true;

        public static readonly InputPlaceholders Placeholders = new(
           "Select URL",
           "Select folder"
       );

        public SDLWindow()
        {
            InitializeComponent();

            DataContext = this;
            URLList.ItemsSource = URLEntries;
        }
    }
}
