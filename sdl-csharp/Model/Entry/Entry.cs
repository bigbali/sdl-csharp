using sdl_csharp.Utility;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Common;

namespace sdl_csharp.Model.Entry
{
    public partial class Entry : NotifyPropertyChanged
    {
        VideoType type;
        IEntryData data;
        string url = null;
        string videoId = null;
        string playlistId = null;

        public VideoType Type { get => type; private set => Set(ref type, value); }

        public IEntryData Data { get => data; private set => Set(ref data, value); }

        public string URL { get => url; private set => Set(ref url, value); }

        public interface IEntryData { };

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
            Data = Type switch
            {
                VideoType.SINGLE => new EntryDataSingle(),
                VideoType.MEMBER => new EntryDataMember(),
                VideoType.PLAYLIST => new EntryDataPlaylist(),
                _ => null
            };

            _ = FetchAsync();
        }

        public async Task Download()
        {
            var settings = Settings.Instance;
            var downloadStartedAt = DateTime.Now;

            string folderPath = settings.folderPath != string.Empty
                ? settings.folderPath // If there is no folder path set, use desktop instead
                : Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\SDL Downloads";

            if (settings.useSubFolderPath)
            {
                folderPath += $"/{settings.subFolderPath}";
            }

            if (settings.inferSubFolderPath) // If inferring is enabled, infer from playlist title
            {
                if (Type is VideoType.PLAYLIST)
                {
                    folderPath += $"/{(Data as EntryDataPlaylist).Title}";
                }
                if (Type is VideoType.MEMBER)
                {
                    folderPath += $"/{(Data as EntryDataMember).PlaylistTitle}";
                }
            }


            string _format = settings.isAudio
                ? " --extract-audio --audio-format mp3"
                : " --format \"bv*[ext=mp4]+ba[ext=m4a]/b[ext=mp4] / bv*+ba/b\"";

            string numbering = settings.automaticNumbering
                ? "%(playlist_index)s "
                : string.Empty;

            string playlist = settings.isPlaylist
                ? " --yes-playlist"
                : " --no-playlist";

            StatusDownloading();

            //Task.Run

            await Task.Run(() =>
            {
                ProcessStartInfo process = new("./youtube-dl.exe")
                {
                    Arguments = (
                        $"\"{URL}\"" +
                        $" -o \"{folderPath}/{numbering}%(title)s.%(ext)s\"" +
                        _format +
                        playlist
                    ),
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using Process downloadProcess = new()
                {
                    StartInfo = process,
                    EnableRaisingEvents = true
                };

                Utility.Logger.Log(process.Arguments);

                downloadProcess.Exited += (sender, e) => sdl_csharp.Download.ProcessExited(this, e);
                downloadProcess.OutputDataReceived += (sender, e) => sdl_csharp.Download.ProcessHasReceivedOutput(this, sender, e);
                downloadProcess.ErrorDataReceived += (sender, e) => sdl_csharp.Download.ProcessHasFaulted(this, sender, e);

                downloadProcess.Start();
                downloadProcess.BeginOutputReadLine();
                downloadProcess.BeginErrorReadLine();

                downloadProcess.WaitForExit();
            }).ContinueWith(
                (x) => { /* no-op */ }
            );

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
                }
                else if (Type is VideoType.MEMBER && playlistId is not null)
                {
                    await FetchMemberAsync(client);
                }
            }
            else if (playlistId is not null && Type is VideoType.PLAYLIST)
            {
                await FetchPlaylistAsync(client);
            }
            else
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
                data.Author = v.Author?.ChannelTitle ?? "[Unknown Author]";
                Logger.Log(data.Title);
                Logger.Log(data.Thumbnail);
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
                data.Author = v.Author?.ChannelTitle ?? "[Unknown Author]";
                data.PlaylistTitle = p.Title;
                data.PlaylistThumbnail = p.Thumbnails[0].Url;
                data.PlaylistCount = (ushort)pAll.Count;
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
                data.Author = p.Author?.ChannelTitle ?? "[Unknown Author]";
                data.Count = (ushort)pAll.Count;
                Logger.Log(data.Title);
            }
        }
    }
}
