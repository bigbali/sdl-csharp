using sdl_csharp.Model.Entry;
using sdl_csharp.Utility;
using System.ComponentModel;

namespace sdl_csharp.ViewModel
{
    public class EntryStatusViewModel : NotifyPropertyChanged
    {
        Entry entry;

        public EntryStatusViewModel(Entry entry)
        {
            this.entry = entry;
            entry.PropertyChanged += EntryPropertyChanged;
        }

        void EntryPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(entry.Status))
            {
                // Update the UI properties based on the changes in entry.Status
                OnPropertyChanged(nameof(IsInitialized));
                OnPropertyChanged(nameof(IsDownloading));
                OnPropertyChanged(nameof(IsConverting));
                OnPropertyChanged(nameof(IsDone));
                OnPropertyChanged(nameof(IsCancelled));
                OnPropertyChanged(nameof(IsError));
                OnPropertyChanged(nameof(IsInProgress));
                OnPropertyChanged(nameof(Label));
                OnPropertyChanged(nameof(Status));
            }
        }

        public bool IsInitialized => entry.Status is EntryStatus.INITIALIZED;
        public bool IsDownloading => entry.Status is EntryStatus.DOWNLOADING;
        public bool IsDone => entry.Status is EntryStatus.DONE;
        public bool IsCancelled => entry.Status is EntryStatus.CANCELLED;
        public bool IsConverting => entry.Status is EntryStatus.CONVERTING;
        public bool IsError => entry.Status is EntryStatus.ERROR;
        public bool IsInProgress => Status is (EntryStatus.DOWNLOADING or EntryStatus.CONVERTING);
        public EntryStatus Status => entry.Status;

        public string Label => entry.Status switch
        {
            EntryStatus.INITIALIZED => "Download",
            EntryStatus.CANCELLED => "Download",
            EntryStatus.DOWNLOADING => "Stop",
            EntryStatus.CONVERTING  => "Stop",
            EntryStatus.ERROR => "Error",
            EntryStatus.DONE => "Done",
            _ => "???"
        };      
    }
}
