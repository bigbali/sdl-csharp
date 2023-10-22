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
                OnPropertyChanged(nameof(IsError));
                OnPropertyChanged(nameof(IsDone));
                OnPropertyChanged(nameof(IsDownloading));
                OnPropertyChanged(nameof(IsConverting));
                OnPropertyChanged(nameof(Label));
                OnPropertyChanged(nameof(Status));
            }
        }

        public bool IsError => entry.Status is EntryStatus.ERROR;
        public bool IsDone => entry.Status is EntryStatus.DONE;
        public bool IsDownloading => entry.Status is EntryStatus.DOWNLOADING;
        public bool IsConverting => entry.Status is EntryStatus.CONVERTING;
        public EntryStatus Status => entry.Status;

        public string Label => entry.Status switch
        {
            EntryStatus.INITIALIZED => "Download",
            EntryStatus.DOWNLOADING => "Downloading",
            EntryStatus.CONVERTING  => "Converting",
            EntryStatus.ERROR => "Error",
            EntryStatus.DONE => "Done",
            _ => "Unknown (wtf?)"
        };      
    }
}
