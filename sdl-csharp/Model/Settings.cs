using sdl_csharp.Model.Entry;
using sdl_csharp.Utility;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading;

namespace sdl_csharp.Resource.Model
{
    public class Settings: NotifyPropertyChanged
    {
        private static Settings _instance;
        private Settings()
        {
            Entries.CollectionChanged += EntriesChanged;
        }

        public static Settings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Settings();
                }
                return _instance;
            }
        }

        private bool _useSubFolderPath   = false;
        private bool _inferSubFolderPath = false;
        private bool _automaticNumbering = false;
        private bool _removeEntries      = false;
        private bool _isPlaylist         = true;
        private bool _isAudio            = true;
        private string _folderPath       = string.Empty;
        private string _subFolderPath    = string.Empty;

        public readonly SynchronizationContext UIContext  = SynchronizationContext.Current;
        public ObservableCollection<Entry> Entries { get; set; } = new();

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
        public bool AutomaticNumbering
        {
            get => _automaticNumbering;
            set => Set(ref _automaticNumbering, value);
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
        private void EntriesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                Utility.Logger.Log("New entry added to collection");
                Entry entry = (Entry) e.NewItems[0];
                _ = entry.FetchAsync();
            }
        }
    }
}
