using sdl_csharp.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos;

namespace sdl_csharp.Model
{
    public enum PlaylistMemberState
    {
        INITIALIZED,
        FETCHING,
        DOWNLOADING,
        CONVERTING,
        DONE,
        UNAVAILABLE,
        UNKNOWN
    }

    public class PlaylistMember: NotifyPropertyChanged
    {
        int totalBytes;
        int downloadedBytes;
        float downloadPercent;
        float conversionPercent;
        PlaylistMemberState state;
        PlaylistVideo video;

        // in bytes per second
        float? downloadSpeed;
        float? timeElapsed;
        int? timeLeft;

        public int TotalBytes { get => totalBytes; set => Set(ref totalBytes, value); }
        public int DownloadedBytes {
            get => downloadedBytes;
            set
            {
                Set(ref downloadedBytes, value);

                if (totalBytes > 0 && downloadedBytes > 0)
                {
                    DownloadPercent = (float)downloadedBytes / totalBytes * 100.0f;
                }
            }
        }
        public int? TimeLeft { get => timeLeft; set => Set(ref timeLeft, value); }
        public float? DownloadSpeed { get => downloadSpeed; set => Set(ref downloadSpeed, value); }
        public float? TimeElapsed { get => timeElapsed; set => Set(ref timeElapsed, value); }
        public float DownloadPercent { get => downloadPercent; set => Set(ref downloadPercent, value); }

        // for now we are unable to extract this
        public float ConversionPercent { get => conversionPercent; set => Set(ref conversionPercent, value); }
        public PlaylistMemberState State { get => state; set => Set(ref state, value); }
        public PlaylistVideo Video { get => video; set => Set(ref video, value); }

        public void Reset()
        {
            State = PlaylistMemberState.INITIALIZED;
            TotalBytes = 0;
            DownloadedBytes = 0;
            DownloadPercent = 0.0f;
            ConversionPercent = 0.0f;
            TimeElapsed = null;
            TimeLeft = null;
            DownloadSpeed = null;
        }
    }

    public interface IEntry
    {
        bool IsLoaded { get; set; }

        void Reset();
    }

    public interface IEntryPlaylistData : IEntry
    {
        string PlaylistAuthor { get; set; }
        string PlaylistTitle { get; set; }
        string PlaylistThumbnail { get; set; }
        int PlaylistDownloadIndex { get; set; }
        int PlaylistMemberCount { get; set; }

        // TODO eliminate this
        float PlaylistMemberDownloadPercent { get; set; }

        // TODO PlaylistDownloadPercent
        float PlaylistOverallDownloadPercent { get; set; }
        BindingList<PlaylistMember> PlaylistMembers { get; set; }
        PlaylistMember CurrentMember { get; set; }

        void Initialize(Playlist playlistMetadata, IReadOnlyList<PlaylistVideo> playlistEntries);
    }

    public interface IEntrySingleData : IEntry
    {
        string Author { get; set; }
        string Title { get; set; }
        string Thumbnail { get; set; }
        float DownloadPercent { get; set; }
        TimeSpan? Duration { get; set; }

        void Initialize(Video videoMetadata);
    }

    public class EntrySingleData : NotifyPropertyChanged, IEntrySingleData
    {
        bool isLoaded;
        string author;
        string title;
        string thumbnail;
        float downloadPercent;
        TimeSpan? duration;

        public bool IsLoaded { get => isLoaded; set => Set(ref isLoaded, value); }

        public string Author { get => author; set => Set(ref author, value); }

        public string Title { get => title; set => Set(ref title, value); }

        public string Thumbnail { get => thumbnail; set => Set(ref thumbnail, value); }

        public float DownloadPercent { get => downloadPercent; set => Set(ref downloadPercent, value); }

        public TimeSpan? Duration { get => duration; set => Set(ref duration, value); }

