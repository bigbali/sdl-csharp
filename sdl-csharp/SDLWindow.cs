using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows;

namespace sdl
{
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

        public struct DownloadLabel // enum 2.0?
        {
            public static readonly string Default = "Download";
            public static readonly string Downloading = "Downloading...";
            public static readonly string Done = "Done";
        }

        public class URLEntry: INotifyPropertyChanged
        {
            private string _x = DownloadLabel.Default;
            private bool _y;

            public event PropertyChangedEventHandler PropertyChanged;

            void OnPropertyChanged(string propertyName)
            {
                if (propertyName == "Label")
                {
                    SpinnerAnimation(this);
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            public string Entry { get; set; }
            public string Label
            {
                get
                {
                    return _x;
                }
                set
                {
                    _x = value;
                    OnPropertyChanged("Label");
                }
            }
            public bool IsDownloading {
                get
                {
                    return _y;
                }
                set
                {
                    Label = value
                        ? DownloadLabel.Downloading
                        : DownloadLabel.Default;
                    _y = value;
                    OnPropertyChanged("IsDownloading");
                }
            }
            public URLEntry(string urlString)
            {
                Entry = urlString;
            }
        }
        public static ObservableCollection<URLEntry> URLEntries = new();
        public static string FolderPath { get; set; } = string.Empty;
        public static string SubFolderPath { get; set; } = string.Empty;
        public static bool IsPlaylist { get; set; } = true;

        public static readonly SynchronizationContext UIContext = SynchronizationContext.Current;
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
