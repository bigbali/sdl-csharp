using System;
using sdl_csharp.Utility;

namespace sdl_csharp.Model
{
    public class ArgTemplate : NotifyPropertyChanged
    {
        string template;
        public string Template
        {
            get => template;
            set => Set(ref template, value);
        }

        public ArgTemplate(Settings s)
        {
            string folderPath = BuildFolderPath(s);
            string fileName = BuildFileName(s);
            string format = BuildFormat(s);
            string kind = BuildKind(s);
            string misc = BuildMisc(s);
            string hooks = BuildHooks(s);

            Template = $"\"{folderPath + fileName}\"" + format + kind + misc + hooks;
        }

        private static string BuildFolderPath(Settings s)
        {
            string folderPath = s.folderPath != string.Empty
                ? s.folderPath
                : Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\SDL Downloads";

            if (s.useSubFolder)
            {
                folderPath += @$"\{s.subFolderPath}";
            }

            if (s.inferFolderByAuthor)
            {
                 folderPath += @"\[author]";
            }

            if (s.inferFolderByPlaylistTitle)
            {
                folderPath += @"\%(playlist_title)s";
            }

            return folderPath;
        }

        static string BuildFormat(Settings s) => s.isAudio
                ? " --extract-audio --audio-format mp3"
                : " --format \"bv*[ext=mp4]+ba[ext=m4a]/b[ext=mp4] / bv*+ba/b\"";

        static string BuildFileName(Settings s)
        {
            string numbering = s.automaticNumbering
                ? "%(playlist_index)s "
                : string.Empty;

            return @$"\{numbering}%(title)s.%(ext)s";
        }

        static string BuildKind(Settings s) => s.isPlaylist
                ? " --yes-playlist"
                : " --no-playlist";

        static string BuildMisc(Settings s) => " --force-overwrites --postprocessor-args ffmpeg:-stats --audio-quality 0 --compat-options no-youtube-unavailable-videos --verbose";

        static string BuildHooks(Settings s)
        {
            string download = " --progress-template \"download:" +
                "[progress] " +
                "STATUS:%(progress.status)s" +
                "|DOWNLOADED:%(progress.downloaded_bytes)s" +
                "|TOTAL:%(progress.total_bytes)s" +
                "|ETA:%(progress.eta)s" +
                "|SPEED:%(progress.speed)s" +
                "|ELAPSED:%(progress.elapsed)s\"";

            string postprocess = " --progress-template \"postprocess:" +
                "[postprocess] " +
                "POSTPROCESS_STATUS:%(progress.status)s\"";

            return $"{download}{postprocess}";
        }
    }
}
