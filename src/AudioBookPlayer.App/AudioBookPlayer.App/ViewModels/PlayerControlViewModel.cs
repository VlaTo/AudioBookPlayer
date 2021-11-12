using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Domain.Services;
using AudioBookPlayer.App.Services;
using AudioBookPlayer.App.ViewModels.RequestContexts;
using LibraProgramming.Xamarin.Dependency.Container.Attributes;
using LibraProgramming.Xamarin.Interaction;
using LibraProgramming.Xamarin.Interaction.Contracts;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AudioBookPlayer.App.Models;
using Xamarin.Forms;

namespace AudioBookPlayer.App.ViewModels
{
    [QueryProperty(nameof(MediaId), "mid")]
    public class PlayerControlViewModel : ViewModelBase, IInitialize, ICleanup
    {
        private readonly IMediaBrowserServiceConnector connector;
        private readonly ICoverService coverService;
        private readonly TaskExecutionMonitor loadBookMonitor;

        private string mediaId;
        private string title;
        private string subtitle;
        private string description;
        private string queueItemTitle;
        private int queueItemIndex;
        private long position;
        private long duration;
        private long left;
        private TimeSpan bookProgress;
        private Func<CancellationToken, Task<Stream>> imageSource;
        private bool isPlaying;
        private bool isPlaybackEnabled;
        private long activeQueueItemId;
        private bool wasPlaying;
        private bool seeking;

        private bool isExpanding;
        private bool isVisible;
        private double expandingPercentage;
        private ChapterViewModel selectedChapter;
        private bool isInitializing;

        #region Properties

        public string MediaId
        {
            get => mediaId;
            set
            {
                if (SetProperty(ref mediaId, value))
                {
                    loadBookMonitor.Start();
                }
            }
        }

        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        public string Subtitle
        {
            get => subtitle;
            set => SetProperty(ref subtitle, value);
        }

        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }

        public Func<CancellationToken, Task<Stream>> ImageSource
        {
            get => imageSource;
            set => SetProperty(ref imageSource, value);
        }

        public long ActiveQueueItemId
        {
            get => activeQueueItemId;
            set
            {
                if (SetProperty(ref activeQueueItemId, value))
                {
                    for (var index = 0; index < connector.Chapters.Count; index++)
                    {
                        var chapter = connector.Chapters[index];

                        if (chapter.QueueId == value)
                        {
                            QueueItemIndex = index;
                            QueueItemTitle = chapter.Title;

                            return;
                        }
                    }

                    QueueItemIndex = -1;
                    QueueItemTitle = String.Empty;

                    for (var index = 0; index < Sections.Count; index++)
                    {
                        var viewModel = Sections[index];

                        if (viewModel is SectionViewModel section)
                        {
                            for (var entryIndex = 0; entryIndex < section.Entries.Count; entryIndex++)
                            {
                                var entry = section.Entries[entryIndex];

                                if (entry.QueueId == activeQueueItemId)
                                {
                                    SelectedChapter = entry;
                                    break;
                                }
                            }
                        }
                        else
                        if (viewModel is ChapterViewModel chapter && chapter.QueueId == activeQueueItemId)
                        {
                            SelectedChapter = chapter;
                            break;
                        }
                    }
                }
            }
        }

        public long Position
        {
            get => position;
            set
            {
                if (SetProperty(ref position, value))
                {
                    Left = duration - position;
                }
            }
        }

        public long Duration
        {
            get => duration;
            set
            {
                if (SetProperty(ref duration, value))
                {
                    Left = duration - position;
                }
            }
        }

        public int QueueItemIndex
        {
            get => queueItemIndex;
            set
            {
                if (SetProperty(ref queueItemIndex, value))
                {
                    if (-1 < value && value < connector.Chapters.Count)
                    {
                        var chapter = connector.Chapters[value];
                        ActiveQueueItemId = chapter.QueueId;
                    }
                }
            }
        }

        public string QueueItemTitle
        {
            get => queueItemTitle;
            set => SetProperty(ref queueItemTitle, value);
        }

        public long Left
        {
            get => left;
            set => SetProperty(ref left, value);
        }

        public bool IsPlaybackEnabled
        {
            get => isPlaybackEnabled;
            set => SetProperty(ref isPlaybackEnabled, value);
        }

        public bool IsPlaying
        {
            get => isPlaying;
            set => SetProperty(ref isPlaying, value);
        }

