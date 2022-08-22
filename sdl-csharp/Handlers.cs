using System;
using System.Collections.Specialized;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.WindowsAPICodePack.Dialogs;
using sdl_csharp;

namespace sdl
{
    public partial class SDLWindow : Window
    {
        private void URLAdded(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                URLEntry newUrl = (URLEntry) e.NewItems[0];
                Download.FetchData(newUrl);
            }
        }

        private void RemoveEntry(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            URLEntry entry = (URLEntry) button.DataContext;
            WindowSettings.URLEntries.Remove(entry);
        }

        private void AddURL(object sender, RoutedEventArgs e)
        {
            string newURL = URLInput.Text;

            if (newURL != string.Empty && newURL.StartsWith("http")) {
                URLEntry urlEntry = new(newURL);
                WindowSettings.URLEntries.Add(urlEntry);

                return;
            }

            MessageBox.Show(newURL.ToString());
            MessageBox.Show("URL Invalid");
        }
        private void InitIndividualDownload(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            URLEntry url = (URLEntry) button.DataContext;

            if (url.IsDownloading)
            {
                MessageBox.Show("This entry has not yet finished downloading.\nPlease wait.",
                                "Entry is downloading",
                                MessageBoxButton.OK);
                return;

            }

            bool retry = url.IsDone && MessageBox.Show("This entry has already been downloaded.\nDo you want to retry?",
                                                     "Entry already downloaded",
                                                      MessageBoxButton.YesNo) == MessageBoxResult.Yes;

            if (retry)
            {
                url.Reset();
            }

            Download.BeginDownload(url, WindowSettings.FolderPath, false);
        }

        private void InitDownload(object sender, RoutedEventArgs e)
        {
            bool includesDownloadedEntry = false;
            bool includesDownloadingEntry = false;
            foreach (URLEntry url in WindowSettings.URLEntries) // Premilinary checks
            {
                if (url.IsDone && !includesDownloadedEntry)
                {
                    includesDownloadedEntry = true;
                }

                if (url.IsDownloading && !includesDownloadingEntry)
                {
                    includesDownloadingEntry = true;
                }
            }

            if (includesDownloadedEntry && MessageBox.Show("One or more entries have already been downloaded.\nDo you want to retry?",
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

            foreach (URLEntry url in WindowSettings.URLEntries) // For every URL, start a new thread with a download process
            {
                if (url.IsDone)
                {
                    url.Reset();
                }

                Download.BeginDownload(url, WindowSettings.FolderPath, WindowSettings.IsPlaylist);
            }
        }

        private void SelectFolder(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog folderDialog = new();
            folderDialog.IsFolderPicker = true;

            if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                WindowSettings.FolderPath = folderDialog.FileName;
            }

            if (WindowSettings.FolderPath == string.Empty)
            {
                MessageBox.Show("Please select a folder.",
                                "No folder selected",
                                 MessageBoxButton.OK);

                return;
            }

            //FolderPathInput.Text = FolderPath;
        }

        private void UpdateFolder(object sender, RoutedEventArgs e)
        {
            TextBox input = (TextBox) sender;
            WindowSettings.FolderPath = input.Text;
        }

        private void SetSubFolder(object sender, RoutedEventArgs e)
        {
            WindowSettings.SubFolderPath = SubFolderPathInput.Text;
        }

        private void UpdateSubFolder(object sender, RoutedEventArgs e)
        {
            TextBox subFolderInput = (TextBox) sender;
            WindowSettings.SubFolderPath = subFolderInput.Text;
        }

        private void TogglePlaylist(object sender, RoutedEventArgs e)
        {
            WindowSettings.IsPlaylist = !WindowSettings.IsPlaylist;
        }
        private void ToggleAudio(object sender, RoutedEventArgs e)
        {
            WindowSettings.IsAudio = !WindowSettings.IsAudio;
        }

        private void ToggleUseSubFolderPath(object sender, RoutedEventArgs e)
        {
            WindowSettings.UseSubFolderPath = !WindowSettings.UseSubFolderPath;
        }

        private void ToggleInferSubFolderPath(object sender, RoutedEventArgs e)
        {
            WindowSettings.InferSubFolderPath = !WindowSettings.InferSubFolderPath;
        }
        private void ToggleRemoveEntries(object sender, RoutedEventArgs e)
        {
            WindowSettings.RemoveEntries = !WindowSettings.RemoveEntries;
        }
    }
}
