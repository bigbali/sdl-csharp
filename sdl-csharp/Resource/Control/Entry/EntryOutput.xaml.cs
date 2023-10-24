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

        public EntryOutput()
        {
            InitializeComponent();

            DataContextChanged += ApplyOnPercentChanged;
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(EntryViewModel), typeof(EntryOutput), new PropertyMetadata(null));

        public EntryViewModel Source
        {
            get => Utility.Thread.ThreadSafeAction(() => (EntryViewModel)GetValue(SourceProperty));
            set => Utility.Thread.ThreadSafeAction(() => SetValue(SourceProperty, value));
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
            int currentIndex = (int)Math.Max(data.PlaylistDownloadIndex, 1);
            int i = Math.Max(currentIndex - 1, 0);

            string state = $"[{currentIndex}]: {data.PlaylistMemberDownloadPercent}%";

            Utility.Thread.ThreadSafeAction(() =>
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

                Out.ScrollIntoView(Out.Items[^1]);
            });
        }
    }
}
