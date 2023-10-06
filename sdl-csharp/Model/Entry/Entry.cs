using sdl_csharp.Utility;
using System.Net.Http;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Common;

namespace sdl_csharp.Model.Entry
{
    public partial class Entry : NotifyPropertyChanged
    {
        private VideoType  type;
        private IEntryData data;
        private string url = null;
        private string videoId = null;
        private string playlistId = null;

        public VideoType Type { get => type; private set => Set(ref type, value); }

        public IEntryData Data { get => data; private set => Set(ref data, value); }

        public string URL { get => url; private set => Set(ref url, value); }

        public interface IEntryData {};

        public enum VideoType
        {
            SINGLE,
            MEMBER,
            PLAYLIST
        }

        public Entry(string url)
        {
            URL = url;

            Logger.Log($"Entry {url}");

            Type = GetType(url, ref videoId, ref playlistId) ?? Type;
            Data = CreateVideoData();
        }

        private IEntryData CreateVideoData()
        {
            return Type switch
            {
                VideoType.SINGLE => new EntryDataSingle(),
                VideoType.MEMBER => new EntryDataMember(),
                VideoType.PLAYLIST => new EntryDataPlaylist(),
                _ => null
            };
        }

        public async Task FetchAsync()
        {
            HttpClient httpClient = new();
            YoutubeClient client = new(httpClient);

            Logger.Log($"Fetching for type {Type}");

            if (videoId is not null)
            {
                if (Type is VideoType.SINGLE)
                {
                    await FetchSingleAsync(client);
                } else if (Type is VideoType.MEMBER && playlistId is not null)
                {
                    await FetchMemberAsync(client);
                }
            } else if (playlistId is not null && Type is VideoType.PLAYLIST)
            {
                await FetchPlaylistAsync(client);
            } else
            {
                Logger.Log($"URL appears to be invalid: {URL}");
            }
        }

        private async Task FetchSingleAsync(YoutubeClient client)
        {
            Logger.Log();
            var v = await client.Videos.GetAsync($"https://www.youtube.com/watch?v={videoId}");
            if (Data is EntryDataSingle data)
            {
                data.Title = v.Title;
                data.Thumbnail = v.Thumbnails[0].Url;
                data.Duration = v.Duration.ToString();
                data.Author = v.Author.ChannelTitle;
                Logger.Log(data.Title);
            }
        }

        private async Task FetchMemberAsync(YoutubeClient client)
        {
            Logger.Log();
            var v = await client.Videos.GetAsync($"https://www.youtube.com/watch?v={videoId}");
            var p = await client.Playlists.GetAsync($"https://youtube.com/playlist?list={playlistId}");
            var pAll = await client.Playlists.GetVideosAsync($"https://youtube.com/playlist?list={playlistId}");

            if (Data is EntryDataMember data)
            {
                data.Title = v.Title;
                data.Thumbnail = v.Thumbnails[0].Url;
                data.Duration = v.Duration.ToString();
                data.Author = v.Author.ChannelTitle;
                data.PlaylistTitle = p.Title;
                data.PlaylistThumbnail = p.Thumbnails[0].Url;
                data.PlaylistCount = (ushort) pAll.Count;
                Logger.Log(data.Title);
            }
        }

        private async Task FetchPlaylistAsync(YoutubeClient client)
        {
            Logger.Log();
            var p = await client.Playlists.GetAsync($"https://youtube.com/playlist?list={playlistId}");
            var pAll = await client.Playlists.GetVideosAsync($"https://youtube.com/playlist?list={playlistId}");

            if (Data is EntryDataPlaylist data)
            {
                data.Title = p.Title;
                data.Thumbnail = p.Thumbnails[0].Url;
                data.Author = p.Author?.ChannelTitle ?? "Anyád";
                data.Count = (ushort) pAll.Count;
                Logger.Log(data.Title);
            }
        }
    }
}
