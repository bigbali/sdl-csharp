using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows;

namespace sdl
{
    public partial class SDLWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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
            private string label = DownloadLabel.Default;
            private bool isDownloading = false;

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
                    return label;
                }
                set
                {
                    label = value;
                    OnPropertyChanged("Label");
                }
            }
            public bool IsDownloading {
                get
                {
                    return isDownloading;
                }
                set
                {
                    Label = value
                        ? DownloadLabel.Downloading
                        : DownloadLabel.Default;
                    isDownloading = value;
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

        private bool useSubFolderPath = false;
        private bool inferSubFolderPath = false;
        private bool isPlaylist = false;

        public bool UseSubFolderPath {
            get
            {
                return useSubFolderPath;
            }
            set
            {
                useSubFolderPath = value;
                OnPropertyChanged("UseSubFolderPath");
            }
        }

        public bool InferSubFolderPath
        {
            get
            {
                return inferSubFolderPath;
            }
            set
            {
                inferSubFolderPath = value;
                OnPropertyChanged("InferSubFolderPath");
            }
        }

        public bool IsPlaylist
        {
            get
            {
                return isPlaylist;
            }
            set
            {
                isPlaylist = value;
                OnPropertyChanged("IsPlaylist");
            }
        }

        //public static bool IsPlaylist { get; set; } = true;

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
