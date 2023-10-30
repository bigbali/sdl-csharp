using sdl_csharp.Model;
using sdl_csharp.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace sdl_csharp.Resource.Control.Entry
{
    /// <summary>
    /// Interaction logic for EntryOutput.xaml
    /// </summary>
    public partial class EntryOutput : UserControl
    {
        public ObservableCollection<string> Lines { get; set; } = new();
        bool isRedownload;

        public EntryOutput()
        {
            InitializeComponent();

            DataContextChanged += ApplyOnProgressChanged;
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(EntryViewModel), typeof(EntryOutput), new PropertyMetadata(null));

        public EntryViewModel Source
        {
            get => Utility.Thread.ThreadSafeAction(() => (EntryViewModel)GetValue(SourceProperty));
            set => Utility.Thread.ThreadSafeAction(() => SetValue(SourceProperty, value));
        }

        void ApplyOnProgressChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is null) return;

            if (e.NewValue is EntryViewModel vm)
            {
                vm.StatusViewModel.PropertyChanged += UpdateWhenDoneOrCancelled;

                if (vm.Data is EntrySingleData)
                {
                    return;
                }

                ((BindingList<PlaylistMember>)vm.entry.data.PlaylistMembers).ListChanged += OnMembersChanged;
            }
        }

        void OnMembersChanged(object sender, ListChangedEventArgs e)
        {
            Update();
        }

        void UpdateWhenDoneOrCancelled(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EntryStatusViewModel.IsDone) || e.PropertyName == nameof(EntryStatusViewModel.IsCancelled))
            {
                Update();
            }
        }

        string GetDownloadSpeed(PlaylistMember member)
        {
            float? value = member.DownloadSpeed;

            if (!value.HasValue) return string.Empty;

            int speed = 0;
            if ((int)value > 0)
            {
                speed = (int)value / 1_000_000;
            }

            return $" ({speed} MB/s)";
        }

        /// <summary>
        /// Update the contents of each line based on <see cref="IEntryPlaylistData.PlaylistMembers"/>.
        /// </summary>
        void UpdateForEach()
        {
            IEntryPlaylistData data = Source.Data;

            int i = 0;
            foreach(PlaylistMember member in data.PlaylistMembers)
            {
                if (i >= data.PlaylistDownloadIndex) continue;

                string status = member.State switch
                {
                    PlaylistMemberState.FETCHING => "fetching",
                    PlaylistMemberState.DOWNLOADING => $"downloading{GetDownloadSpeed(member)}",
                    PlaylistMemberState.CONVERTING => "converting",
                    PlaylistMemberState.DONE => "done",
                    _ => null
                };

                string statusPart = status switch
                {
                    null => "",
                    string part => $" | {part}"
                };

                string state = $"[{i + 1}]: {member.DownloadPercent:0.0}%{statusPart}";

                if (Lines.Count <= i)
                {
                    Lines.Add(state);
                }
                else
                {
                    Lines[i] = state;
                }

                i++;
            }
        }

        bool HandleRedownload()
        {
            if (Source.StatusViewModel.IsCancelled)
            {
                Utility.Thread.ThreadSafeAction(() =>
                {
                    Lines.Clear();
                    Lines.Add("Download cancelled");
                });

                return true;
            }

            if (Source.StatusViewModel.IsInProgress && isRedownload)
            {
                Utility.Thread.ThreadSafeAction(() =>
                {
                    Lines.Clear();
                });

                isRedownload = false;
            }

            if (isRedownload)
            {
                return true;
            }

            return false;
        }

        void Update()
        {
            if (HandleRedownload())
            {
                return;
            }

            IEntryPlaylistData data = Source.Data;

            Utility.Thread.ThreadSafeAction(() =>
            {
                if (!isRedownload && Source.StatusViewModel.IsDone)
                {
                    isRedownload = true;

                    TimeSpan span = DateTime.Now - Source.entry.downloadStart;
                    string elapsed = $"{span:mm\\:ss}";

                    Lines.Add($"Done in {elapsed}");
                }

                UpdateForEach();

                if (Out.Items.Count > 0)
                {
                    Out.ScrollIntoView(Out.Items[^1]);
                }
            });
        }
    }
}
