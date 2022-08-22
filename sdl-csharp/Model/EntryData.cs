using sdl_csharp.Utility;

namespace sdl_csharp.Model
{
    /// <summary>
    /// Contains data about the entry, such as title, thumbnail URL and possibly playlist information.
    /// </summary>
    public class EntryData: NotifyPropertyChanged
    {
        private string title;
        private string thumbnail;
        private string length;
        private string playlistTitle;
        private string playlistThumbnail;
        private short  playlistLength;
        private bool   isPlaylist;

        public string Title
        {
            get => title;
            set => Set(ref title, value);
        }

        public string Thumbnail
        {
            get => thumbnail;
            set => Set(ref thumbnail, value);
        }
        public string Length
        {
            get => length;
            set => Set(ref length, value);
        }
        public string PlaylistTitle
        {
            get => playlistTitle;
            set => Set(ref playlistTitle, value);
        }
        public string PlaylistThumbnail
        {
            get => playlistThumbnail;
            set => Set(ref playlistThumbnail, value);
        }

        public short PlaylistLength
        {
            get => playlistLength;
            set => Set(ref playlistLength, value);
        }

        public bool IsPlaylist
        {
            get => isPlaylist;
            set => Set(ref isPlaylist, value);
        }
    }
}
