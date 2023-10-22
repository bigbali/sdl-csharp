using sdl_csharp.Model;
using sdl_csharp.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YoutubeExplode.Playlists;

namespace sdl_csharp.Resource.Control.Entry
{
    /// <summary>
    /// Interaction logic for EntryOutput.xaml
    /// </summary>
    public partial class EntryOutput : UserControl
    {
        public ObservableCollection<string> Lines { get; set; } = new();

        public EntryOutput()
        {
            InitializeComponent();

            DataContextChanged += ApplyOnPercentChanged;
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(EntryViewModel), typeof(EntryOutput), new PropertyMetadata(null));

        public EntryViewModel Source
        {
            get
            {
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    return (EntryViewModel)GetValue(SourceProperty);
                }
                else
                {
                    EntryViewModel result = null;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        result = (EntryViewModel)GetValue(SourceProperty);
                    });
                    return result;
                }
            }
            set
            {
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    SetValue(SourceProperty, value);
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        SetValue(SourceProperty, value);
                    });
                }
            }
        }

        void ApplyOnPercentChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is null) return;

            if (e.NewValue is EntryViewModel vm)
            {
                vm.StatusViewModel.PropertyChanged += UpdateWhenStatusDone;

                if (vm.Data is EntryPlaylistData playlist)
                {
                    playlist.PropertyChanged += OnPercentChanged;
                }

                if (vm.Data is EntryMemberData member)
                {
                    member.PropertyChanged += OnPercentChanged;
                }
            }
        }

        void OnPercentChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EntryPlaylistData.PlaylistOverallDownloadPercent))
            {
                Update();
            }
        }

        delegate void ThreadSafeActionCallback();

        static void ThreadSafeAction(ThreadSafeActionCallback action)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    action();
                });
            }
        }

        void UpdateWhenStatusDone(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDone")
            {
                Update();
            }
        }

        void Update()
        {
            IEntryPlaylistData data = Source.Data;
            int currentIndex = (int)data.PlaylistDownloadIndex;
            int i = Math.Max(currentIndex - 1, 0);

            string state = $"[{currentIndex}]: {data.PlaylistMemberDownloadPercent}%";

            ThreadSafeAction(() =>
            {
                if ((int)data.PlaylistOverallDownloadPercent == 100 && Source.StatusViewModel.IsDone)
                {
                    Lines.Add($"Done: {data.PlaylistTitle}");
                }

                if (Lines.Count <= i)
                {
                    Lines.Add(state);
                }
                else
                {
                    Lines[i] = state;
                }
            });
        }
    }
}
