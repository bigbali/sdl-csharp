using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using WK.Libraries.BetterFolderBrowserNS;

namespace simple_ytdl_cs
{
    public partial class SYTDLWindow : Window
    {
        private string url = string.Empty;
        private string folderPath = string.Empty;

        private void setUrl(object sender, RoutedEventArgs e)
        {
            url = URLInput.Text;
        }
        private void setFolderPath(object sender, RoutedEventArgs e)
        {
            folderPath = FolderPathInput.Text;
        }

        private async void Download(bool downloadSingle = true)
        {
            ProcessStartInfo process = new ProcessStartInfo("./youtube-dl");
            process.Arguments = $"\"{url}\" -o \"{folderPath}/%(title)s.%(ext)s\" -x --audio-format mp3 {(downloadSingle ? "--no-playlist" : string.Empty)}";
            //process.CreateNoWindow = true;

            using (Process ytdl = Process.Start(process))
            {
                Task exit = ytdl.WaitForExitAsync();
                await exit;
                Console.WriteLine(ytdl.ExitCode);
                //MessageBox.Show(process.Arguments);
                //MessageBox.Show(url);
                //MessageBox.Show(folderPath);
                //MessageBox.Show(mode);
                //MessageBox.Show(exit.IsCompleted.ToString());
                //MessageBox.Show(exit.IsCompletedSuccessfully.ToString());
            }
        }
        private void toggleUrlLock(object sender, RoutedEventArgs e)
        {

        }
        private void DownloadSingle(object sender, RoutedEventArgs e)
        {
            //URLInput.Text;
            //MessageBox.Show(Globals.url);
            //Globals.url = urlInputField.Text;
            //sURL = sUrl.Text;
            //MessageBox.Show(sURL);
            Download();
        }
        private void DownloadPlaylist(object sender, RoutedEventArgs e)
        {
            Download(false);
        }
        private void toggleFolderLock(object sender, RoutedEventArgs e)
        {
            
        }
        private void SelectFolder(object sender, RoutedEventArgs e)
        {
            BetterFolderBrowser folder = new BetterFolderBrowser();
            folder.Title = "Select download folder";
            folder.Multiselect = false;
            folder.RootFolder = "./";

            //if (folder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    string[] selectedFolders = betterFolderBrowser1.SelectedFolders;

            //    // If you've disabled multi-selection, use 'SelectedFolder'.
            //    // string selectedFolder = betterFolderBrowser1.SelectedFolder;
            //}
        }
    }
}
