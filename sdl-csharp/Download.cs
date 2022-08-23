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

        public static void BeginDownload(URLEntry url, string folderPath, bool isPlaylist)
        {
            string _folderPath = folderPath + 
                ((SDLWindowReference.WindowSettings.SubFolderPath.Length > 0 && SDLWindowReference.WindowSettings.UseSubFolderPath == true)
                    ? $"/{SDLWindowReference.WindowSettings.SubFolderPath}"
                    : string.Empty);

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

        public static void ReceivedData(URLEntry url, object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null && e.Data.Length > 0)
            {
                //Console.WriteLine(e.Data);
                //url.SettingsJSON = e.Data;
                //MessageBox.Show("wtf");

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                Dictionary<string, object>  data = serializer.Deserialize<Dictionary<string, object>>(e.Data);


                //url.Data = data;

                foreach (KeyValuePair<string, object> y in data)
                {
                    if (y.Value != null)
                    {
                        //Console.WriteLine($"{y.Key} => {y.Value}");
                        if (y.Key == "playlist_title")
                        {
                            //url.Dudu.playlist_title = (string)y.Value;
                        }
                    }
                }
            }
        }

        public static void FetchData(URLEntry url)
        {
            Task.Factory.StartNew(() =>
            {
                ProcessStartInfo process = new("youtube-dl")
                {
                    Arguments = (
                    $"\"{url.Entry}\"" +
                    " -j --yes-playlist"
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

                downloadProcess.Start();
                downloadProcess.BeginOutputReadLine();

                downloadProcess.WaitForExit();
            }).ContinueWith(
                (x) => { /* no-op */
                    //MessageBox.Show(url.SettingsJSON);
                }
            );
        }
    }
}
