using sdl_csharp.Utility;
using System;
using System.Net.Http;
using System.Windows;
using YoutubeExplode;
using YoutubeExplode.Common;

namespace sdl_csharp.Model.Entry
{
    public partial class Entry : NotifyPropertyChanged
    {
        public enum VideoType
        {
            SINGLE,
            MEMBER,
            PLAYLIST
        }

        public interface IVideoData { };

        private string VideoId    = null;
        private string PlaylistId = null;
        public VideoType Type  { get; private set; }
        public IVideoData Data { get; private set; }
        public string URL = null;

        public Entry(string url)
        {
            URL = url;
            Console.WriteLine($"Entry {url}");

            Type = Model.Entry.Data.GetType(url, ref VideoId, ref PlaylistId) ?? Type;
            IVideoData videoData = Type switch
            {
                VideoType.SINGLE   => new Single(),
                VideoType.MEMBER   => new Member(),
                VideoType.PLAYLIST => new Playlist(),
                _ => null
            };

            Fetch(url, videoData);

            Data = videoData;
        }

        public class Single : IVideoData
        {
            public string Author { get; set; }
            public string Title { get; set; }
            public string Duration { get; set; }
            public string Thumbnail { get; set; }
        }

        public class Member: IVideoData
        {
            public string Author { get; set; }
            public string Title { get; set; }
            public string Duration { get; set; }
            public string Thumbnail { get; set; }
            public string PlaylistTitle { get; set; }
            public string PlaylistThumbnail { get; set; }
            public ushort PlaylistCount { get; set; }
        }

        public class Playlist: IVideoData
        {
            public string Author { get; set; }
            public string Title { get; set; }
            public string Thumbnail { get; set; }
            public ushort Count { get; set; }
        }

        private async void Fetch(string url, IVideoData data)
        {
            HttpClient httpClient = new();
            YoutubeClient client = new(httpClient);

            Console.WriteLine("Fetching...");

            Single S = data as Single;
            Member M = data as Member;
            Playlist P = data as Playlist;


            // SINGLE
            if (VideoId is not null && Type is VideoType.SINGLE)
            {
                var v = await client.Videos.GetAsync($"https://www.youtube.com/watch?v={VideoId}");

                S.Title     = v.Title;
                S.Thumbnail = v.Thumbnails[0].Url;
                S.Duration  = v.Duration.ToString();
            }

            // MEMBER
            if (VideoId is not null && PlaylistId is not null && Type is VideoType.MEMBER)
            {
                var v = await client.Videos.GetAsync($"https://www.youtube.com/watch?v={VideoId}");
                var p = await client.Playlists.GetAsync($"https://youtube.com/playlist?list={PlaylistId}");
                var pAll = await client.Playlists.GetVideosAsync($"https://youtube.com/playlist?list={PlaylistId}");

                M.Title = v.Title;
                M.Thumbnail = v.Thumbnails[0].Url;
                M.Duration = v.Duration.ToString();
                M.Author = v.Author.ChannelTitle;
                M.PlaylistTitle = p.Title;
                M.PlaylistThumbnail = p.Thumbnails[0].Url;
                M.PlaylistCount = (ushort) pAll.Count;
            }

            // PLAYLIST
            if (PlaylistId is not null && Type is VideoType.PLAYLIST)
            {
                var p = await client.Playlists.GetAsync($"https://youtube.com/playlist?list={PlaylistId}");
                var pAll = await client.Playlists.GetVideosAsync($"https://youtube.com/playlist?list={PlaylistId}");

                P.Title = p.Title;
                P.Thumbnail = p.Thumbnails[0].Url;
                P.Count = (ushort) pAll.Count;
            }

            if (VideoId is null && PlaylistId is null) // Presume that URL is invalid
            {
                MessageBox.Show($"{url} appears to be invalid.",
                                "URL is not valid",
                                MessageBoxButton.OK);
            }
        }
    }
}
