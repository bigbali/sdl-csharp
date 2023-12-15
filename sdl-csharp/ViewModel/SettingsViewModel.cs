using sdl_csharp.Model;
using sdl_csharp.Utility;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
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
            EntryViewModels = new ObservableCollection<EntryViewModel>(
                settings.entries.Select(entry => new EntryViewModel(entry)));
            EntryViewModels.CollectionChanged += SyncModelEntriesToViewModelEntries;

            ResetCommand = new RelayCommand(ResetTemplate);
            SelectFolderCommand = new RelayCommand(SelectFolder);
            ArgTemplateTextChangedCommand = new RelayCommand(ArgTemplateTextChanged);

            DownloadSelectedCommand = new RelayCommand(DownloadSelected);
            RemoveSelectedCommand = new RelayCommand(RemoveSelected);

            SelectedEntriesSelectAllCommand = new RelayCommand(SelectedEntriesSelectAll);
            SelectedEntriesDeselectAllCommand = new RelayCommand(SelectedEntriesDeselectAll);
        }

        public ICommand ResetCommand { get; }

        public ICommand SelectFolderCommand { get; }

        public ICommand ArgTemplateTextChangedCommand { get; }

        public ICommand DownloadSelectedCommand { get; }
        public ICommand RemoveSelectedCommand { get; }

        public static ICommand SelectedEntriesSelectAllCommand { get; set; }
        public static ICommand SelectedEntriesDeselectAllCommand { get; set; }

        public ArgTemplate ArgTemplate { get => settings.argTemplate; set => Set(ref settings.argTemplate, value); }

        public ObservableCollection<EntryViewModel> EntryViewModels { get; set; } = new();

        public string ArgTemplateString
        {
            get => settings.argTemplateString;
            set => Set(ref settings.argTemplateString, value);
        }

        public bool UseSubFolder
        {
            get => settings.useSubFolder;
            set => UpdateArgSet(ref settings.useSubFolder, value);
        }

        public bool InferFolderByAuthor
        {
            get => settings.inferFolderByAuthor;
            set => UpdateArgSet(ref settings.inferFolderByAuthor, value);
        }

        public bool InferFolderByPlaylistTitle
        {
            get => settings.inferFolderByPlaylistTitle;
            set => UpdateArgSet(ref settings.inferFolderByPlaylistTitle, value);
        }


        public bool IsPlaylist { get => settings.isPlaylist; set => UpdateArgSet(ref settings.isPlaylist, value); }

        public bool IsAudio { get => settings.isAudio; set => UpdateArgSet(ref settings.isAudio, value); }

        public bool AutomaticNumbering
        {
            get => settings.automaticNumbering;
            set => UpdateArgSet(ref settings.automaticNumbering, value);
        }

        public bool RemoveEntries { get => settings.removeEntries; set => Set(ref settings.removeEntries, value); }

        public string FolderPath { get => settings.folderPath; set => UpdateArgSet(ref settings.folderPath, value); }

        public string SubFolderPath
        {
            get => settings.subFolderPath;
            set => UpdateArgSet(ref settings.subFolderPath, value);
        }

        private void UpdateArgSet<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            Set(ref storage, value, propertyName);
            ArgTemplate = new ArgTemplate(settings);

            if (!isAppliedTemplateChanged)
                ArgTemplateString = ArgTemplate.Template;
        }

        private void ArgTemplateTextChanged(object obj)
        { isAppliedTemplateChanged = ArgTemplateString != ArgTemplate.Template; }

        private void ResetTemplate(object obj) { ArgTemplateString = ArgTemplate.Template; }

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

        private void DownloadSelected(object obj)
        {
            if (EntryViewModels.Count == 0)
            {
                // todo in this case disable button
                System.Windows.MessageBox
                    .Show("Please specify some URLs to be downloaded!", "No entries to download", MessageBoxButton.OK);
                return;
            }

            List<EntryViewModel> downloading = new();
            List<EntryViewModel> done = new();

            foreach (EntryViewModel vm in EntryViewModels)
            {
                if (vm.StatusViewModel.IsDownloading)
                {
                    downloading.Add(vm);
                    continue;
                }

                if (vm.StatusViewModel.IsDone)
                {
                    done.Add(vm);
                    continue;
                }

                if (vm.IsSelected)
                {
                    _ = vm.entry.Download();
                }
            }

            if (downloading.Count > 0)
            {
                System.Windows.MessageBox
                    .Show(
                        """
                        One or more entries have not yet finished downloading.
                        Please wait.
                        """,
                        "Entries already downloading",
                        MessageBoxButton.OK);

                downloading.Clear();
            }

            if (done.Count > 0)
            {
                if (System.Windows.MessageBox
                        .Show(
                            """
                            Some entries have already been downloaded.
                            Would you like to download them again?.
                            """,
                            "Entries already downloaded",
                            MessageBoxButton.YesNo) ==
                    MessageBoxResult.Yes)
                {
                    foreach (EntryViewModel vm in done)
                    {
                        if (vm.IsSelected)
                        {
                            _ = vm.entry.Download();
                        }
                    };
                }
            }
        }

        private void RemoveSelected(object obj)
        {
            List<EntryViewModel> downloading = new();
            List<EntryViewModel> entriesToRemove = new();

            foreach (EntryViewModel vm in EntryViewModels)
            {
                if (vm.StatusViewModel.IsDownloading)
                {
                    downloading.Add(vm);
                    continue;
                }

                if (vm.IsSelected)
                {
                    entriesToRemove.Add(vm);
                }
            }

            if (downloading.Count > 0)
            {
                if (System.Windows.MessageBox
                    .Show(
                        """
                        One or more entries have not yet finished downloading.
                        Remove anyway?.
                        """,
                        "Entries downloading",
                        MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    foreach (EntryViewModel vm in EntryViewModels)
                    {
                        if (vm.IsSelected)
                        {
                            entriesToRemove.Add(vm);
                        }
                    }
                }

            }

            Logger.Log($"Remove {entriesToRemove.Count} entries");
            entriesToRemove.ForEach(vm => { vm.Remove(); });
            downloading.Clear();
        }

        public static void SelectedEntriesSelectAll(object obj)
        {
            Logger.Log("Select all command");
            if (instance is not null)
            {
                foreach (EntryViewModel entryvm in instance.EntryViewModels)
                {
                    entryvm.IsSelected = true;
                    Logger.Log("Selected EntryViewModel");
                }
            }
        }

        public static void SelectedEntriesDeselectAll(object obj)
        {
            if (instance is not null)
            {
                foreach (EntryViewModel entryvm in instance.EntryViewModels)
                {
                    entryvm.IsSelected = false;
                }
            }
        }

        private void SyncModelEntriesToViewModelEntries(object sender, NotifyCollectionChangedEventArgs e)
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
                    EntryViewModel modelToRemove = EntryViewModels.FirstOrDefault(
                        viewmodel => viewmodel.entry == oldItem.entry);
                    if (modelToRemove != null)
                    {
                        settings.entries.Remove(modelToRemove.entry);
                    }
                }
            }
        }
    }
}
