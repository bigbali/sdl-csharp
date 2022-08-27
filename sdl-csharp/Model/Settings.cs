using sdl;
using sdl_csharp.Utility;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading;

namespace sdl_csharp.Resource.Model
{
    public class Settings: NotifyPropertyChanged
    {
        private bool _useSubFolderPath   = false;
        private bool _inferSubFolderPath = false;
        private bool _isPlaylist         = true;
        private bool _isAudio            = true;
        private bool _removeEntries      = true;
        private string _folderPath       = string.Empty;
        private string _subFolderPath    = string.Empty;
  
        public readonly SynchronizationContext UIContext  = SynchronizationContext.Current;
        public ObservableCollection<URLEntry> URLEntries { get; set; } = new();

        public bool UseSubFolderPath
        {
            get => _useSubFolderPath;
            set => Set(ref _useSubFolderPath, value);
        }

        public bool InferSubFolderPath
        {
            get => _inferSubFolderPath;
            set => Set(ref _inferSubFolderPath, value);
        }

        public bool IsPlaylist
        {
            get => _isPlaylist;
            set => Set(ref _isPlaylist, value);
        }

        public bool IsAudio
        {
            get => _isAudio;
            set => Set(ref _isAudio, value);
        }

        public bool RemoveEntries
        {
            get => _removeEntries;
            set => Set(ref _removeEntries, value);
        }

        public string FolderPath {
            get => _folderPath;
            set => Set(ref _folderPath, value);
        }
        public string SubFolderPath { 
            get => _subFolderPath;
            set => Set(ref _subFolderPath, value);
        }
        private void URLEntriesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                URLEntry newUrl = (URLEntry)e.NewItems[0];
                Download.FetchData(newUrl);
            }
        }

        public Settings()
        {
            URLEntries.CollectionChanged += URLEntriesChanged;
        }
    }
}
