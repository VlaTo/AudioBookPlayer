using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Models;
using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Dependency.Container.Attributes;
using LibraProgramming.Xamarin.Interaction.Contracts;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using AudioBookPlayer.App.Domain;
using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Domain.Services;
using AudioBookPlayer.App.ViewModels.RequestContexts;
using LibraProgramming.Xamarin.Interaction;
using Xamarin.Forms;

namespace AudioBookPlayer.App.ViewModels
{
    [QueryProperty(nameof(BookId), nameof(BookId))]
    public class PlayerControlViewModel : ViewModelBase, IInitialize
    {
        private readonly IMediaLibrary mediaLibrary;
        private readonly IPlaybackService playbackService;
        private readonly IRemoteControlService remoteControlService;

        private readonly TaskExecutionMonitor loadBookMonitor;
        //private AudioBook book;
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
        private ImageSource imageSource;
        private bool canPlay;
        private bool isPlaying;

        public string BookId
        {
            get => bookId;
            set
            {
                
                Debug.WriteLine($"[PlayerControlViewModel] Set BookId: {value}");

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

        public ImageSource ImageSource
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
        public PlayerControlViewModel(
            IMediaLibrary mediaLibrary,
            IPlaybackService playbackService,
            IRemoteControlService remoteControlService)
        {
            this.mediaLibrary = mediaLibrary;
            this.playbackService = playbackService;
            this.remoteControlService = remoteControlService;

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

            playbackService.IsPlayingChanged += OnPlaybackControllerIsPlayingChanged;
            playbackService.AudioBookChanged += OnPlaybackControllerAudioBookChanged;
            playbackService.ChapterIndexChanged += OnPlaybackControllerChapterIndexChanged;
            playbackService.CurrentPositionChanged += OnPlaybackControllerPositionChanged;
        }

        public void OnInitialize()
        {
            Debug.WriteLine($"[PlayerControlViewModel] [OnInitialize] Execute");
        }

        private void DoPickChapter()
        {
            if (null != playbackService.AudioBook && playbackService.AudioBook.Id.HasValue)
            {
                var context = new PickChapterRequestContext(playbackService.AudioBook.Id.Value);

                PickChapterRequest.Raise(context, () =>
                {
                    Debug.WriteLine($"[PlayerControlViewModel] [DoPickChapter] Callback");
                });
            }
        }

        private void DoSmallRewindCommand()
        {
            Debug.WriteLine($"[PlayerControlViewModel] [DoSmallRewindCommand] Execute");
        }

        private async void DoPlayCommand()
        {
            if (IsPlaying)
            {
                playbackService.Pause();
            }
            else
            {
                playbackService.Play();
            }

            await mediaLibrary.RecordBookActivityAsync(playbackService.AudioBook.Id.Value, AudioBookActivityType.Playing);
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
            var bookmark = new AudioBookPosition(
                playbackService.AudioBook.Id.Value,
                ChapterIndex,
                TimeSpan.FromMilliseconds(ChapterPosition)
            );
            var context = new BookmarkRequestContext(bookmark);

            BookmarkRequest.Raise(context, () =>
            {
                Debug.WriteLine($"[PlayerControlViewModel] [DoBookmarkCurrentCommand] Callback");
            });
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

        private async Task DoLoadBookAsync()
        {
            if (String.IsNullOrEmpty(BookId))
            {
                return;
            }

            var id = long.Parse(BookId, CultureInfo.InvariantCulture);

            if (playbackService.IsPlaying)
            {
                if (playbackService.AudioBook.Id == id)
                {
                    UpdateAudioBookProperties();
                    ChapterIndex = playbackService.ChapterIndex;
                    UpdateChapterProperties();

                    return;
                }

                playbackService.Pause();
            }
            
            var book = await mediaLibrary.GetBookAsync(id);

            if (null != book)
            {
                using (playbackService.BatchUpdate())
                {
                    var index = 0 < book.Chapters.Count ? 0 : -1;

                    playbackService.AudioBook = book;
                    playbackService.ChapterIndex = index;
                }
            }
            else
            {
                // should we clear book properties?
            }
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

        private void OnPlaybackControllerIsPlayingChanged(object sender, EventArgs e)
        {
            IsPlaying = playbackService.IsPlaying;

            if (IsPlaying)
            {
                remoteControlService.ShowInformation(playbackService.AudioBook);
            }
            else
            {
                remoteControlService.HideInformation();
            }
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
            //var images = await book.GetImageAsync(WellKnownMediaTags.Cover);

            CanPlay = 0 < playbackService.AudioBook.Chapters.Count;
            BookTitle = playbackService.AudioBook.Title;
            BookSubtitle = GetBookAuthors();
            //ImageSource = images;

            BookDuration = playbackService.AudioBook.Duration;
            BookPosition = TimeSpan.Zero;
        }
    }
}