using System.Windows;
//using System.Windows.Forms;
using System.Diagnostics;
using TextBox = System.Windows.Controls.TextBox;
using Button = System.Windows.Controls.Button;
using System.Collections.ObjectModel;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace sdl
{
    public partial class SDLWindow : Window
    {
        const string URLInputFieldPlaceholder = "Select URL";
        const string FolderInputPlaceholder = "Select folder";

        public struct URLEntry
        {
            public string entry { get; set; }
            public URLEntry(string urlString)
            {
                entry = urlString;
            }
        };

        public ObservableCollection<URLEntry> URLEntries = new();
        public string FolderPath;

        private void removeEntry(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            URLEntry entry = (URLEntry)button.DataContext;
            URLEntries.Remove(entry);
        }

        private void addURL(object sender, RoutedEventArgs e)
        {
            string newURL = URLInput.Text;
            URLInput.Text = URLInputFieldPlaceholder;

            if (newURL == URLInputFieldPlaceholder)
            {
                MessageBox.Show("Please select a valid URL.");
                return;
            }

            if (newURL != string.Empty && newURL.StartsWith("http")) {
                URLEntry urlEntry = new(newURL);
                URLEntries.Add(urlEntry);

                return;
            }

            MessageBox.Show("URL Invalid");
        }

        private void clearURLInputPlaceholder(object sender, RoutedEventArgs e)
        {
            TextBox input = (TextBox) sender;

            if (input.Text == URLInputFieldPlaceholder)
            {
                input.Clear();
            }
        }

        private void addURLInputPlaceholder(object sender, RoutedEventArgs e)
        {
            TextBox input = (TextBox) sender;

            if (input.Text == string.Empty)
            {
                input.Text = URLInputFieldPlaceholder;
            }
        }

        private void Download(bool downloadSingle = true)
        {
            ProcessStartInfo process = new ProcessStartInfo("youtube-dl");
            //process.Arguments = $"\"{url}\" -o \"{folderPath}/%(title)s.%(ext)s\" -x --audio-format mp3 {(downloadSingle ? "--no-playlist" : string.Empty)}";
            //process.CreateNoWindow = true;

            using (Process ytdl = Process.Start(process)) // TODO add safety checks for inputs
            {
                ytdl.WaitForExit();
                
            }
        }
        private void toggleUrlLock(object sender, RoutedEventArgs e)
        {

        }
        private void DownloadSingle(object sender, RoutedEventArgs e)
        {
            //URLInput.Text;
            //System.Windows.MessageBox.Show(folderPath);
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
        private void selectFolder(object sender, RoutedEventArgs e)
        {
            //FolderBrowserDialog folderDialog = new FolderBrowserDialog();

            CommonOpenFileDialog folderDialog = new();
            folderDialog.IsFolderPicker = true;

            if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                FolderPath = folderDialog.FileName;
            }

            if (FolderPath == string.Empty)
            {
                FolderPathInput.Text = FolderInputPlaceholder;
                return;
            }

            FolderPathInput.Text = FolderPath;

            //MessageBox.Show(FolderPath);
            //if (folder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    string[] selectedFolders = betterFolderBrowser1.SelectedFolders;

            //    // If you've disabled multi-selection, use 'SelectedFolder'.
            //    // string selectedFolder = betterFolderBrowser1.SelectedFolder;
            //}
        }
    }
}
