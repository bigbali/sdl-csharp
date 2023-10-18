using System;
using System.Windows;

namespace sdl_csharp
{
    public partial class SDLWindow : Window
    {
        //private void InitDownload(object sender, RoutedEventArgs e)
        //{
        //    Utility.Logger.Log("INIT DOWNLOAD");
        //    if (Settings.Instance.EntryViewModels.Count == 0)
        //    {
        //        MessageBox.Show("Please specify some URLs to be downloaded!",
        //                        "No entries to download",
        //                         MessageBoxButton.OK);
        //        return;

        //    }

        //    bool includesDownloadedEntry = false;
        //    bool includesDownloadingEntry = false;
        //    foreach (Entry entry in Settings.Instance.EntryViewModels) // Preminilary checks
        //    {
        //        if (entry.IsDone && !includesDownloadedEntry)
        //        {
        //            includesDownloadedEntry = true;
        //        }

        //        if (entry.IsDownloading && !includesDownloadingEntry)
        //        {
        //            includesDownloadingEntry = true;
        //        }
        //    }

        //    if (includesDownloadedEntry
        //        && MessageBox.Show("One or more entries have already been downloaded.\nDo you want to retry?",
        //                            "EntryViewModels already downloaded",
        //                            MessageBoxButton.YesNo) != MessageBoxResult.Yes)
        //    {
        //        return;
        //    }

        //    if (includesDownloadingEntry)
        //    {
        //        MessageBox.Show("One or more entries have not yet finished downloading.\nPlease wait.",
        //                        "EntryViewModels are downloading",
        //                         MessageBoxButton.OK);
        //        return;

        //    }

        //    foreach (Entry entry in Settings.Instance.EntryViewModels) // For every URL, start a new thread with a download process
        //    {
        //        if (entry.IsDone)
        //        {
        //            entry.Reset();
        //        }

        //        //Download.BeginDownload(entry, SettingsViewModel.YTDLArgTemplate, SettingsViewModel.IsPlaylist);
        //    }
        //}
      
    }
}
