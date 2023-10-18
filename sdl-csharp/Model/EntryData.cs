using sdl_csharp.Utility;
using System;
using System.Collections.Generic;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos;

namespace sdl_csharp.Model
{
    public interface IEntry
    {
        public bool IsLoaded { get; set; }
    }

    public interface IEntryPlaylistData : IEntry
    {
        public string PlaylistAuthor { get; set; }

        public string PlaylistTitle { get; set; }

        public string PlaylistThumbnail { get; set; }

        public uint PlaylistDownloadIndex { get; set; }

        public uint PlaylistMemberCount { get; set; }

        public void Set(Playlist playlistMetadata, IReadOnlyList<PlaylistVideo> playlistEntries);
    }

    public interface IEntrySingleData : IEntry
    {
        public string Author { get; set; }

        public string Title { get; set; }

        public string Thumbnail { get; set; }

        public TimeSpan? Duration { get; set; }

        public void Set(Video videoMetadata);
    }

    public class EntrySingleData : NotifyPropertyChanged, IEntrySingleData
    {
        bool isLoaded;
        string author;
        string title;
        string thumbnail;
        TimeSpan? duration;

        public bool IsLoaded { get => isLoaded; set => Set(ref isLoaded, value); }

        public string Author { get => author; set => Set(ref author, value); }

        public string Title { get => title; set => Set(ref title, value); }

        public string Thumbnail { get => thumbnail; set => Set(ref thumbnail, value); }

        public TimeSpan? Duration { get => duration; set => Set(ref duration, value); }

        public void Set(Video videoMetadata)
        {
            Title = videoMetadata.Title;
            Thumbnail = videoMetadata.Thumbnails[0].Url;
            Duration = videoMetadata.Duration;
            Author = videoMetadata.Author?.ChannelTitle;
            IsLoaded = true;
        }
    }

    public class EntryPlaylistData : NotifyPropertyChanged, IEntryPlaylistData
    {
        bool isLoaded;
        string playlistAuthor;
        string playlistTitle;
        string playlistThumbnail;
        uint playlistDownloadIndex;
        uint playlistMemberCount;

        public bool IsLoaded { get => isLoaded; set => Set(ref isLoaded, value); }

        public string PlaylistAuthor { get => playlistAuthor; set => Set(ref playlistAuthor, value); }

        public string PlaylistTitle { get => playlistTitle; set => Set(ref playlistTitle, value); }

        public string PlaylistThumbnail { get => playlistThumbnail; set => Set(ref playlistThumbnail, value); }

        public uint PlaylistDownloadIndex
        {
            get => playlistDownloadIndex;
            set => Set(ref playlistDownloadIndex, value);
        }

        public uint PlaylistMemberCount { get => playlistMemberCount; set => Set(ref playlistMemberCount, value); }

        public void Set(Playlist playlistMetadata, IReadOnlyList<PlaylistVideo> playlistEntries)
        {
            PlaylistTitle = playlistMetadata.Title;
            PlaylistThumbnail = playlistMetadata.Thumbnails[0].Url;
            PlaylistAuthor = playlistMetadata.Author?.ChannelTitle;
            PlaylistMemberCount = (uint)playlistEntries.Count;
            IsLoaded = true;
        }
    }

    public class EntryMemberData : NotifyPropertyChanged, IEntryPlaylistData, IEntrySingleData
    {
        bool isPartLoaded;
        bool isLoaded;
        string author;
        string title;
        string thumbnail;
        TimeSpan? duration;
        string playlistAuthor;
        string playlistTitle;
        string playlistThumbnail;
        uint playlistDownloadIndex;
        uint playlistMemberCount;

        public bool IsLoaded { get => isLoaded; set => Set(ref isLoaded, value); }

        public string Author { get => author; set => Set(ref author, value); }

        public string Title { get => title; set => Set(ref title, value); }

        public string Thumbnail { get => thumbnail; set => Set(ref thumbnail, value); }

        public TimeSpan? Duration { get => duration; set => Set(ref duration, value); }

        public string PlaylistAuthor { get => playlistAuthor; set => Set(ref playlistAuthor, value); }

        public string PlaylistTitle { get => playlistTitle; set => Set(ref playlistTitle, value); }

        public string PlaylistThumbnail { get => playlistThumbnail; set => Set(ref playlistThumbnail, value); }

        public uint PlaylistDownloadIndex
        {
            get => playlistDownloadIndex;
            set => Set(ref playlistDownloadIndex, value);
        }

        public uint PlaylistMemberCount { get => playlistMemberCount; set => Set(ref playlistMemberCount, value); }

        public void Set(Video videoMetadata)
        {
            Title = videoMetadata.Title;
            Thumbnail = videoMetadata.Thumbnails[0].Url;
            Duration = videoMetadata.Duration;
            Author = videoMetadata.Author?.ChannelTitle;
            isPartLoaded = true;

            if (isPartLoaded) IsLoaded = true;
        }

        public void Set(Playlist playlistMetadata, IReadOnlyList<PlaylistVideo> playlistEntries)
        {
            PlaylistTitle = playlistMetadata.Title;
            PlaylistThumbnail = playlistMetadata.Thumbnails[0].Url;
            PlaylistAuthor = playlistMetadata.Author?.ChannelTitle;
            PlaylistMemberCount = (uint)playlistEntries.Count;
            isPartLoaded = true;

            if (isPartLoaded) IsLoaded = true;
        }
    }
}
