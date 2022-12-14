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
using VideoLibrary;
using YoutubeExplode;
using System.Net.Http;
using YoutubeExplode.Common;
using sdl;
using sdl_csharp.Model;
using static sdl_csharp.Model.EntryData;

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

        public static void BeginDownload(URLEntry url, string folderPath, bool isPlaylist)
        {
            string _folderPath = folderPath != string.Empty
                ? folderPath // If there is no folder path set, use desktop instead
                : Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\SDL Downloads";

            if (SDLWindowReference.WindowSettings.UseSubFolderPath)
            {
                _folderPath += $"/{SDLWindowReference.WindowSettings.SubFolderPath}";
            }

            if (SDLWindowReference.WindowSettings.InferSubFolderPath) // If inferring is enabled, infer from playlist title
            {
                if (url.Data.Type is VideoType.PLAYLIST)
                {
                    _folderPath += $"/{(url.Data.Data as PLAYLIST).Title}";
                }
                if (url.Data.Type is VideoType.PLAYLIST_MEMBER)
                {
                    _folderPath += $"/{(url.Data.Data as PLAYLIST_MEMBER).PlaylistTitle}";
                }
            }

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

        public static string GetVideoKindByURI(
            URLEntry url,
            NameValueCollection queryParams,
            ref string vId,
            ref string pId)
        {
            try
            {
                if (url.Entry.StartsWith("https://www.youtube.com/watch"))
                {
                    // SINGLE
                    if (queryParams["v"] is not null && queryParams["list"] is null)
                    {
                        Console.WriteLine("SINGLE");

                        vId = queryParams["v"];

                        return VideoType.SINGLE; ;
                    }

                    // PLAYLIST MEMBER
                    if (queryParams["v"] is not null
                        && queryParams["list"] is not null
                        && queryParams["index"] is not null)
                    {
                        Console.WriteLine("MEMBER");

                        vId = queryParams["v"];
                        pId = queryParams["list"];

                        return VideoType.PLAYLIST_MEMBER;
                    }

                    // PLAYLIST
                    if (queryParams["list"] is not null)
                    {
                        Console.WriteLine("PLAYLIST");

                        pId = queryParams["list"];

                        return VideoType.PLAYLIST;
                    }
                }

                // PLAYLIST
                if (url.Entry.StartsWith("https://www.youtube.com/playlist")
                    && queryParams["list"] is not null)
                {
                    pId = queryParams["list"];
                    return VideoType.PLAYLIST;
                }

                if (url.Entry.StartsWith("https://youtu.be"))
                {
                    // SINGLE
                    if (queryParams["v"] is null && queryParams["list"] is null)
                    {
                        vId = url.Entry.Remove(0, "https://youtu.be/".Length);
                        return VideoType.SINGLE;
                    }

                    // PLAYLIST MEMBER
                    if (queryParams["v"] is null && queryParams["list"] is not null)
                    {
                        string vIdFirstPartRemoved = url.Entry.Remove(0, "https://youtu.be/".Length);
                        int vIdListParamStartIndex = vIdFirstPartRemoved.IndexOf('?'); // Presume ?list= starts at this point

                        vId = vIdFirstPartRemoved.Remove(vIdListParamStartIndex);
                        pId = queryParams["list"];

                        return VideoType.PLAYLIST_MEMBER;
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($"DEBUG_GET_VIDEO_KIND_BY_URL_EXCEPTION: {e.Message}");
            }

            return null;
        }

        public static async void FetchData(URLEntry url)
        {
            UriBuilder uri = new(url.Entry);
            NameValueCollection queryParams = HttpUtility.ParseQueryString(uri.Query);

            string vId = null;
            string pId = null;

            url.Data.Type = GetVideoKindByURI(url, queryParams, ref vId, ref pId);

            //MessageBox.Show($"Type: {url.Data.Type}\n" +
            //                $"VID: {vId}\n" +
            //                $"PID: {pId}\n");

            HttpClient httpClient = new();
            YoutubeClient client = new(httpClient);

            // SINGLE
            if (vId is not null && url.Data.Type is VideoType.SINGLE)
            {
                var v = await client.Videos.GetAsync($"https://www.youtube.com/watch?v={vId}");

                (url.Data.Data as SINGLE).Title     = v.Title;
                (url.Data.Data as SINGLE).Thumbnail = v.Thumbnails[0].Url;
                (url.Data.Data as SINGLE).Duration  = v.Duration.ToString();
            }

            // MEMBER
            if (vId is not null && pId is not null && url.Data.Type is VideoType.PLAYLIST_MEMBER)
            {
                var v = await client.Videos.GetAsync($"https://www.youtube.com/watch?v={vId}");
                var p = await client.Playlists.GetAsync($"https://youtube.com/playlist?list={pId}");
                var pAllVideos = await client.Playlists.GetVideosAsync($"https://youtube.com/playlist?list={pId}");

                (url.Data.Data as PLAYLIST_MEMBER).MemberTitle       = v.Title;
                (url.Data.Data as PLAYLIST_MEMBER).MemberThumbnail   = v.Thumbnails[0].Url;
                (url.Data.Data as PLAYLIST_MEMBER).MemberDuration    = v.Duration.ToString();
                (url.Data.Data as PLAYLIST_MEMBER).PlaylistTitle     = p.Title;
                (url.Data.Data as PLAYLIST_MEMBER).PlaylistThumbnail = p.Thumbnails[0].Url;
                (url.Data.Data as PLAYLIST_MEMBER).PlaylistCount     = (ushort) pAllVideos.Count;
            }

            // PLAYLIST
            if (pId is not null && url.Data.Type is VideoType.PLAYLIST)
            {
                var p = await client.Playlists.GetAsync($"https://youtube.com/playlist?list={pId}");
                var pAllVideos = await client.Playlists.GetVideosAsync($"https://youtube.com/playlist?list={pId}");

                (url.Data.Data as PLAYLIST).Title     = p.Title;
                (url.Data.Data as PLAYLIST).Thumbnail = p.Thumbnails[0].Url;
                (url.Data.Data as PLAYLIST).Count     = (ushort)pAllVideos.Count;
            }

            if (vId is null && pId is null) // Presume that URL is invalid
            {
                url.StatusInvalid();
                MessageBox.Show($"{url.Entry} appears to be invalid.",
                                "URL is not valid",
                                MessageBoxButton.OK);
            }

            url.Data.DEBUG_PRINT_SINGLE();
            url.Data.DEBUG_PRINT_MEMBER();
            url.Data.DEBUG_PRINT_PLAYLIST();
        }
    }
}
