using sdl_csharp.Model.Entry;
using sdl_csharp.Model;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;

namespace sdl_csharp.Event
{
    internal static class DownloadProcess
    {
        public static void Exited(Entry entry, object sender, EventArgs e)
        {
            var p = ((Process)sender);
            //p.CancelOutputRead();
            //p.CancelErrorRead();
            //entry.Status = EntryStatus.DONE;

#if DEBUG
            MessageBox.Show("EXIT " + p.ExitCode + "\n" + p.ExitTime);
#endif


            if (Settings.Instance.removeEntries)
            {
                entry.Remove();
            }
        }

        public static void ErrorDataReceived(Entry entry, object sender, DataReceivedEventArgs e)
        {
            try
            {
                Utility.Logger.Log($"ErrorDataReceived: {e.Data}");
            }
            catch (Exception ex)
            {
                Utility.Logger.Log($"ErrorDataReceived threw: {ex.Message}");
            }
        }

        public static void OutputDataReceived(Entry entry, object sender, DataReceivedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                Utility.Logger.Log($"OutputDataReceived threw: {ex.Message}");
            }
        }
    }
}
