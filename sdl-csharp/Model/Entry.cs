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
            var entryvms = SettingsViewModel.Instance.EntryViewModels;
            Thread.ThreadSafeAction(() => entryvms.Remove(entryvms.First((vm) => vm.entry == this)));
        }

        public async Task Download()
        {
            try
            {

                ((IEntry)data).Reset();

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
           
                Task task = Task.Run(async () =>
                {
                    //try
                    //{
                        downloadStart = DateTime.Now;
                        Status = EntryStatus.DOWNLOADING;

                        using Process downloadProcess = new()
                        {
                            StartInfo = process,
                            EnableRaisingEvents = true,
                        };

                        Logger logger = new(this);

                        downloadProcess.Exited += (sender, e) =>
                        {
                            try
                            {
                                Event.DownloadProcess.Exited(this, sender, e);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("ERR exited" + ex.Message);
                            }
                        };

                        downloadProcess.ErrorDataReceived += (sender, e) =>
                        {
                            try
                            {
                                Event.DownloadProcess.ErrorDataReceived(this, sender, e);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("ERR error data received " + ex.Message);
                            }
                        };

                        downloadProcess.OutputDataReceived += (sender, e) =>
                        {
                            try
                            {
                                Event.DownloadProcess.OutputDataReceived(this, sender, e);
                                logger.LogToFile(this, e.Data);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("ERR output data received " + ex.Message);

                            }
                        };

                        downloadProcess.Start();
                        downloadProcess.BeginOutputReadLine();
                        downloadProcess.BeginErrorReadLine();

                        //downloadProcess.WaitForExit();

                        await downloadProcess.WaitForExitAsync();

                        //MessageBox.Show("WaitForExitAsync");

                        //downloadProcess.CancelOutputRead();
                        //downloadProcess.CancelErrorRead();
                        //downloadProcess.Close();

                    //    Status = EntryStatus.DONE;
                    //}
                    //catch
                    //{
                    //    Status = EntryStatus.ERROR;
                    //}
                });

                try
                {
                    await task;
                    Status = EntryStatus.DONE;
                }
                catch (Exception e)
                {
                    Logger.Log("wtf " + e.Message);
                    Status = EntryStatus.ERROR;
                }
            }
            catch (Exception e)
            {
                Logger.Log("miapicsa ", e.Message);
                MessageBox.Show("miapicsa ", e.Message);

            }
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
