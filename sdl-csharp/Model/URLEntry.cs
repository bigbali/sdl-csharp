using sdl_csharp.Model;
using sdl_csharp.Utility;
using static sdl_csharp.Resource.Global;

namespace sdl
{
    public class URLEntry : NotifyPropertyChanged
    {
        private string label         = DownloadLabel.Default;
        private bool   isDownloading = false;
        private bool   isDone        = false;
        private bool   isInvalid     = false;
        public EntryData Data { get; set; } = new();
        public string Entry { get; set; }
        public bool IsDone {
            get => isDone;
            set => Set(ref isDone, value);
        }
        public bool IsDownloading {
            get => isDownloading;
            set => Set(ref isDownloading, value);
        }
        public bool IsInvalid
        {
            get => isInvalid;
            set => Set(ref isInvalid, value);
        }
        public string Label
        {
            get => label;
            set => Set(ref label, value);
        }
        public void StatusDownloading()
        {
            IsDownloading = true;
            IsDone = false;
            Label = DownloadLabel.Downloading;
        }
        public void StatusDone()
        {
            IsDownloading = false;
            IsDone = true;
            Label = DownloadLabel.Done;
        }
        public void StatusInvalid()
        {
            IsDownloading = false;
            IsDone = false;
            IsInvalid = true;
            Label = DownloadLabel.Default;
        }
        public void Reset()
        {
            IsDownloading = false;
            IsDone = false;
            Label = DownloadLabel.Default;
        }
        public URLEntry(string urlString)
        {
            Entry = urlString;
        }
    }
}
