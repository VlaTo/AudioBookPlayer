using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Extensions;
using AudioBookPlayer.App.Services;
using LibraProgramming.Media.Common;
using LibraProgramming.Xamarin.Dependency.Container.Attributes;
using LibraProgramming.Xamarin.Interaction.Contracts;
using Xamarin.Forms;

namespace AudioBookPlayer.App.ViewModels
{
    [QueryProperty(nameof(BookId), nameof(BookId))]
    public class PlayerControlViewModel : ViewModelBase, IInitialize
    {
        private readonly IBookShelfProvider provider;
        private readonly TaskExecutionMonitor executionMonitor;
        private string bookId;
        private string bookTitle;
        private string bookSubtitle;
        private double chapterStart;
        private double chapterEnd;
        private double chapterPosition;
        private TimeSpan elapsed;
        private TimeSpan left;
        private TimeSpan duration;
        private ImageSource imageSource;
        private bool isPlaying;

        public string BookId
        {
            get => bookId;
            set
            {
                if (SetProperty(ref bookId, value))
                {
                    executionMonitor.Start();
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

        public TimeSpan Duration
        {
            get => duration;
            set => SetProperty(ref duration, value);
        }
        
        public double ChapterPosition
        {
            get => chapterPosition;
            set
            {
                if (SetProperty(ref chapterPosition, value))
                {
                    elapsed = TimeSpan.FromMilliseconds(value);
                    left = elapsed - duration;

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

        public bool IsPlaying
        {
            get => isPlaying;
            set => SetProperty(ref isPlaying, value);
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

        [PrefferedConstructor]
        public PlayerControlViewModel(IBookShelfProvider provider)
        {
            this.provider = provider;
            executionMonitor = new TaskExecutionMonitor(DoLoadBookAsync);

            SmallRewind = new Command(DoSmallRewindCommand);
            Play = new Command(DoPlayCommand);
            SmallFastForward = new Command(DoSmallFastForwardCommand);
            ChangeCover = new Command(DoChangeCoverCommand);
            BookmarkCurrent = new Command(DoBookmarkCurrentCommand);
            Snooze = new Command(DoSnoozeCommand);

            ChapterStart = 0.0d;
            ChapterEnd = 1.0d;
            ChapterPosition = 0.0d;

            //ChapterEnd = 32755299.0d;
            //ChapterPosition = 110.0d;

            Duration = TimeSpan.Zero; // TimeSpan.FromMilliseconds(ChapterEnd);
            Elapsed = TimeSpan.Zero;
            Left = TimeSpan.Zero;
        }

        public void OnInitialize()
        {
            Debug.WriteLine($"[PlayerControlViewModel] [OnInitialize] Execute");
        }

        private void DoSmallRewindCommand()
        {
            Debug.WriteLine($"[PlayerControlViewModel] [DoSmallRewindCommand] Execute");
        }

        private void DoPlayCommand()
        {
            IsPlaying = false == IsPlaying;
            Debug.WriteLine($"[PlayerControlViewModel] [DoPlayCommand] Playing: {IsPlaying}");
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
            string JoinAuthors(IEnumerable<string> authors)
            {
                var sep = CultureInfo.CurrentUICulture.TextInfo.ListSeparator;
                return String.Join(sep, authors);
            }

            if (String.IsNullOrEmpty(BookId))
            {
                return;
            }

            var id = long.Parse(BookId, CultureInfo.InvariantCulture);
            var book = await provider.GetBookAsync(id);

            if (null != book)
            {
                BookTitle = book.Title;
                BookSubtitle = JoinAuthors(book.Authors);
                
                Duration = book.Duration;
                Elapsed = TimeSpan.Zero;
                Left = -book.Duration;
                
                ChapterStart = 0.0d;
                ChapterEnd = book.Duration.TotalMilliseconds;
                ChapterPosition = 0.0d;

                ImageSource = await book.GetImageAsync(WellKnownMetaItemNames.Cover);
            }
        }
    }
}