        public void Initialize(Video videoMetadata)
        {
            Title = videoMetadata.Title;
            Thumbnail = videoMetadata.Thumbnails[0].Url;
            Duration = videoMetadata.Duration;
            Author = videoMetadata.Author?.ChannelTitle;
            IsLoaded = true;
        }

        public void Reset() { downloadPercent = 0.0f; }
    }

    public class EntryPlaylistData : NotifyPropertyChanged, IEntryPlaylistData
    {
        bool isLoaded;
        string playlistAuthor;
        string playlistTitle;
        string playlistThumbnail;
        int playlistDownloadIndex;
        int playlistMemberCount;
        float playlistMemberDownloadPercent;
        float playlistOverallDownloadPercent;

        public bool IsLoaded
        {
            get => isLoaded;
            set
            {
                Set(ref isLoaded, value);

                // need to update UI index binding
                OnPropertyChanged(nameof(PlaylistDownloadIndex));
            }
        }

        public string PlaylistAuthor { get => playlistAuthor; set => Set(ref playlistAuthor, value); }

        public string PlaylistTitle { get => playlistTitle; set => Set(ref playlistTitle, value); }

        public string PlaylistThumbnail { get => playlistThumbnail; set => Set(ref playlistThumbnail, value); }

        public int PlaylistDownloadIndex
        {
            get => playlistDownloadIndex;
            set {
                Set(ref playlistDownloadIndex, value);

                int i = System.Math.Min(System.Math.Max(PlaylistDownloadIndex, 1) - 1, PlaylistMemberCount - 1);
                CurrentMember = PlaylistMembers[i];
            }
        }

        public int PlaylistMemberCount { get => playlistMemberCount; set => Set(ref playlistMemberCount, value); }

        public float PlaylistMemberDownloadPercent
        {
            get => playlistMemberDownloadPercent;
            set
            {
                Set(ref playlistMemberDownloadPercent, value);
                PlaylistOverallDownloadPercent = Utility.Math.Percent(PlaylistMemberCount, PlaylistDownloadIndex, value);
            }
        }

        public float PlaylistOverallDownloadPercent
        {
            get => playlistOverallDownloadPercent;
            set => Set(ref playlistOverallDownloadPercent, value);
        }

        public BindingList<PlaylistMember> PlaylistMembers { get; set; } = new();
        public PlaylistMember CurrentMember { get; set; }

        public void Initialize(Playlist playlistMetadata, IReadOnlyList<PlaylistVideo> playlistEntries)
        {
            PlaylistTitle = playlistMetadata.Title;
            PlaylistThumbnail = playlistMetadata.Thumbnails[0].Url;
            PlaylistMemberCount = playlistEntries.Count;

            // if there is no playlist author, assume that the playlist is generated by YouTube
            // in which case a playlist member likely has the same author
            PlaylistAuthor =
                playlistMetadata.Author?.ChannelTitle ?? playlistEntries[0].Author?.ChannelTitle;

            for (int i = 0; i < playlistEntries.Count; i++)
            {
                PlaylistMembers.Add(new PlaylistMember
                {
                    Video = playlistEntries[i]
                });
            }

            CurrentMember = PlaylistMembers[0];

            IsLoaded = true;
        }

        public void Reset()
        {
            PlaylistDownloadIndex = 0;
            PlaylistMemberDownloadPercent = 0.0f;
            PlaylistOverallDownloadPercent = 0.0f;

            foreach (PlaylistMember member in PlaylistMembers)
            {
                member.Reset();
            }

            CurrentMember = PlaylistMembers[0];
        }
    }

    public class EntryMemberData : NotifyPropertyChanged, IEntryPlaylistData, IEntrySingleData
    {
        bool isPartLoaded;
        bool isLoaded;
        string author;
        string title;
        string thumbnail;
        float downloadPercent;
        TimeSpan? duration;
        string playlistAuthor;
        string playlistTitle;
        string playlistThumbnail;
        int playlistDownloadIndex;
        int playlistMemberCount;
        float playlistMemberDownloadPercent;
        float playlistOverallDownloadPercent;

