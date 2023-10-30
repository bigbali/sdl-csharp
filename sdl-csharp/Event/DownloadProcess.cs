using sdl_csharp.Model.Entry;
using sdl_csharp.Model;
using sdl_csharp.Utility;
using System;
using System.Diagnostics;

namespace sdl_csharp.Event
{
    internal static class DownloadProcess
    {
        public static void Exited(Entry entry, object sender, EventArgs e)
        {
#if DEBUG
            Process p = ((Process)sender);
            Logger.Log("EXIT " + p.ExitCode + "\n" + p.ExitTime);
#endif
            if (Settings.Instance.removeEntries && entry.Status is not EntryStatus.ERROR)
            {
                entry.Remove();
            }
        }

        public static void ErrorDataReceived(Entry entry, object sender, DataReceivedEventArgs e)
        {
            try
            {
                Logger.Log($"ErrorDataReceived: {e.Data}");
            }
            catch (Exception ex)
            {
                Logger.Log($"ErrorDataReceived threw: {ex.Message}");
            }
        }

        public static void OutputDataReceived(Entry entry, object sender, DataReceivedEventArgs e)
        {
            try
            {
                if (e.Data == null) return;                

                EntryProgressParser.ParseOutput(entry, e.Data);
            }
            catch (Exception ex)
            {
                Logger.Log($"OutputDataReceived threw: {ex.Message}");
            }
        }
    }
}
