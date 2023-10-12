using System;

// move to model viewmodel
namespace sdl_csharp.Utility
{
    public class YTDLArgTemplate : NotifyPropertyChanged
    {
        string template;
        public string Template
        {
            get => template;
            set => Set(ref template, value);
        }

        // tried accessing the Settings instance directly, but that causes a stack overflow
        // so we stick to good old fashioned args
        public YTDLArgTemplate(Model.Settings s)
        {
            string folderPath = BuildFolderPath(s);
            string fileName = BuildFileName(s);
            string format = BuildFormat(s);
            string kind = BuildKind(s);

            Template = folderPath + fileName + format + kind;
            Logger.Log(Template);
        }

        private static string BuildFolderPath(Model.Settings s)
        {
            string folderPath = s.FolderPath != string.Empty
                ? s.FolderPath
                : Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\SDL Downloads";

            if (s.UseSubFolderPath)
            {
                folderPath += @$"\{s.SubFolderPath}";
            }

            if (s.InferSubFolderPath)
            {
                folderPath += @"\%(playlist_title)s";
            }

            return folderPath;
        }

        private static string BuildFormat(Model.Settings s) => s.IsAudio
                ? " --extract-audio --audio-format mp3"
                : " --format \"bv*[ext=mp4]+ba[ext=m4a]/b[ext=mp4] / bv*+ba/b\"";

        private static string BuildFileName(Model.Settings s)
        {
            string numbering = s.AutomaticNumbering
                ? "%(playlist_index)s "
                : string.Empty;

            return @$"\{numbering}%(title)s.%(ext)s\";
        }

        private static string BuildKind(Model.Settings s) => s.IsPlaylist
                ? " --yes-playlist"
                : " --no-playlist";
    }
}