        public bool IsLoaded
        {
            get => isLoaded;
            set
            {
                Set(ref isLoaded, value);

                // need to update UI index binding
                OnPropertyChanged(nameof(PlaylistDownloadIndex));
            }
        }

        public string Author { get => author; set => Set(ref author, value); }

        public string Title { get => title; set => Set(ref title, value); }

        public string Thumbnail { get => thumbnail; set => Set(ref thumbnail, value); }

        public TimeSpan? Duration { get => duration; set => Set(ref duration, value); }

        public string PlaylistAuthor { get => playlistAuthor; set => Set(ref playlistAuthor, value); }

        public string PlaylistTitle { get => playlistTitle; set => Set(ref playlistTitle, value); }

        public string PlaylistThumbnail { get => playlistThumbnail; set => Set(ref playlistThumbnail, value); }

        public int PlaylistDownloadIndex
        {
            get => playlistDownloadIndex;
            set
            {
                Set(ref playlistDownloadIndex, value);

                int i = System.Math.Min(System.Math.Max(PlaylistDownloadIndex, 1) - 1, PlaylistMemberCount - 1);
                CurrentMember = PlaylistMembers[i];
            }
        }

        public int PlaylistMemberCount { get => playlistMemberCount; set => Set(ref playlistMemberCount, value); }

        public float PlaylistMemberDownloadPercent
        {
            get => playlistMemberDownloadPercent;
            set
            {
                Set(ref playlistMemberDownloadPercent, value);
                PlaylistOverallDownloadPercent = Utility.Math.Percent(PlaylistMemberCount, PlaylistDownloadIndex, value);
            }
        }

        public float PlaylistOverallDownloadPercent
        {
            get => playlistOverallDownloadPercent;
            set => Set(ref playlistOverallDownloadPercent, value);
        }

        public BindingList<PlaylistMember> PlaylistMembers { get; set; } = new();
        public PlaylistMember CurrentMember { get; set; }

        public float DownloadPercent { get => downloadPercent; set => Set(ref downloadPercent, value); }

        public void Initialize(Video videoMetadata)
        {
            Console.WriteLine("MEMBER INITIALIZE SINGLE");
            Title = videoMetadata.Title;
            Thumbnail = videoMetadata.Thumbnails[0].Url;
            Duration = videoMetadata.Duration;
            Author = videoMetadata.Author?.ChannelTitle;

            if (isPartLoaded)
                IsLoaded = true;

            isPartLoaded = true;
        }

        public void Initialize(Playlist playlistMetadata, IReadOnlyList<PlaylistVideo> playlistEntries)
        {
            Console.WriteLine("MEMBER INITIALIZE PLAYLIST");
            PlaylistTitle = playlistMetadata.Title;
            PlaylistThumbnail = playlistMetadata.Thumbnails[0].Url;
            PlaylistMemberCount = playlistEntries.Count;

            // if there is no playlist author, assume that the playlist is generated by YouTube
            // in which case a playlist member likely has the same author
            PlaylistAuthor =
                playlistMetadata.Author?.ChannelTitle ?? playlistEntries[0].Author?.ChannelTitle;

            for (int i = 0; i < playlistEntries.Count; i++)
            {
                PlaylistMembers.Add(new PlaylistMember
                {
                    Video = playlistEntries[i]
                });
            }

            CurrentMember = PlaylistMembers[0];

            if (isPartLoaded)
                IsLoaded = true;

            isPartLoaded = true;

        }

        public void Reset()
        {
            PlaylistDownloadIndex = 0;
            PlaylistMemberDownloadPercent = 0.0f;
            PlaylistOverallDownloadPercent = 0.0f;

            foreach (PlaylistMember member in PlaylistMembers)
            {
                member.Reset();
            }

            CurrentMember = PlaylistMembers[0];
        }
    }
}
