using sdl_csharp.Model;
using sdl_csharp.Utility;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Input;

namespace sdl_csharp.ViewModel
{
    public class SettingsViewModel : NotifyPropertyChanged
    {
        static SettingsViewModel instance;
        readonly Settings settings;
        bool isAppliedTemplateChanged;

        public static SettingsViewModel Instance
        {
            get
            {
                instance ??= new SettingsViewModel();
                return instance;
            }
        }

        SettingsViewModel()
        {
            settings = Settings.Instance;
            EntryViewModels = new ObservableCollection<EntryViewModel>(settings.entries.Select(entry => new EntryViewModel(entry)));
            EntryViewModels.CollectionChanged += SyncModelToViewModel;

            ResetCommand = new RelayCommand(ResetTemplate);
            SelectFolderCommand = new RelayCommand(SelectFolder);
            ArgTemplateTextChangedCommand = new RelayCommand(ArgTemplateTextChanged);
        }

        public ICommand ResetCommand { get; }
        public ICommand SelectFolderCommand { get; }
        public ICommand ArgTemplateTextChangedCommand { get; }

        public YTDLArgTemplate ArgTemplate
        {
            get => settings.argTemplate;
            set => Set(ref settings.argTemplate, value);
        }

        public ObservableCollection<EntryViewModel> EntryViewModels { get; set; } = new();
        public string ArgTemplateString { get => settings.argTemplateString; set => Set(ref settings.argTemplateString, value); }

        public bool UseSubFolderPath
        {
            get => settings.useSubFolderPath;
            set => UpdateArgSet(ref settings.useSubFolderPath, value);
        }
        public bool InferSubFolderPath
        {
            get => settings.inferSubFolderPath;
            set => UpdateArgSet(ref settings.inferSubFolderPath, value);
        }
        public bool IsPlaylist
        {
            get => settings.isPlaylist;
            set => UpdateArgSet(ref settings.isPlaylist, value);
        }
        public bool IsAudio
        {
            get => settings.isAudio;
            set => UpdateArgSet(ref settings.isAudio, value);
        }
        public bool AutomaticNumbering
        {
            get => settings.automaticNumbering;
            set => UpdateArgSet(ref settings.automaticNumbering, value);
        }
        public bool RemoveEntries
        {
            get => settings.removeEntries;
            set => Set(ref settings.removeEntries, value);
        }
        public string FolderPath
        {
            get => settings.folderPath;
            set => UpdateArgSet(ref settings.folderPath, value);
        }
        public string SubFolderPath
        {
            get => settings.subFolderPath;
            set => UpdateArgSet(ref settings.subFolderPath, value);
        }

        private void UpdateArgSet<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            Set(ref storage, value, propertyName);
            ArgTemplate = new YTDLArgTemplate(settings);

            if (!isAppliedTemplateChanged)
                ArgTemplateString = ArgTemplate.Template;
        }

        private void ArgTemplateTextChanged(object obj)
        {
            isAppliedTemplateChanged = ArgTemplateString != ArgTemplate.Template;
        }

        private void ResetTemplate(object obj)
        {
            ArgTemplateString = ArgTemplate.Template;
        }

        private void SyncModelToViewModel(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var newItem in e.NewItems.Cast<EntryViewModel>())
                {
                    settings.entries.Add(newItem.entry);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var oldItem in e.OldItems.Cast<EntryViewModel>())
                {
                    EntryViewModel modelToRemove = EntryViewModels.FirstOrDefault(viewmodel => viewmodel.entry == oldItem.entry);
                    if (modelToRemove != null)
                    {
                        settings.entries.Remove(modelToRemove.entry);
                    }
                }
            }
        }

        private void SelectFolder(object obj)
        {
            FolderBrowserDialog dialog = new();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrEmpty(dialog.SelectedPath))
                    return;

                FolderPath = dialog.SelectedPath;
            }
        }
    }
}
