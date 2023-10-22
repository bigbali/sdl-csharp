using sdl_csharp.Model.Entry;
using sdl_csharp.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace sdl_csharp.Event
{
    internal static class DownloadProcess
    {
        public static void Exited(Entry entry, EventArgs e)
        {
            if (Settings.Instance.removeEntries)
            {
                entry.Remove();
            }
        }

        // Called regardless of error status, apparently
        // At this point, process has net yet been terminated
        public static void ErrorDataReceived(Entry entry, object sender, DataReceivedEventArgs e)
        {
            Utility.Logger.Log($"Fault: {e.Data}");
        }

        public static void OutputDataReceived(Entry entry, object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null) return;

            if (e.Data.Contains("[download]"))
            {
                entry.Status = EntryStatus.DOWNLOADING;

                // when starting download of a playlist member
                if (e.Data.Contains("Downloading item") && (entry.type is EntryType.PLAYLIST || entry.type is EntryType.MEMBER))
                {
                    entry.data.PlaylistDownloadIndex++;
                }

                // extract download percent from output
                Match match = Regex.Match(e.Data, @"(\d+\.\d+)%");

                if (match.Success)
                {
                    string floatString = match.Value.Trim('%');

                    float floatValue = float.Parse(floatString);

                    if (entry.data is EntrySingleData)
                    {
                        ((EntrySingleData)entry.data).DownloadPercent = floatValue;
                    }
                    else
                    {
                        entry.data.PlaylistMemberDownloadPercent = floatValue;
                    }
                }
            }

            if (e.Data.Contains("[ExtractAudio]"))
            {
                entry.Status = EntryStatus.CONVERTING;
            }
        }
    }
}
