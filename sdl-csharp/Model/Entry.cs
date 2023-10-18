using sdl_csharp.Utility;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Videos;

namespace sdl_csharp.Model.Entry
{
    public enum EntryStatus
    {
        INITIALIZED,
        DOWNLOADING,
        CONVERTING,
        DONE,
        ERROR
    }
    public enum EntryType
    {
        SINGLE,
        MEMBER,
        PLAYLIST
    }

    public partial class Entry : NotifyPropertyChanged
    {
        public EntryType type;
        public dynamic data;
        public string url;
        public string videoId;
        public string playlistId;
        public DateTime downloadStart;

        private float? downloadPercent;
        public float? DownloadPercent { get => downloadPercent; set  { Logger.Log(value.ToString()); Set(ref downloadPercent, value); } }

        // this is updated from inside the class
        private EntryStatus status;
        public EntryStatus Status { get => status; set => Set(ref status, value); }

        public Entry(string _url)
        {
            Logger.Log($"Entry {_url}");

            url = _url;
            type = EntryParser.GetType(url, ref videoId, ref playlistId) ?? type;
            data = type switch
            {
                EntryType.SINGLE => new EntrySingleData(),
                EntryType.MEMBER => new EntryMemberData(),
                EntryType.PLAYLIST => new EntryPlaylistData(),
                _ => null
            };

            _ = FetchAsync();
        }

        public async Task Download()
        {
            ProcessStartInfo process = new("./youtube-dl.exe")
            {
                Arguments = (
                    $"\"{url}\" -o {Settings.Instance.argTemplateString}"
                ),
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Logger.Log(process.Arguments);
           
            Task task = Task.Run(() =>
            {
                try
                {
                    downloadStart = DateTime.Now;
                    Status = EntryStatus.DOWNLOADING;

                    using Process downloadProcess = new()
                    {
                        StartInfo = process,
                        EnableRaisingEvents = true
                    };

                    downloadProcess.Exited += (sender, e) => sdl_csharp.Download.ProcessExited(this, e);
                    downloadProcess.OutputDataReceived += (sender, e) => sdl_csharp.Download.ProcessHasReceivedOutput(this, sender, e);
                    downloadProcess.ErrorDataReceived += (sender, e) => sdl_csharp.Download.ProcessHasFaulted(this, sender, e);

                    downloadProcess.Start();
                    downloadProcess.BeginOutputReadLine();
                    downloadProcess.BeginErrorReadLine();

                    downloadProcess.WaitForExit();

                    Status = EntryStatus.DONE;
                }
                catch
                {
                    Status = EntryStatus.ERROR;
                }
            });

            await task;
        }

        public async Task FetchAsync()
        {
            HttpClient httpClient = new();
            YoutubeClient client = new(httpClient);

            if (videoId is not null)
            {
                if (type is EntryType.SINGLE)
                {
                    await FetchSingleAsync(client);
                }
                else if (type is EntryType.MEMBER && playlistId is not null)
                {
                    await FetchMemberAsync(client);
                }
            }
            else if (playlistId is not null && type is EntryType.PLAYLIST)
            {
                await FetchPlaylistAsync(client);
            }
            else
            {
                Logger.Log($"URL appears to be invalid: {url}");
            }
        }

        async Task FetchSingleAsync(YoutubeClient client)
        {
            Video video = await client.Videos.GetAsync($"https://www.youtube.com/watch?v={videoId}");
            if (data is EntrySingleData singleData)
            {
                singleData.Set(video);
            }
        }

        async Task FetchMemberAsync(YoutubeClient client)
        {
            var video = await client.Videos.GetAsync($"https://www.youtube.com/watch?v={videoId}");
            var playlist = await client.Playlists.GetAsync($"https://youtube.com/playlist?list={playlistId}");
            var playlistEntries = await client.Playlists.GetVideosAsync($"https://youtube.com/playlist?list={playlistId}");

            if (data is EntryMemberData memberData)
            {
                memberData.Set(video);
                memberData.Set(playlist, playlistEntries);
            }
        }

        async Task FetchPlaylistAsync(YoutubeClient client)
        {
            var playlist = await client.Playlists.GetAsync($"https://youtube.com/playlist?list={playlistId}");
            var playlistEntries = await client.Playlists.GetVideosAsync($"https://youtube.com/playlist?list={playlistId}");

            if (data is EntryPlaylistData playlistData)
            {
                playlistData.Set(playlist, playlistEntries);
            }
        }
    }
}
