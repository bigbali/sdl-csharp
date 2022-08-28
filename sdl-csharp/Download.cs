using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using sdl;

namespace sdl_csharp
{
    internal static class Download
    {
        public static SDLWindow SDLWindowReference { get; set; }
        private static void ProcessExited(URLEntry url, EventArgs e) // sender will already be destroyed
        {
            SDLWindowReference.WindowSettings.UIContext.Send((x) => // allow changing URLEntries from a thread other than main
            {
                if (SDLWindowReference.WindowSettings.RemoveEntries)
                {
                    SDLWindowReference.WindowSettings.URLEntries.Remove(url);
                    return;
                }

                url.StatusDone();
            }, null);
        }

        // Called regardless of error status, apparently
        // At this point, process has net yet been terminated
        private static void ProcessHasFaulted(URLEntry entry, object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine($"Fault: {e.Data}");
        }

        private static void ProcessHasReceivedOutput(URLEntry entry, object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
        }

        private static bool SetPlaylistData(URLEntry url, string key, object value)
        {
            var p = url.Data.Playlist;

            switch (key)
            {
                case "title":
                    p.Title = (string)value;
                    return true;

                case "playlist_count":
                    p.Count = Convert.ToUInt16(value);
                    return true;
                default:
                    return false;
            }
        }

        private static bool SetSingleData(URLEntry url, string key, object value, bool isPlaylistMember = false)
        {
            return false;
        }

        public static void BeginDownload(URLEntry url, string folderPath, bool isPlaylist)
        {
            string _folderPath = folderPath + 
                ((SDLWindowReference.WindowSettings.SubFolderPath.Length > 0 && SDLWindowReference.WindowSettings.UseSubFolderPath == true)
                    ? $"/{SDLWindowReference.WindowSettings.SubFolderPath}"
                    : string.Empty);

            // if infer is set and url refers to playlist, use playlist name instead of this
            //if (SDLWindowReference.WindowSettings.InferSubFolderPath && url.Data.IsPlaylist && url.Data.PlaylistTitle != null)
            //{
            //    _folderPath = $"${folderPath}/{url.Data.PlaylistTitle}";
            //}

            string _format = SDLWindowReference.WindowSettings.IsAudio
                ? " --extract-audio --audio-format mp3"
                : " --format \"bv*[ext=mp4]+ba[ext=m4a]/b[ext=mp4] / bv*+ba/b\"";

            string _playlist = isPlaylist
                ? " --yes-playlist"
                : " --no-playlist";

            url.StatusDownloading();

            Task.Factory.StartNew(() =>
            {
                ProcessStartInfo process = new("youtube-dl")
                {
                    Arguments = (
                    $"\"{url.Entry}\"" +
                    $" -o \"{_folderPath}/%(title)s.%(ext)s\"" +
                    _format +
                    _playlist
                ),
                    RedirectStandardError  = true,
                    RedirectStandardOutput = true,
                    RedirectStandardInput  = true,
                    UseShellExecute        = false,
                    CreateNoWindow         = true
                };

                using Process downloadProcess = new()
                {
                    StartInfo = process,
                    EnableRaisingEvents = true
                };

                downloadProcess.Exited             += (sender, e) => ProcessExited(url, e);
                downloadProcess.OutputDataReceived += (sender, e) => ProcessHasReceivedOutput(url, sender, e);
                downloadProcess.ErrorDataReceived  += (sender, e) => ProcessHasFaulted(url, sender, e);

                downloadProcess.Start();
                downloadProcess.BeginOutputReadLine();
                downloadProcess.BeginErrorReadLine();

                downloadProcess.WaitForExit();
            }).ContinueWith(
                (x) => { /* no-op */ }
            );
        }

        private static void ReceivedData(URLEntry url, object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null || e.Data.Length == 0) return;

            JavaScriptSerializer serializer = new();
            Dictionary<string, object> data =
                serializer.Deserialize<Dictionary<string, object>>(e.Data);

            bool isPlaylist = false;
            bool isPlaylistMember = false;
            bool hasDataChanged = false; // will check if any data has been changed, so we won't refresh the UI needlessly

            if (data.TryGetValue("_type", out object _type)) // "playlist" || "video"
            {
                if ((string)_type == "playlist")
                {
                    // Check if ourl and wurl are present
                    if (data.TryGetValue("original_url", out object ourl) && data.TryGetValue("webpage_url", out object wurl)
                        && ourl != wurl // if they don't match, we have a single
                        && ((string)ourl).StartsWith("https://youtu.be")) // in this case we expect the video to be a member of a playlist
                    {
                        isPlaylistMember = true;
                        Console.WriteLine("===============================================");
                        Console.WriteLine("========== ENTRY IS PLAYLIST MEMBER! ==========");
                        Console.WriteLine("===============================================");
                    }
                    else
                    {
                        isPlaylist = true;
                        url.Data.IsPlaylist = true;
                    }
                }
                else if((string)_type != "video")
                {
                    Console.WriteLine("Something is wrong");
                    Console.WriteLine($"_type: {_type}");
                    return;
                }
            }

            foreach (KeyValuePair<string, object> pair in data)
            {
                if (pair.Value == null) continue;

                Console.WriteLine(pair.ToString());

                if ((isPlaylist && SetPlaylistData(url, pair.Key, pair.Value))
                    || (!isPlaylist && SetSingleData(url, pair.Key, pair.Value))
                    || (isPlaylistMember && SetSingleData(url, pair.Key, pair.Value, isPlaylistMember)))
                {
                    hasDataChanged = true;
                }

                if (hasDataChanged) // Manually tell WPF that we have changed this, as the feller is apparently unable to tell
                {
                    url.OnPropertyChanged("Data");
                }
            }

            Console.WriteLine(url.Data.Playlist.Title);
            Console.WriteLine(url.Data.Playlist.Count);
        }

        public static void FetchData(URLEntry url)
        {
            //string ExtractorArguments = "--extractor-args youtube:player_skip=webpage,configs,js;player_client=android,web";

            // TODO: we get playlist if we provide a url to a playlist member. We need in that case a single with a reference to the playlist!
            // download both list and single and compare original_url and webpage_url?

            Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Initiating prefetch");
                ProcessStartInfo process = new("youtube-dl")
                {
                    // --dump-single-json
                    Arguments = (
                    $"\"{url.Entry}\"" + 
                    //$" --print \"%(title)j\" --yes-playlist --flat-playlist {ExtractorArguments}"
                    $" --dump-single-json --flat-playlist"
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

                downloadProcess.OutputDataReceived += (sender, e) => ReceivedData(url, sender, e);
                downloadProcess.ErrorDataReceived  += (sender, e) => Console.WriteLine($"Fault: {e.Data}");

                downloadProcess.Start();
                downloadProcess.BeginOutputReadLine();
                downloadProcess.BeginErrorReadLine();

                downloadProcess.WaitForExit();
            }).ContinueWith(
                (x) => { /* no-op */
                }
            );
        }
    }
}
