using sdl_csharp.Utility;
using sdl_csharp.ViewModel;
using System;
using System.Diagnostics;
using System.Linq;
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

    public class Entry : NotifyPropertyChanged
    {
        public EntryType type;
        public dynamic data;
        public string url;
        public string videoId;
        public string playlistId;
        public DateTime downloadStart;

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

        public void Remove()
        {
            SettingsViewModel.Instance.EntryViewModels.Remove(SettingsViewModel.Instance.EntryViewModels.First((vm) => vm.entry == this));
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

                    Logger logger = new();

                    downloadProcess.Exited += (sender, e) => Event.DownloadProcess.Exited(this, e);
                    downloadProcess.ErrorDataReceived += (sender, e) => Event.DownloadProcess.ErrorDataReceived(this, sender, e);
                    downloadProcess.OutputDataReceived += (sender, e) =>
                    {
                        Event.DownloadProcess.OutputDataReceived(this, sender, e);
                        logger.LogToFile(this, e.Data);
                    };

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
                singleData.Initialize(video);
            }
        }

        async Task FetchMemberAsync(YoutubeClient client)
        {
            var video = await client.Videos.GetAsync($"https://www.youtube.com/watch?v={videoId}");
            var playlist = await client.Playlists.GetAsync($"https://youtube.com/playlist?list={playlistId}");
            var playlistEntries = await client.Playlists.GetVideosAsync($"https://youtube.com/playlist?list={playlistId}");

            if (data is EntryMemberData memberData)
            {
                memberData.Initialize(video);
                memberData.Initialize(playlist, playlistEntries);
            }
        }

        async Task FetchPlaylistAsync(YoutubeClient client)
        {
            var playlist = await client.Playlists.GetAsync($"https://youtube.com/playlist?list={playlistId}");
            var playlistEntries = await client.Playlists.GetVideosAsync($"https://youtube.com/playlist?list={playlistId}");

            if (data is EntryPlaylistData playlistData)
            {
                playlistData.Initialize(playlist, playlistEntries);
            }
        }
    }
}
