using sdl_csharp.Utility;

namespace sdl_csharp.Model
{
    /// <summary>
    /// Contains data about the entry, such as title, thumbnail URL and possibly playlist information.
    /// </summary>
    public class EntryData: NotifyPropertyChanged
    {
        public class SINGLE: NotifyPropertyChanged
        {
            private string title;
            private string thumbnail;
            private ushort length;
            private bool   isPlaylistMember;
            private string playlistTitle;
            private string playlistThumbnail;
            private ushort playlistCount;

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
            public ushort Length
            {
                get => length;
                set => Set(ref length, value);
            }
            public bool IsPlaylistMember
            {
                get => isPlaylistMember;
                set => Set(ref isPlaylistMember, value);
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
            public ushort PlaylistCount
            {
                get => playlistCount;
                set => Set(ref playlistCount, value);
            }
        }

        public class PLAYLIST : NotifyPropertyChanged
        {
            private string title;
            private string thumbnail;
            private ushort count;

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
            public ushort Count
            {
                get => count;
                set => Set(ref count, value);
            }
        }

        private bool isPlaylist = false;

        public bool IsPlaylist
        {
            get => isPlaylist;
            set => Set(ref isPlaylist, value);
        }

        public SINGLE Single = new();
        public PLAYLIST Playlist = new();
    }
}
