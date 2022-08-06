using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using sdl;

namespace sdl_csharp
{
    internal class Download
    {
        private void ProcessExited(SDLWindow.URLEntry url, EventArgs e) // sender will already be destroyed
        {
            MessageBox.Show(url.Entry);
            SDLWindow.UIContext.Send((x) => // allow changing URLEntries from a thread other than main
            {
                SDLWindow.URLEntries.Remove(url);
            }, null);
        }

        // Called regardless of error status, apparently
        // At this point, process has net yet been terminated
        private void ProcessHasFaulted(SDLWindow.URLEntry entry, object sender, DataReceivedEventArgs e)
        {
        }

        private void ProcessHasReceivedOutput(SDLWindow.URLEntry entry, object sender, DataReceivedEventArgs e)
        {
        }

        public void BeginDownload(SDLWindow.URLEntry url, string folderPath, bool isPlaylist)
        {
            Task.Factory.StartNew(() =>
            {
                ProcessStartInfo process = new("youtube-dl")
                {
                    Arguments = (
                    $"\"{url.Entry}\"" +
                    $" -o \"{folderPath}/%(title)s.%(ext)s\"" +
                    $" -x --audio-format mp3" +
                    $" {(isPlaylist ? "--yes-playlist" : "--no-playlist")}"
                ),
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false
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
    }
}
