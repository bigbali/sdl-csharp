using sdl_csharp.Model.Entry;
using System;
using System.Diagnostics;
using System.IO;

namespace sdl_csharp
{
    internal static class Download
    {
        public static SDLWindow SDLWindowReference { get; set; }
        private static DateTime downloadStartedAt;
        public static void ProcessExited(Entry entry, EventArgs e) // sender will already be destroyed
        {
            //var settings = Settings.Instance;
            //settings.UIContext.Send((x) => // allow changing URLEntries from a thread other than main
            //{
            //    if (settings.RemoveEntries)
            //    {
            //        settings.Entries.Remove(entry);
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

        public static void ProcessHasReceivedOutput(Entry entry, object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null) return;

            lock (_logLock)
            {
                File.AppendAllText(
                    $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\{$"sdl-{downloadStartedAt.ToString("yyyy-MM-dd_HH-mm-ss")}.txt"}",
                    $"{entry.URL}: {e.Data}{Environment.NewLine}");
            }
            Utility.Logger.Log(e.Data);
        }
    }
}
