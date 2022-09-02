using sdl_csharp.Utility;

namespace sdl_csharp.Model
{
    /// <summary>
    /// Contains data about the entry, grouped based on type.
    /// </summary>
    public class EntryData: NotifyPropertyChanged
    {
        public static class VideoType
        {
            public const string SINGLE          = "SINGLE";
            public const string PLAYLIST_MEMBER = "PLAYLIST_MEMBER";
            public const string PLAYLIST        = "PLAYLIST";
        }

        public class SINGLE: NotifyPropertyChanged
        {
            private string title;
            private string thumbnail;
            private string duration;

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
            public string Duration
            {
                get => duration;
                set => Set(ref duration, value);
            }
        }

        public class PLAYLIST_MEMBER: NotifyPropertyChanged
        {
            private string memberTitle;
            private string memberThumbnail;
            private string memberDuration;
            private string playlistTitle;
            private string playlistThumbnail;
            private ushort playlistCount;

            public string MemberTitle
            {
                get => memberTitle;
                set => Set(ref memberTitle, value);
            }
            public string MemberThumbnail
            {
                get => memberThumbnail;
                set => Set(ref memberThumbnail, value);
            }
            public string MemberDuration
            {
                get => memberDuration;
                set => Set(ref memberDuration, value);
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

        private string type = null;

        

        /// <summary>
        /// One of: Single, Playlist Member, Playlist.
        /// </summary>
        public object Data
        {
            get
            {
                if (Type == VideoType.SINGLE)
                    return Single;
                if (Type == VideoType.PLAYLIST_MEMBER)
                    return PlaylistMember;
                if (Type == VideoType.PLAYLIST)
                    return Playlist;

                return null;
            }
        }

        public SINGLE Single = new();
        public PLAYLIST_MEMBER PlaylistMember = new();
        public PLAYLIST Playlist = new();
        public string Type
        {
            get => type;
            set {
                if (value is VideoType.SINGLE
                    or VideoType.PLAYLIST_MEMBER
                    or VideoType.PLAYLIST)
                {
                    Set(ref type, value);
                }
            }
        }
    }
}
