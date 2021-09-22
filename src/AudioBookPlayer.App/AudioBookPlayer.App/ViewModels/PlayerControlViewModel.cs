using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Domain.Services;
using AudioBookPlayer.App.Services;
using AudioBookPlayer.App.ViewModels.RequestContexts;
using LibraProgramming.Xamarin.Dependency.Container.Attributes;
using LibraProgramming.Xamarin.Interaction;
using LibraProgramming.Xamarin.Interaction.Contracts;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AudioBookPlayer.App.ViewModels
{
    [QueryProperty(nameof(BookId), nameof(BookId))]
    public class PlayerControlViewModel : ViewModelBase, IInitialize
    {
        private readonly IBooksService booksService;
        private readonly IMediaBrowserServiceConnector connector;
        private readonly IPlaybackService playbackService;
        private readonly IActivityTrackerService activityTrackerService;
        // private readonly INotificationService notificationService;
        private readonly TaskExecutionMonitor serviceConnectMonitor;
        private readonly TaskExecutionMonitor loadBookMonitor;
        private readonly TaskExecutionMonitor trackBookActivity;

        private string bookId;
        private string bookTitle;
        private string bookSubtitle;
        private string currentChapterTitle;
        private int chapterIndex;
        private double chapterStart;
        private double chapterEnd;
        private double chapterPosition;
        private TimeSpan elapsed;
        private TimeSpan left;
        private TimeSpan bookDuration;
        private TimeSpan bookProgress;
        private TimeSpan chapterDuration;
        private Func<CancellationToken, Task<Stream>> imageSource;
        private bool canPlay;
        private bool isPlaying;

        public string BookId
        {
            get => bookId;
            set
            {
                if (SetProperty(ref bookId, value))
                {
                    loadBookMonitor.Start();
                }
            }
        }

        public string BookTitle
        {
            get => bookTitle;
            set => SetProperty(ref bookTitle, value);
        }

        public string BookSubtitle
        {
            get => bookSubtitle;
            set => SetProperty(ref bookSubtitle, value);
        }

        public Func<CancellationToken, Task<Stream>> ImageSource
        {
            get => imageSource;
            set => SetProperty(ref imageSource, value);
        }

        public double ChapterEnd
        {
            get => chapterEnd;
            set
            {
                if (SetProperty(ref chapterEnd, value))
                {
                    ;
                }
            }
        }

        public double ChapterPosition
        {
            get => chapterPosition;
            set
            {
                if (SetProperty(ref chapterPosition, value))
                {
                    elapsed = TimeSpan.FromMilliseconds(value);
                    left = elapsed - chapterDuration;

                    OnPropertyChanged(nameof(Elapsed));
                    OnPropertyChanged(nameof(Left));
                }
            }
        }

        public TimeSpan BookDuration
        {
            get => bookDuration;
            set => SetProperty(ref bookDuration, value);
        }

        public TimeSpan BookPosition
        {
            get => bookProgress;
            set => SetProperty(ref bookProgress, value);
        }

        public int ChapterIndex
        {
            get => chapterIndex;
            set => SetProperty(ref chapterIndex, value);
        }

        public string CurrentChapterTitle
        {
            get => currentChapterTitle;
            set => SetProperty(ref currentChapterTitle, value);
        }

        public TimeSpan Elapsed
        {
            get => elapsed;
            set => SetProperty(ref elapsed, value);
        }

        public TimeSpan Left
        {
            get => left;
            set => SetProperty(ref left, value);
        }

        public bool CanPlay
        {
            get => canPlay;
            set => SetProperty(ref canPlay, value);
        }

        public bool IsPlaying
        {
            get => isPlaying;
            set => SetProperty(ref isPlaying, value);
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

        public InteractionRequest<PickChapterRequestContext> PickChapterRequest
        {
            get;
        }

        public InteractionRequest<BookmarkRequestContext> BookmarkRequest
        {
            get;
        }

        [PrefferedConstructor]
        // ReSharper disable once UnusedMember.Global
        public PlayerControlViewModel(
            IBooksService booksService,
            IMediaBrowserServiceConnector connector,
            IActivityTrackerService activityTrackerService)
        {
            this.booksService = booksService;
            this.activityTrackerService = activityTrackerService;
            this.connector = connector;

            serviceConnectMonitor = new TaskExecutionMonitor(DoServiceConnectAsync);
            loadBookMonitor = new TaskExecutionMonitor(DoLoadBookAsync);
            trackBookActivity = new TaskExecutionMonitor(DoTrackBookActivityAsync);

            PickChapter = new Command(DoPickChapter);
            SmallRewind = new Command(DoSmallRewindCommand);
            Play = new Command(DoPlayCommand);
            SmallFastForward = new Command(DoSmallFastForwardCommand);
            ChangeCover = new Command(DoChangeCoverCommand);
            BookmarkCurrent = new Command(DoBookmarkCurrentCommand);
            Snooze = new Command(DoSnoozeCommand);
            PreviousChapter = new Command(DoPreviousChapter);
            NextChapter = new Command(DoNextChapter);
            PickChapterRequest = new InteractionRequest<PickChapterRequestContext>();
            BookmarkRequest = new InteractionRequest<BookmarkRequestContext>();

            chapterStart = 0.0d;
            chapterDuration = TimeSpan.Zero;

            ChapterEnd = 0.1d;
            ChapterPosition = 0.0d;
            ChapterIndex = -1;
            CurrentChapterTitle = String.Empty;

            BookDuration = TimeSpan.Zero;
            BookPosition = TimeSpan.Zero;
            Elapsed = TimeSpan.Zero;
            Left = TimeSpan.Zero;

            // playbackService.IsPlayingChanged += OnPlaybackControllerIsPlayingChanged;
            // playbackService.AudioBookChanged += OnPlaybackControllerAudioBookChanged;
            // playbackService.ChapterIndexChanged += OnPlaybackControllerChapterIndexChanged;
            // playbackService.CurrentPositionChanged += OnPlaybackControllerPositionChanged;
        }

        public void OnInitialize()
        {
            serviceConnectMonitor.Start();
        }

        private void DoPickChapter()
        {
            /*if (null != playbackService.AudioBook && playbackService.AudioBook.Id.HasValue)
            {
                var context = new PickChapterRequestContext(playbackService.AudioBook.Id.Value);

                PickChapterRequest.Raise(context, () =>
                {
                    Debug.WriteLine($"[PlayerControlViewModel] [DoPickChapter] Callback");
                });
            }*/
        }

        private void DoSmallRewindCommand()
        {
            Debug.WriteLine($"[PlayerControlViewModel] [DoSmallRewindCommand] Execute");
        }

        private void DoPlayCommand()
        {
            if (IsPlaying)
            {
                playbackService.Pause();
            }
            else
            {
                playbackService.Play();
            }
        }

        private void DoSmallFastForwardCommand()
        {
            Debug.WriteLine($"[PlayerControlViewModel] [DoSmallFastForwardCommand] Execute");
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
            var position = TimeSpan.FromMilliseconds(ChapterPosition);
            Debug.WriteLine($"[PlayerControlViewModel] [DoSnoozeCommand] Current: {position:g}");
        }

        private void DoPreviousChapter()
        {
            var index = chapterIndex - 1;

            if (0 > index)
            {
                return;
            }

            playbackService.ChapterIndex = index;
        }

        private void DoNextChapter()
        {
            var index = chapterIndex + 1;

            if (playbackService.AudioBook.Chapters.Count <= index)
            {
                return;
            }

            playbackService.ChapterIndex = index;
        }

        private Task DoServiceConnectAsync()
        {
            //mediaBrowserServiceConnector.Connect();
            return Task.CompletedTask;
        }

        private Task DoLoadBookAsync()
        {
            if (Domain.Models.BookId.TryParse(BookId, out var id))
            {
                /*var book = booksService.GetBook(id);

                if (null != book)
                {

                    using (playbackService.BatchUpdate())
                    {
                        if (AudioBook.AreSame(playbackService.AudioBook, book))
                        {
                            UpdateAudioBookProperties();
                        }
                        else
                        {
                            playbackService.AudioBook = book;
                        }
                    }
                }
                else
                {
                    // should we clear book properties?

                }*/
            }

            return Task.CompletedTask;
        }

        private string GetBookAuthors()
        {
            var builder = new StringBuilder();
            var separator = CultureInfo.CurrentUICulture.TextInfo.ListSeparator;
            var authors = playbackService.AudioBook.Authors;

            foreach (var author in authors)
            {
                if (0 < builder.Length)
                {
                    builder.Append(separator);
                }

                builder.Append(author.Name);
            }

            return builder.ToString();
        }

        private Task DoTrackBookActivityAsync()
        {
            /*var position = new AudioBookPosition(
                playbackService.AudioBook.Id.Value,
                ChapterIndex,
                TimeSpan.FromMilliseconds(ChapterPosition)
            );

            return activityTrackerService.TrackActivityAsync(
                IsPlaying ? ActivityType.Play : ActivityType.Pause,
                position
            );*/

            return Task.CompletedTask;
        }

        private void OnPlaybackControllerIsPlayingChanged(object sender, EventArgs e)
        {
            IsPlaying = playbackService.IsPlaying;
            
            if (IsPlaying)
            {
                //notificationService.ShowInformation(playbackService.AudioBook);
            }
            else
            {
                //notificationService.HideInformation();
            }

            trackBookActivity.Start();
        }

        private void OnPlaybackControllerAudioBookChanged(object sender, EventArgs e)
        {
            UpdateAudioBookProperties();
        }

        private void OnPlaybackControllerChapterIndexChanged(object sender, EventArgs e)
        {
            ChapterIndex = playbackService.ChapterIndex;
            UpdateChapterProperties();
        }

        private void OnPlaybackControllerPositionChanged(object sender, EventArgs e)
        {
            var position = playbackService.CurrentPosition;

            if (position > BookPosition)
            {
                BookPosition = position;
            }

            ChapterPosition = position.TotalMilliseconds;
        }

        private void UpdateChapterProperties()
        {
            if (-1 < ChapterIndex)
            {
                var chapter = playbackService.AudioBook.Chapters[ChapterIndex];

                chapterStart = chapter.Start.TotalMilliseconds;
                chapterDuration = chapter.Duration;

                CurrentChapterTitle = chapter.Title;
                ChapterEnd = chapterDuration.TotalMilliseconds;
                Left = -chapter.Duration;
            }
            else
            {
                chapterStart = 0.0d;
                chapterDuration = TimeSpan.Zero;

                CurrentChapterTitle = String.Empty;
                ChapterEnd = 0.1d;
                Left = Elapsed;
            }

            Elapsed = TimeSpan.Zero;
            ChapterPosition = 0.0d;
        }

        private void UpdateAudioBookProperties()
        {
            CanPlay = 0 < playbackService.AudioBook.Chapters.Count;
            BookTitle = playbackService.AudioBook.Title;
            BookSubtitle = GetBookAuthors();

            //BookDuration = playbackService.AudioBook.Duration;
            BookPosition = TimeSpan.Zero;

            ImageSource = 0 < playbackService.AudioBook.Images.Count
                ? (Func<CancellationToken, Task<Stream>>)playbackService.AudioBook.Images[0].GetStreamAsync
                : null;
        }
    }
}