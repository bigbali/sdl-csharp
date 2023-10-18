using sdl_csharp.Model;
using sdl_csharp.Model.Entry;
using sdl_csharp.Utility;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace sdl_csharp
{
    internal static class Download
    {
        //public static SDLWindow SDLWindowReference { get; set; }
        //private static DateTime downloadStartedAt;
        public static void ProcessExited(Entry entry, EventArgs e) // sender will already be destroyed
        {
            //var settings = Settings.Instance;
            //settings.UIContext.Send((x) => // allow changing URLEntries from a thread other than main
            //{
            //    if (settings.RemoveEntries)
            //    {
            //        settings.EntryViewModels.Remove(entry);
            //        return;
            //    }

            //    entry.StatusDone();
            //}, null);
        }

        // Called regardless of error status, apparently
        // At this point, process has net yet been terminated
        public static void ProcessHasFaulted(Entry entry, object sender, DataReceivedEventArgs e)
        {
            Utility.Logger.Log($"Fault: {e.Data}");
        }

        private static readonly object _logLock = new();

        private static string GetEntryTitle(Entry entry)
        {
            return entry.data switch
            {
                EntrySingleData single => single.Title,
                EntryPlaylistData playlist => playlist.PlaylistTitle,
                EntryMemberData member => Settings.Instance.isPlaylist
                    ? member.PlaylistTitle
                    : member.Title,
                _ => "unknown title"
            };
        }

        // move out of static so we can keep count of write ops
        public static void ProcessHasReceivedOutput(Entry entry, object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null) return;

            if (e.Data.Contains("[download]"))
            {
                entry.Status = EntryStatus.DOWNLOADING;

                Match match = Regex.Match(e.Data, @"(\d+\.\d+)%");

                if (match.Success)
                {
                    string floatString = match.Value.Trim('%');

                    float floatValue = float.Parse(floatString);

                    entry.DownloadPercent = floatValue;
                }
            }

            if (e.Data.Contains("[ExtractAudio]"))
            {
                entry.Status = EntryStatus.CONVERTING;

                if (entry.type is EntryType.PLAYLIST || entry.type is EntryType.MEMBER)
                {
                    (entry.data as EntryPlaylistData).PlaylistDownloadIndex++;
                }
            }

            string dirPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\sdl_logs";
            string logPath = $"{dirPath}\\{$"sdl-{GetEntryTitle(entry)}.txt"}";

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            
            if (!File.Exists(logPath))
            {
                File.Create(logPath).Close();
            }

            File.AppendAllText(
                logPath,
                $"{entry.url}: {e.Data}{Environment.NewLine}");
        }
    }
}
