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
using LibraProgramming.Xamarin.Interaction;
using Xamarin.Forms;

namespace AudioBookPlayer.App.ViewModels
{
    [QueryProperty(nameof(BookId), nameof(BookId))]
    public class PlayerControlViewModel : ViewModelBase, IInitialize
    {
        private readonly IMediaLibrary mediaLibrary;
        private readonly IPlaybackController playbackController;
        private readonly TaskExecutionMonitor loadBookMonitor;
        private AudioBook book;
        // private IPlayback playback;
        private string bookId;
        private string bookTitle;
        private string bookSubtitle;
        private double chapterStart;
        private double chapterEnd;
        private double chapterPosition;
        private TimeSpan elapsed;
        private TimeSpan left;
        private TimeSpan bookDuration;
        private ImageSource imageSource;
        private bool canPlayBook;
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

        public double ChapterStart
        {
            get => chapterStart;
            set => SetProperty(ref chapterStart, value);
        }

        public double ChapterEnd
        {
            get => chapterEnd;
            set => SetProperty(ref chapterEnd, value);
        }

        public TimeSpan BookDuration
        {
            get => bookDuration;
            set => SetProperty(ref bookDuration, value);
        }
        
        public double ChapterPosition
        {
            get => chapterPosition;
            set
            {
                if (SetProperty(ref chapterPosition, value))
                {
                    elapsed = TimeSpan.FromMilliseconds(value);
                    left = elapsed - bookDuration;

                    OnPropertyChanged(nameof(Elapsed));
                    OnPropertyChanged(nameof(Left));
                }
            }
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

        public bool CanPlayBook
        {
            get => canPlayBook;
            set => SetProperty(ref canPlayBook, value);
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

        public InteractionRequest<PickChapterRequestContext> PickChapterRequest
        {
            get;
        }

        [PrefferedConstructor]
        public PlayerControlViewModel(
            IMediaLibrary mediaLibrary,
            IPlaybackController playbackController)
        {
            this.mediaLibrary = mediaLibrary;
            this.playbackController = playbackController;

            // playback = null;
            loadBookMonitor = new TaskExecutionMonitor(DoLoadBookAsync);

            PickChapter = new Command(DoPickChapter);
            SmallRewind = new Command(DoSmallRewindCommand);
            Play = new Command(DoPlayCommand);
            SmallFastForward = new Command(DoSmallFastForwardCommand);
            ChangeCover = new Command(DoChangeCoverCommand);
            BookmarkCurrent = new Command(DoBookmarkCurrentCommand);
            Snooze = new Command(DoSnoozeCommand);
            PickChapterRequest = new InteractionRequest<PickChapterRequestContext>();

            ChapterStart = 0.0d;
            ChapterEnd = 0.1d;
            ChapterPosition = 0.0d;

            BookDuration = TimeSpan.Zero;
            Elapsed = TimeSpan.Zero;
            Left = TimeSpan.Zero;

            playbackController.IsPlayingChanged += OnPlaybackControllerIsPlayingChanged;
            playbackController.AudioBookChanged += OnPlaybackControllerAudioBookChanged;
            playbackController.CurrentChapterChanged += OnPlaybackControllerCurrentChapterChanged;
            playbackController.CurrentPositionChanged += OnPlaybackControllerCurrentPositionChanged;
        }

        public void OnInitialize()
        {
            Debug.WriteLine($"[PlayerControlViewModel] [OnInitialize] Execute");
        }

        private void DoPickChapter()
        {
            var context = new PickChapterRequestContext(book.Id.Value);

            PickChapterRequest.Raise(context, () =>
            {
                Debug.WriteLine($"[PlayerControlViewModel] [DoPickChapter] Callback");
            });
        }

        private void DoSmallRewindCommand()
        {
            Debug.WriteLine($"[PlayerControlViewModel] [DoSmallRewindCommand] Execute");
        }

        private void DoPlayCommand()
        {
            Debug.WriteLine($"[PlayerControlViewModel] [DoPlayCommand] Playing: {IsPlaying}");

            if (IsPlaying)
            {
                playbackController.Stop();
            }
            else
            {
                playbackController.Play(TimeSpan.Zero);
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
            var position = TimeSpan.FromMilliseconds(ChapterPosition);
            Debug.WriteLine($"[PlayerControlViewModel] [DoBookmarkCurrent] Current: {position:g}");
        }

        private void DoSnoozeCommand()
        {
            var position = TimeSpan.FromMilliseconds(ChapterPosition);
            Debug.WriteLine($"[PlayerControlViewModel] [DoSnoozeCommand] Current: {position:g}");
        }

        private async Task DoLoadBookAsync()
        {
            if (String.IsNullOrEmpty(BookId))
            {
                return;
            }

            var id = long.Parse(BookId, CultureInfo.InvariantCulture);
            
            book = await mediaLibrary.GetBookAsync(id);

            if (null != book)
            {
                BindBookProperties();
                playbackController.AudioBook = book;
            }
            else
            {
                // should we clear book properties?
            }
        }

        private void BindBookProperties()
        {
            var authors = GetBookAuthors();
            //var images = await book.GetImageAsync(WellKnownMediaTags.Cover);
            var canPlay = 0 < book.Chapters.Count;

            BookTitle = book.Title;
            BookSubtitle = authors;
            //ImageSource = images;

            BookDuration = book.Duration;
            Elapsed = TimeSpan.Zero;
            Left = -book.Duration;

            ChapterStart = 0.0d;
            ChapterEnd = book.Duration.TotalMilliseconds;
            ChapterPosition = 0.0d;

            CanPlayBook = canPlay;
        }

        private string GetBookAuthors()
        {
            var builder = new StringBuilder();
            var separator = CultureInfo.CurrentUICulture.TextInfo.ListSeparator;

            for (var index = 0; index < book.Authors.Count; index++)
            {
                if (0 < builder.Length)
                {
                    builder.Append(separator);
                }

                builder.Append(book.Authors[index].Name);
            }

            return builder.ToString();
        }

        private void OnPlaybackControllerIsPlayingChanged(object sender, EventArgs e)
        {
            IsPlaying = playbackController.IsPlaying;
        }

        private void OnPlaybackControllerAudioBookChanged(object sender, EventArgs e)
        {
            ;
        }

        private void OnPlaybackControllerCurrentChapterChanged(object sender, EventArgs e)
        {
            ;
        }

        private void OnPlaybackControllerCurrentPositionChanged(object sender, EventArgs e)
        {
            var position = playbackController.CurrentPosition;
            ChapterPosition = position.TotalMilliseconds;
        }
    }
}