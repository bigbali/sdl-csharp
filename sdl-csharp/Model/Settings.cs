using sdl_csharp.Utility;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Threading;

namespace sdl_csharp.Model
{
    public class Settings: NotifyPropertyChanged
    {
        private Settings()
        {
            Entries.CollectionChanged += EntriesChanged;

            ArgTemplate = new YTDLArgTemplate(this);
            ArgTemplateString = ArgTemplate.Template;

            ResetCommand = new RelayCommand(ResetTemplate);
            ArgTemplateTextChangedCommand = new RelayCommand(ArgTemplateTextChanged);
        }

        public RelayCommand ResetCommand { get; }
        public RelayCommand ArgTemplateTextChangedCommand { get; }


        private static Settings _instance;
        public static Settings Instance
        {
            get
            {
                _instance ??= new Settings();
                return _instance;
            }
        }

         bool _useSubFolderPath   = false;
         bool _inferSubFolderPath = false;
         bool _automaticNumbering = false;
         bool _removeEntries      = false;
         bool _isPlaylist         = true;
         bool _isAudio            = true;
         string _folderPath       = string.Empty;
         string _subFolderPath    = string.Empty;
         string argTemplateString;

        public void ArgTemplateTextChanged(object obj)
        {
            isChanged = ArgTemplateString != ArgTemplate.Template;
        }

        bool isChanged = false;
        public string ArgTemplateString { get => argTemplateString; set => Set(ref argTemplateString, value); }

        private void UpdateArgSet<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            Set(ref storage, value, propertyName);
            ArgTemplate = new YTDLArgTemplate(this);

            if (!isChanged)
                ArgTemplateString = ArgTemplate.Template;

            Logger.Log(isChanged.ToString());
        }

        public readonly SynchronizationContext UIContext  = SynchronizationContext.Current;
        public ObservableCollection<Entry.Entry> Entries { get; set; } = new();

        YTDLArgTemplate argTemplate;
        public YTDLArgTemplate ArgTemplate
        {
            get => argTemplate;
            set =>Set(ref argTemplate, value);
        }

        public void ResetTemplate(object obj)
        {
            Logger.Log($"SET to {ArgTemplate.Template}");
            ArgTemplateString = ArgTemplate.Template;
        }

        

        public bool UseSubFolderPath
        {
            get => _useSubFolderPath;
            set => UpdateArgSet(ref _useSubFolderPath, value);
        }
        public bool InferSubFolderPath
        {
            get => _inferSubFolderPath;
            set => UpdateArgSet(ref _inferSubFolderPath, value);
        }
        public bool IsPlaylist
        {
            get => _isPlaylist;
            set => UpdateArgSet(ref _isPlaylist, value);
        }
        public bool IsAudio
        {
            get => _isAudio;
            set => UpdateArgSet(ref _isAudio, value);
        }
        public bool AutomaticNumbering
        {
            get => _automaticNumbering;
            set => UpdateArgSet(ref _automaticNumbering, value);
        }
        public bool RemoveEntries
        {
            get => _removeEntries;
            set => Set(ref _removeEntries, value);
        }
        public string FolderPath {
            get => _folderPath;
            set => UpdateArgSet(ref _folderPath, value);
        }
        public string SubFolderPath { 
            get => _subFolderPath;
            set => UpdateArgSet(ref _subFolderPath, value);
        }
        private void EntriesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                Logger.Log("New entry added to collection");
                Entry.Entry entry = (Entry.Entry) e.NewItems[0];
                _ = entry.FetchAsync();
            }
        }
    }
}
