using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Domain.Services;
using AudioBookPlayer.App.Services;
using AudioBookPlayer.App.ViewModels.RequestContexts;
using LibraProgramming.Xamarin.Dependency.Container.Attributes;
using LibraProgramming.Xamarin.Interaction;
using LibraProgramming.Xamarin.Interaction.Contracts;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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
        private bool initializing;

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
                    Debug.WriteLine($"[PlayerControlViewModel] [SetPosition] Value: {value} (Duration: {duration})");
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

        /*public TimeSpan Elapsed
        {
            get => elapsed;
            set => SetProperty(ref elapsed, value);
        }*/

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
        public PlayerControlViewModel(
            IMediaBrowserServiceConnector connector,
            ICoverService coverService)
        {
            this.connector = connector;
            this.coverService = coverService;

            initializing = true;
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
            initializing = false;

            IsPlaybackEnabled = connector.IsConnected;
            
            UpdateProperties();
            UpdatePlaybackState();
            
            Debug.WriteLine($"[PlayerControlViewModel] [OnInitialize] Position: {Position}");
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
            if (Core.MediaId.TryParse(MediaId, out var mid) && null != connector)
            {
                connector.PrepareFromMediaId(mid);
            }

            UpdatePlaybackState();

            return Task.CompletedTask;
        }

        /*private void OnPlaybackControllerIsPlayingChanged(object sender, EventArgs e)
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
        }*/

        /*private void OnPlaybackControllerAudioBookChanged(object sender, EventArgs e)
        {
            UpdateAudioBookProperties();
        }*/

        /*private void OnPlaybackControllerChapterIndexChanged(object sender, EventArgs e)
        {
            ChapterIndex = playbackService.ChapterIndex;
            UpdateChapterProperties();
        }*/

        /*private void OnPlaybackControllerPositionChanged(object sender, EventArgs e)
        {
            var position = playbackService.CurrentPosition;

            if (position > BookPosition)
            {
                BookPosition = position;
            }

            ChapterPosition = position.TotalMilliseconds;
        }*/

        /*private void UpdateChapterProperties()
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
        }*/

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
            // connector.Chapters;
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
            Debug.WriteLine($"[PlayerControlViewModel] [AudioBookBrowserConnectorStateChanged] Position: {Position}");
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