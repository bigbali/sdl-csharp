using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using sdl_csharp.Model;
using sdl_csharp.Model.Entry;

namespace sdl_csharp
{
    public partial class SDLWindow : Window
    {
        public void RemoveEntry(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Entry entry = (Entry) button.DataContext;
            Settings.Instance.Entries.Remove(entry);
        }
        public void AddEntry(object sender, RoutedEventArgs e)
        {
            string newURL = URLInput.Text;
            URLInput.Clear();
            Utility.Logger.Log($"Add entry {newURL}");

            if (newURL.Contains("list=RDMM") || newURL.Contains("list=RDGM"))
            {
                MessageBox.Show("This playlist cannot be downloaded because it's personalised (is bound to your YouTube profile).",
                                "EntryDataPlaylist is not valid",
                                MessageBoxButton.OK);
                return;
            }

            if (newURL != string.Empty
                && (newURL.StartsWith("https://youtu.be") || newURL.StartsWith("https://www.youtube.com"))) {
                Entry urlEntry = new(newURL);
                Settings.Instance.Entries.Add(urlEntry);

                //Utility.Logger.Log(urlEntry.ToString());

                return;
            }

            MessageBox.Show("This URL appears to be invalid.\n" +
                            "A valid URL must link to a YouTube video or playlist.",
                            "URL is invalid",
                            MessageBoxButton.OK);
        }
        
        private void InitDownload(object sender, RoutedEventArgs e)
        {
            Utility.Logger.Log("INIT DOWNLOAD");
            if (Settings.Instance.Entries.Count == 0)
            {
                MessageBox.Show("Please specify some URLs to be downloaded!",
                                "No entries to download",
                                 MessageBoxButton.OK);
                return;

            }

            bool includesDownloadedEntry = false;
            bool includesDownloadingEntry = false;
            foreach (Entry entry in Settings.Instance.Entries) // Preminilary checks
            {
                if (entry.IsDone && !includesDownloadedEntry)
                {
                    includesDownloadedEntry = true;
                }

                if (entry.IsDownloading && !includesDownloadingEntry)
                {
                    includesDownloadingEntry = true;
                }
            }

            if (includesDownloadedEntry
                && MessageBox.Show("One or more entries have already been downloaded.\nDo you want to retry?",
                                    "Entries already downloaded",
                                    MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }

            if (includesDownloadingEntry)
            {
                MessageBox.Show("One or more entries have not yet finished downloading.\nPlease wait.",
                                "Entries are downloading",
                                 MessageBoxButton.OK);
                return;

            }

            foreach (Entry entry in Settings.Instance.Entries) // For every URL, start a new thread with a download process
            {
                if (entry.IsDone)
                {
                    entry.Reset();
                }

                //Download.BeginDownload(entry, WindowSettings.YTDLArgTemplate, WindowSettings.IsPlaylist);
            }
        }
        private void SelectFolder(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog folderDialog = new();
            folderDialog.IsFolderPicker = true;

            if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Settings.Instance.FolderPath = folderDialog.FileName;
            }

            if (Settings.Instance.FolderPath == string.Empty)
            {
                MessageBox.Show("Please select a folder.",
                                "No folder selected",
                                 MessageBoxButton.OK);

                return;
            }
        }
        private void ToggleNumbering(object sender, RoutedEventArgs e)
        {
            Utility.Logger.Log($"numbering {!Settings.Instance.AutomaticNumbering}");
            Settings.Instance.AutomaticNumbering = !Settings.Instance.AutomaticNumbering;
        }
        private void TogglePlaylist(object sender, RoutedEventArgs e)
        {
            Settings.Instance.IsPlaylist = !Settings.Instance.IsPlaylist;
        }
        private void ToggleAudio(object sender, RoutedEventArgs e)
        {
            Settings.Instance.IsAudio = !Settings.Instance.IsAudio;
        }
        private void ToggleUseSubFolderPath(object sender, RoutedEventArgs e)
        {
            Settings.Instance.UseSubFolderPath = !Settings.Instance.UseSubFolderPath;
        }
        private void ToggleInferSubFolderPath(object sender, RoutedEventArgs e)
        {
            Settings.Instance.InferSubFolderPath = !Settings.Instance.InferSubFolderPath;
        }
        private void ToggleRemoveEntries(object sender, RoutedEventArgs e)
        {
            Settings.Instance.RemoveEntries = !Settings.Instance.RemoveEntries;
        }
    }
}
