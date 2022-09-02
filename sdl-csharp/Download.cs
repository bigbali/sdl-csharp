using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Windows;
using sdl;
using sdl_csharp.Model;
using VideoLibrary;

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
            var p = url.Data.Single;

            if (isPlaylistMember)
            {
                //p.IsPlaylistMember = true;

                switch (key)
                {
                    case "entries":
                        foreach (Dictionary<string, object>item in (ArrayList)value)
                        {
                            foreach (KeyValuePair<string, object> pair in item)
                            {
                                Console.WriteLine("==================");
                                Console.WriteLine(pair.ToString());
                            }
                        }
                        return false;
                    //case "duration":
                    //    p.Length = (ushort)(Convert.ToUInt16(value) / 60);
                    //    return true;
                    //case "thumbnail":
                    //    p.Thumbnail = (string)value;
                    //    return true;
                    default:
                        return false;
                }
            }

            switch (key)
            {
                case "title":
                    p.Title = (string)value;
                    return true;
                case "duration":
                    //p.Length = (ushort)(Convert.ToUInt16(value) / 60);
                    return true;
                case "thumbnail":
                    p.Thumbnail = (string)value;
                    return true;
                default:
                    return false;
            }
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
                        && ourl != wurl                                   // here, if they don't match we presume we have a single
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
                        //url.Data.IsPlaylist = true;
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

        public static string GetVideoKindByURI(URLEntry url, NameValueCollection queryParams)
        {
            try
            {
                if (url.Entry.StartsWith("https://www.youtube.com/watch"))
                {
                    // SINGLE
                    if (queryParams["v"] is not null && queryParams["list"] is null)
                    {
                        Console.WriteLine("SINGLE");
                        return EntryData.VideoType.SINGLE; ;
                    }

                    // PLAYLIST MEMBER
                    if (queryParams["v"] is not null
                        && queryParams["list"] is not null
                        && queryParams["index"] is not null)
                    {
                        Console.WriteLine("MEMBER");
                        return EntryData.VideoType.PLAYLIST_MEMBER;
                    }

                    // PLAYLIST
                    if (queryParams["list"] is not null)
                    {
                        Console.WriteLine("PLAYLIST");
                        return EntryData.VideoType.PLAYLIST;
                    }
                }

                // PLAYLIST
                if (url.Entry.StartsWith("https://www.youtube.com/playlist")
                    && queryParams["list"] is not null)
                {
                    Console.WriteLine("PLAYLIST");
                    return EntryData.VideoType.PLAYLIST;
                }

                if (url.Entry.StartsWith("https://youtu.be"))
                {
                    // SINGLE
                    if (queryParams["v"] is null && queryParams["list"] is null)
                    {
                        Console.WriteLine("SINGLE");
                        return EntryData.VideoType.SINGLE;
                    }

                    // PLAYLIST MEMBER
                    if (queryParams["list"] is not null)
                    {
                        Console.WriteLine("MEMBER");
                        return EntryData.VideoType.PLAYLIST_MEMBER;
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($"DEBUG: {e.Message}");
            }

            return null;
        }

        public static void FetchData(URLEntry url)
        {
            Task.Factory.StartNew(() =>
            {
                //YouTube     client = YouTube.Default; // can't handle /playlist <--
                //YouTubeVideo video = client.GetVideo(url.Entry);

                //var videoSplitByList = url.Entry.Split(new[] { "?list=" }, StringSplitOptions.None);

                //string videoId = videoSplitByList[0].Remove(0,
                //    (isPlaylist 
                //        ? "https://www.youtube.com/" 
                //        : "https://youtu.be/"
                //    ).Length);

                //string playlistId = videoSplitByList.Length > 1
                //    ? videoSplitByList[1]
                //    : null;

                var uri = new UriBuilder(url.Entry);
                NameValueCollection queryParams = HttpUtility.ParseQueryString(uri.Query);

                url.Data.Type = GetVideoKindByURI(url, queryParams);

                MessageBox.Show(url.Data.Type);

                // Playlist
                // https://www.youtube.com/watch?v=QMMJpFvGMlU&list=OLAK5uy_k0voqAMErjYWLKExWDMZFEWpInRV-fW5w
                // https://www.youtube.com/playlist?list=PLidIjcybOMhyKTdhJUd4STFOEJWsQZAeZ

                // Playlist member
                // https://youtu.be/mgSHazBbDNU?list=RDMM
                // https://www.youtube.com/watch?v=mgSHazBbDNU&list=RDMM&index=26

                // Single
                // https://youtu.be/dBW0_4wJ1kA
                // https://www.youtube.com/watch?v=EJKFKLvWN0Y

                //Console.WriteLine($"vID: {videoId}");
                //Console.WriteLine($"pID: {playlistId}");
            }).ContinueWith(
                (x) => { /* no-op */
                }
            );
        }
    }
}
