using sdl_csharp.Utility;
using System.Collections.ObjectModel;

namespace sdl_csharp.Model
{
    public class Settings : NotifyPropertyChanged
    {
        static Settings instance;
        public static Settings Instance
        {
            get
            {
                instance ??= new Settings();
                return instance;
            }
        }
        Settings()
        {
            argTemplate = new ArgTemplate(this);
            argTemplateString = argTemplate.Template;
        }

        public bool useSubFolder;
        public bool inferFolderByPlaylistTitle;
        public bool inferFolderByAuthor;
        public bool automaticNumbering;
        public bool removeEntries;
        public bool isPlaylist = true;
        public bool isAudio = true;
        public string folderPath = string.Empty;
        public string subFolderPath = string.Empty;
        public string argTemplateString;
        public ArgTemplate argTemplate;
        public ObservableCollection<Entry.Entry> entries = new();
    }
}
