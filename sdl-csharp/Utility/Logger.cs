using sdl_csharp.Model;
using sdl_csharp.Model.Entry;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace sdl_csharp.Utility
{
    public class Logger
    {
        public static void Log(string text = null,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
        {
            Trace.WriteLine($"{Path.GetFileName(file)} {member} line {line}{(text is null ? string.Empty : $": {text}")}");
        }

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

        uint logCount;

        public void LogToFile(Entry entry, string data)
        {
            if (data is null) return;

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
                $"{entry.url}: {data}{Environment.NewLine}");
            logCount++;
        }
    }
}
