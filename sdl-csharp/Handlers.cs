using System.Windows;
using System.Windows.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using sdl_csharp;

namespace sdl
{
    public partial class SDLWindow : Window
    {
        private static string GetPlaceholder(string inputName) // This is how we get the value dynamically, based on the name of the control element
        {
            return (string) Placeholders.GetType().GetProperty(inputName).GetValue(Placeholders);
        }

        private static string GetHash(object sender, RoutedEventArgs e)
        {
            return sender.GetHashCode().ToString();
        }

        private static void SpinnerAnimation(URLEntry entry)
        {
            //MessageBox.Show("Hey, yo");
        }

        private void RemoveEntry(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            URLEntry entry = (URLEntry) button.DataContext;
            URLEntries.Remove(entry);
        }

        private void AddURL(object sender, RoutedEventArgs e)
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

        private void ClearInputPlaceholder(object sender, RoutedEventArgs e)
        {
            TextBox input = (TextBox) sender;

            if (input.Text == GetPlaceholder(input.Name))
            {
                input.Clear();
            }
        }

        private void ResetInputPlaceholder(object sender, RoutedEventArgs e)
        {
            TextBox input = (TextBox) sender;

            if (input.Text == string.Empty)
            {
                input.Text = GetPlaceholder(input.Name);
            }
        }

        private void InitIndividualDownload(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            URLEntry entry = (URLEntry) button.DataContext;
            Download download = new();
            download.BeginDownload(entry, FolderPath, false);
        }

        private void InitDownload(object sender, RoutedEventArgs e)
        {
            foreach (URLEntry url in URLEntries) // For every URL, start a new thread with a download process
            {
                Download download = new();
                download.BeginDownload(url, FolderPath, IsPlaylist);
            }
        }

        private void SelectFolder(object sender, RoutedEventArgs e)
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

        private void UpdateFolder(object sender, RoutedEventArgs e)
        {
            TextBox input = (TextBox) sender;
            FolderPath = input.Text;
        }

        private void TogglePlaylist(object sender, RoutedEventArgs e)
        {
            Button button = (Button) sender;
            IsPlaylist = !IsPlaylist;

            button.Content = IsPlaylist ? "Playlist" : "Single";
        }
    }
}
