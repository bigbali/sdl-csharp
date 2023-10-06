using sdl_csharp.Utility;

namespace sdl_csharp.Model.Entry
{
    class EntryDataMember : NotifyPropertyChanged, Entry.IEntryData
    {
        string author;
        string title;
        string duration;
        string thumbnail;
        string playlistTitle;
        string playlistThumbnail;
        ushort playlistCount;

        public string Author { get => author; set => Set(ref author, value); }
        public string Title { get => title; set => Set(ref title, value); }
        public string Duration { get => duration; set => Set(ref duration, value); }
        public string Thumbnail { get => thumbnail; set => Set(ref thumbnail, value); }
        public string PlaylistTitle { get => playlistTitle; set => Set(ref playlistTitle, value); }
        public string PlaylistThumbnail { get => playlistThumbnail; set => Set(ref playlistThumbnail, value); }
        public ushort PlaylistCount { get => playlistCount; set => Set(ref playlistCount, value); }
    }
}
