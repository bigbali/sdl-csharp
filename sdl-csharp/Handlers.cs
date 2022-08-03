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
        public struct InputPlaceholders
        {
            public string URLInput { set; get; }
            public string FolderPathInput { set; get; }

            public InputPlaceholders(string url, string folder)
            {
                URLInput = url;
                FolderPathInput = folder;
            }
        }

        public static readonly InputPlaceholders Placeholders  = new(
            "Select URL",
            "Select folder"
        );

        public struct URLEntry
        {
            public string entry { get; set; }
            public URLEntry(string urlString)
            {
                entry = urlString;
            }
        };

        public ObservableCollection<URLEntry> URLEntries = new();
        public string FolderPath { get; set; }
        public bool IsPlaylist { get; set; } = true;

        private static string getPlaceholder(string inputName)
        {
            return (string) Placeholders.GetType().GetProperty(inputName).GetValue(Placeholders);
        }

        private void removeEntry(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            URLEntry entry = (URLEntry)button.DataContext;
            URLEntries.Remove(entry);
        }

        private void addURL(object sender, RoutedEventArgs e)
        {
            string newURL = URLInput.Text;
            URLInput.Text = Placeholders.URLInput;

            if (newURL == Placeholders.URLInput)
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

        private void clearInputPlaceholder(object sender, RoutedEventArgs e)
        {
            TextBox input = (TextBox) sender;

            if (input.Text == getPlaceholder(input.Name))
            {
                input.Clear();
            }
        }

        private void resetInputPlaceholder(object sender, RoutedEventArgs e)
        {
            TextBox input = (TextBox) sender;

            if (input.Text == string.Empty)
            {
                input.Text = getPlaceholder(input.Name);
            }
        }

        private void initDownload(object sender, RoutedEventArgs e)
        {
            foreach (URLEntry url in URLEntries)
            {
                //MessageBox.Show(entry.entry);
                ProcessStartInfo process = new ProcessStartInfo("youtube-dl");
                process.Arguments = (
                    $"\"{url.entry}\"" +
                    $" -o \"{FolderPath}/%(title)s.%(ext)s\"" +
                    $" -x --audio-format mp3" +
                    $" {(IsPlaylist ? "--yes-playlist" : "--no-playlist")}"
                );

                using (Process downloadProcess = Process.Start(process)) // TODO add safety checks for inputs
                {
                    //downloadProcess.WaitForExit();
                }
            }
            //process.CreateNoWindow = true;

            
        }
      
        private void selectFolder(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog folderDialog = new();
            folderDialog.IsFolderPicker = true;

            if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                FolderPath = folderDialog.FileName;
            }

            if (FolderPath == string.Empty)
            {
                FolderPathInput.Text = Placeholders.FolderPathInput;
                return;
            }

            FolderPathInput.Text = FolderPath;
        }

        private void updateFolder(object sender, RoutedEventArgs e)
        {
            TextBox input = (TextBox) sender;
            FolderPath = input.Text;
        }

        private void togglePlaylist(object sender, RoutedEventArgs e)
        {
            Button button = (Button) sender;
            IsPlaylist = !IsPlaylist;

            button.Content = IsPlaylist ? "Playlist" : "Single";
        }
    }
}
