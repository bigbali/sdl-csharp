using sdl_csharp.Utility;
using System;
using System.Windows;

namespace sdl_csharp.Model
{
    /// <summary>
    /// Contains data about the entry, grouped based on type.
    /// </summary>
    public class EntryData: NotifyPropertyChanged
    {
        private static string TrimZero(string duration)
        {
            return duration is not null && duration.StartsWith("00")
            ? duration.Remove(0, 3) // Trim if less than an hour long
            : duration;
        }
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
                get => TrimZero(duration);
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
                get => TrimZero(memberDuration);
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

        public void DEBUG_PRINT_SINGLE()
        {
            if (Type is VideoType.SINGLE)
            {
                string S = "DEBUG_SINGLE_";
                Console.WriteLine(S + "TITLE: " + Single.Title);
                Console.WriteLine(S + "THUMBNAIL: " + Single.Thumbnail);
                Console.WriteLine(S + "DURATION: " + Single.Duration);
            }
        }

        public void DEBUG_PRINT_MEMBER()
        {
            if (Type is VideoType.PLAYLIST_MEMBER)
            {
                string S = "DEBUG_MEMBER_SINGLE_";
                string P = "DEBUG_MEMBER_PLAYLIST_";

                Console.WriteLine(S + "TITLE: " + PlaylistMember.MemberTitle);
                Console.WriteLine(S + "THUMBNAIL: " + PlaylistMember.MemberThumbnail);
                Console.WriteLine(S + "DURATION: " + PlaylistMember.MemberDuration);

                Console.WriteLine(P + "TITLE: " + PlaylistMember.PlaylistTitle);
                Console.WriteLine(P + "THUMBNAIL: " + PlaylistMember.PlaylistThumbnail);
                Console.WriteLine(P + "COUNT: " + PlaylistMember.PlaylistCount);
            }
        }

        public void DEBUG_PRINT_PLAYLIST()
        {
            if (Type is VideoType.PLAYLIST)
            {
                string P = "DEBUG_PLAYLIST_";
                Console.WriteLine(P + "TITLE: " + Playlist.Title);
                Console.WriteLine(P + "THUMBNAIL: " + Playlist.Thumbnail);
                Console.WriteLine(P + "COUNT: " + Playlist.Count);
            }
        }
    }
}