        public ObservableCollection<IChapterViewModel> Sections
        {
            get;
        }

        public ChapterViewModel SelectedChapter
        {
            get => selectedChapter;
            set
            {
                if (SetProperty(ref selectedChapter, value))
                {
                    if (isInitializing)
                    {
                        return;
                    }

                    if (null != selectedChapter)
                    {
                        connector.SetActiveQueueItemId(value.QueueId);
                    }

                    DoCloseDrawer();
                }
            }
        }

        public Command PickChapter
        {
            get;
        }

        public Command SmallRewind
        {
            get;
        }

        public Command Play
        {
            get;
        }

        public Command SmallFastForward
        {
            get;
        }

        public Command ChangeCover
        {
            get;
        }

        public Command BookmarkCurrent
        {
            get;
        }

        public Command Snooze
        {
            get;
        }

        public Command PreviousChapter
        {
            get;
        }

        public Command NextChapter
        {
            get;
        }

        public Command SeekStart
        {
            get;
        }

        public Command SeekComplete
        {
            get;
        }

        public Command CloseDrawer
        {
            get;
        }

        public InteractionRequest<PickChapterRequestContext> PickChapterRequest
        {
            get;
        }

        public InteractionRequest<BookmarkRequestContext> BookmarkRequest
        {
            get;
        }

        #endregion

        [PrefferedConstructor]
        // ReSharper disable once UnusedMember.Global
        public PlayerControlViewModel(IMediaBrowserServiceConnector connector, ICoverService coverService)
        {
            this.connector = connector;
            this.coverService = coverService;

            Sections = new ObservableCollection<IChapterViewModel>();
            loadBookMonitor = new TaskExecutionMonitor(DoLoadBookAsync);
            PickChapter = new Command(DoPickChapter);
            SmallRewind = new Command(DoSmallRewindCommand);
            Play = new Command(DoPlayCommand);
            SmallFastForward = new Command(DoSmallFastForwardCommand);
            ChangeCover = new Command(DoChangeCoverCommand);
            BookmarkCurrent = new Command(DoBookmarkCurrentCommand);
            Snooze = new Command(DoSnoozeCommand);
            PreviousChapter = new Command(DoPreviousChapter);
            NextChapter = new Command(DoNextChapter);
            SeekStart = new Command(DoSeekStart);
            SeekComplete = new Command(DoSeekComplete);
            CloseDrawer = new Command(DoCloseDrawer);
            PickChapterRequest = new InteractionRequest<PickChapterRequestContext>();
            BookmarkRequest = new InteractionRequest<BookmarkRequestContext>();

            wasPlaying = false;
            seeking = false;
            ActiveQueueItemId = -1;
            QueueItemIndex = -1;
            Title = String.Empty;
            Subtitle = String.Empty;
            Description=String.Empty;
            QueueItemTitle = String.Empty;
            ImageSource = null;
            //Duration = 1L;
            //Position = 0L;
            IsPlaybackEnabled = true;

            connector.StateChanged += AudioBookBrowserConnectorStateChanged;
            connector.AudioBookMetadataChanged += AudioBookBrowserConnectorAudioBookMetadataChanged;
            connector.ChaptersChanged += AudioBookBrowserConnectorChaptersChanged;
        }

        public void OnInitialize()
        {
            IsPlaybackEnabled = connector.IsConnected;
            
            UpdateProperties();
            UpdatePlaybackState();
        }

        public void OnCleanup()
        {
            connector.StateChanged -= AudioBookBrowserConnectorStateChanged;
            connector.AudioBookMetadataChanged -= AudioBookBrowserConnectorAudioBookMetadataChanged;
            connector.ChaptersChanged -= AudioBookBrowserConnectorChaptersChanged;
        }

        private void DoPickChapter()
        {
            var context = new PickChapterRequestContext(-1);

            PickChapterRequest.Raise(context,
                () =>
                {
                    Debug.WriteLine($"[PlayerControlViewModel] [DoPickChapter] Callback");
                }
            );
        }

        private void DoCloseDrawer()
        {
            // IsVisible = false;
        }

        private void DoPlayCommand()
        {
            if (IsPlaying)
            {
                connector.Pause();
            }
            else
            {
                connector.Play();
            }
        }

        private void DoSmallRewindCommand()
        {
            connector.Rewind();
        }

