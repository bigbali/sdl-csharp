using sdl_csharp.Model;
using sdl_csharp.Model.Entry;
using sdl_csharp.Utility;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
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

            ResetCommand = new RelayCommand(ResetTemplate);
            ArgTemplateTextChangedCommand = new RelayCommand(ArgTemplateTextChanged);
        }

        public ICommand ResetCommand { get; }
        public ICommand ArgTemplateTextChangedCommand { get; }

        public YTDLArgTemplate ArgTemplate
        {
            get => settings.argTemplate;
            set => Set(ref settings.argTemplate, value);
        }

        public ObservableCollection<Entry> Entries { get; set; } = new();
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
    }
}
