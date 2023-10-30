using sdl_csharp.Model;
using sdl_csharp.Model.Entry;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;

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
        readonly string dirPath;
        readonly string filePath;
        readonly string logPath;

        public Logger(Entry entry)
        {
            dirPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\sdl_logs";

            string unsafeFilePath = $"sdl-{GetEntryTitle(entry)}.txt";

            // sanitize file name
            filePath = string.Join(null, unsafeFilePath.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');

            logPath = @$"{dirPath}\{filePath}";
        }

        public void LogToFile(Entry entry, string data)
        {
            if (data is null) return;

            try
            {
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
            catch (Exception ex)
            {
                MessageBox.Show($"Couldn't write to file {logPath}\n" + ex.Message);
            }
        }
    }
}