        private void DoSmallFastForwardCommand()
        {
            connector.FastForward();
        }

        private void DoChangeCoverCommand()
        {
            Debug.WriteLine($"[PlayerControlViewModel] [DoChangeCoverCommand] Execute");
        }

        private void DoBookmarkCurrentCommand()
        {
            /*var bookmark = new AudioBookPosition(
                playbackService.AudioBook.Id.Value,
                ChapterIndex,
                TimeSpan.FromMilliseconds(ChapterPosition)
            );
            var context = new BookmarkRequestContext(bookmark);

            BookmarkRequest.Raise(context, () =>
            {
                Debug.WriteLine($"[PlayerControlViewModel] [DoBookmarkCurrentCommand] Callback");
            });*/
        }

        private void DoSnoozeCommand()
        {
            var position = TimeSpan.FromMilliseconds(Position);
            Debug.WriteLine($"[PlayerControlViewModel] [DoSnoozeCommand] Current: {position:g}");
        }

        private void DoPreviousChapter()
        {
            connector.SkipToPrevious();
        }

        private void DoNextChapter()
        {
            connector.SkipToNext();
        }

        private void DoSeekStart()
        {
            seeking = true;

            if (PlaybackState.Playing == connector.State)
            {
                wasPlaying = true;
                connector.Pause();
            }
        }

        private void DoSeekComplete()
        {
            if (seeking)
            {
                connector.SeekTo(Position);

                if (wasPlaying)
                {
                    connector.Play();
                    wasPlaying = false;
                }

                seeking = false;
            }
        }

        private Task DoLoadBookAsync()
        {
            if (Domain.Models.MediaId.TryParse(MediaId, out var mid) && null != connector)
            {
                connector.PrepareFromMediaId(mid);
            }

            UpdatePlaybackState();

            return Task.CompletedTask;
        }

        private void UpdateProperties()
        {
            var metadata = connector.AudioBookMetadata;

            if (null == metadata)
            {
                return;
            }

            Title = metadata.Title;
            Subtitle = metadata.Subtitle;
            Description = metadata.Description;

            var imageUri = metadata.AlbumArtUri;

            if (null != imageUri)
            {
                ImageSource = cancellationToken => coverService.GetImageAsync(imageUri, cancellationToken);
            }
        }

        private void UpdateChaptersList()
        {
            try
            {
                isInitializing = true;

                Sections.Clear();

                SectionViewModel GetOrCreateSection(ISectionMetadata sectionMetadata)
                {
                    for (var index = 0; index < Sections.Count; index++)
                    {
                        if (Sections[index] is SectionViewModel svm)
                        {
                            if (svm.Index == sectionMetadata.Index)
                            {
                                return svm;
                            }
                        }
                    }

                    var section = new SectionViewModel
                    {
                        Title = sectionMetadata.Name,
                        Index = sectionMetadata.Index
                    };

                    Sections.Add(section);

                    return section;
                }

                for (var chapterIndex = 0; chapterIndex < connector.Chapters.Count; chapterIndex++)
                {
                    var chapter = connector.Chapters[chapterIndex];
                    var sectionViewModel = GetOrCreateSection(chapter.Section);
                    var chapterViewModel = new ChapterViewModel(chapterIndex, chapter.QueueId)
                    {
                        Title = chapter.Title
                    };

                    sectionViewModel.Entries.Add(chapterViewModel);

                    if (connector.ActiveQueueItemId == chapter.QueueId)
                    {
                        selectedChapter = chapterViewModel;
                    }
                }
            }
            finally
            {
                isInitializing = false;
            }
        }

        private void UpdatePlaybackState()
        {
            IsPlaying = PlaybackState.Playing == connector.State;
            ActiveQueueItemId = connector.ActiveQueueItemId;
            Duration = Math.Max(1L, connector.Duration);
            Position = connector.Position;
        }

        private void AudioBookBrowserConnectorStateChanged(object sender, EventArgs _)
        {
            IsPlaybackEnabled = connector.IsConnected;
            UpdatePlaybackState();
        }

        private void AudioBookBrowserConnectorAudioBookMetadataChanged(object sender, EventArgs _)
        {
            UpdateProperties();
        }

        private void AudioBookBrowserConnectorChaptersChanged(object sender, EventArgs _)
        {
            UpdateChaptersList();
        }
   }
}