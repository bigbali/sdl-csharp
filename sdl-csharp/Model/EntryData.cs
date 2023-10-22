﻿using sdl_csharp.Utility;
using System;
using System.Collections.Generic;
using System.Windows;
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

        public float PlaylistMemberDownloadPercent { get; set; }

        public float PlaylistOverallDownloadPercent { get; set; }

        public void Initialize(Playlist playlistMetadata, IReadOnlyList<PlaylistVideo> playlistEntries);
    }

    public interface IEntrySingleData : IEntry
    {
        public string Author { get; set; }

        public string Title { get; set; }

        public string Thumbnail { get; set; }

        public float DownloadPercent { get; set; }

        public TimeSpan? Duration { get; set; }

        public void Initialize(Video videoMetadata);
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
    }

    public class EntryPlaylistData : NotifyPropertyChanged, IEntryPlaylistData
    {
        bool isLoaded;
        string playlistAuthor;
        string playlistTitle;
        string playlistThumbnail;
        uint playlistDownloadIndex;
        uint playlistMemberCount;
        float playlistMemberDownloadPercent;
        float playlistOverallDownloadPercent;

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

        public float PlaylistMemberDownloadPercent {
            get => playlistMemberDownloadPercent;
            set
            {
                Set(ref playlistMemberDownloadPercent, value);

                float members = Math.Max(PlaylistMemberCount, 1.0f);
                float index = Math.Max(PlaylistDownloadIndex, 1.0f);

                float maxPercentPerMember = 1.0f / members * 100.0f;
                float basePercent = maxPercentPerMember * index - maxPercentPerMember;

                float memberPercent = value / 100.0f * maxPercentPerMember;

                float totalPercent = basePercent + memberPercent;

                PlaylistOverallDownloadPercent = totalPercent;
            }
        }

        public float PlaylistOverallDownloadPercent {
            get => playlistOverallDownloadPercent;
            set => Set(ref playlistOverallDownloadPercent, value);
        }


        public void Initialize(Playlist playlistMetadata, IReadOnlyList<PlaylistVideo> playlistEntries)
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
        float downloadPercent;
        TimeSpan? duration;
        string playlistAuthor;
        string playlistTitle;
        string playlistThumbnail;
        uint playlistDownloadIndex;
        uint playlistMemberCount;
        float playlistMemberDownloadPercent;
        float playlistOverallDownloadPercent;

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

        public float PlaylistMemberDownloadPercent
        {
            get => playlistMemberDownloadPercent;
            set
            {
                Set(ref playlistMemberDownloadPercent, value);

                float members = Math.Max(PlaylistMemberCount, 1.0f);
                float index = Math.Max(PlaylistDownloadIndex, 1.0f);

                float maxPercentPerMember = 1.0f / members * 100.0f;
                float basePercent = maxPercentPerMember * index - maxPercentPerMember;

                float memberPercent = value / 100.0f * maxPercentPerMember;

                float totalPercent = basePercent + memberPercent;

                PlaylistOverallDownloadPercent = totalPercent;
            }
        }

        public float PlaylistOverallDownloadPercent
        {
            get => playlistOverallDownloadPercent;
            set => Set(ref playlistOverallDownloadPercent, value);
        }

        public float DownloadPercent { get => downloadPercent; set => Set(ref downloadPercent, value); }

        public void Initialize(Video videoMetadata)
        {
            Title = videoMetadata.Title;
            Thumbnail = videoMetadata.Thumbnails[0].Url;
            Duration = videoMetadata.Duration;
            Author = videoMetadata.Author?.ChannelTitle;
            isPartLoaded = true;

            if (isPartLoaded) IsLoaded = true;
        }

        public void Initialize(Playlist playlistMetadata, IReadOnlyList<PlaylistVideo> playlistEntries)
        {
            PlaylistTitle = playlistMetadata.Title;
            PlaylistThumbnail = playlistMetadata.Thumbnails[0].Url;
            PlaylistAuthor = playlistMetadata.Author?.ChannelTitle;
            PlaylistMemberCount = (uint)playlistEntries.Count;

            
            IsLoaded = isPartLoaded;
            isPartLoaded = true;
        }
    }
}
