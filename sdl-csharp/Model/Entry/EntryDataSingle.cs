using sdl_csharp.Utility;

namespace sdl_csharp.Model.Entry
{
    public class EntryDataSingle : NotifyPropertyChanged, Entry.IEntryData
    {
        string author;
        string title;
        string duration;
        string thumbnail;

        public string Author { get => author; set => Set(ref author, value); }
        public string Title { get => title; set => Set(ref title, value); }
        public string Duration { get => duration; set => Set(ref duration, value); }
        public string Thumbnail { get => thumbnail; set => Set(ref thumbnail, value); }
    }
}
