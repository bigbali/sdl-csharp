using sdl_csharp.Utility;

namespace sdl_csharp.Model.Entry
{
    class EntryDataPlaylist : NotifyPropertyChanged, Entry.IEntryData
    {
        string author = null;
        string title = null;
        string thumbnail = null;
        ushort count;

        public string Author { get => author; set => Set(ref author, value); }
        public string Title { get => title; set => Set(ref title, value); }
        public string Thumbnail { get => thumbnail; set => Set(ref thumbnail, value); }
        public ushort Count { get => count; set => Set(ref count, value); }
    }
}
