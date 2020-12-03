using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Core.Services;
using LibraProgramming.Media.Common;
using LibraProgramming.Media.QuickTime;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using static Xamarin.Essentials.Permissions;

namespace AudioBookPlayer.App.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private readonly ISourceStreamProvider streamProvider;
        private readonly IPlaybackController playbackControl;
        private readonly IEventAggregator ea;
        private ImageSource imageSource;
        private string bookTitle;
        private string bookSubtitle;
        private double chapterStart;
        private double chapterEnd;
        private double chapterPosition;
        private TimeSpan elapsed;
        private TimeSpan left;
        private TimeSpan duration;

        public ImageSource ImageSource
        {
            get => imageSource;
            private set => SetProperty(ref imageSource, value);
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

        public double ChapterPosition
        {
            get => chapterPosition;
            set
            {
                if (SetProperty(ref chapterPosition, value))
                {
                    elapsed = TimeSpan.FromMilliseconds(value);
                    left = elapsed - duration;

                    RaisePropertyChanged(nameof(Elapsed));
                    RaisePropertyChanged(nameof(Left));
                }
            }
        }

        public TimeSpan Elapsed => elapsed;

        public TimeSpan Left => left;

        public ICommand Play
        {
            get;
        }

        public ICommand ChangeCover
        {
            get;
        }

        public MainPageViewModel(
            INavigationService navigationService,
            ISourceStreamProvider streamProvider,
            IPlaybackController playbackControl,
            IEventAggregator ea)
            : base(navigationService)
        {
            this.streamProvider = streamProvider;
            this.playbackControl = playbackControl;
            this.ea = ea;

            //Title = "Main Page";
            Play = new DelegateCommand(DoPlayCommand);
            ChangeCover = new DelegateCommand(DoChangeCoverCommand);

            ChapterStart = 0.0d;
            ChapterEnd = 32755299.0d;
            ChapterPosition = 110.0d;

            duration = TimeSpan.FromMilliseconds(ChapterEnd);
            elapsed = TimeSpan.Zero;
            left = -duration;
        }

        public override void Initialize(INavigationParameters parameters)
        {
            var positionChanged = ea.GetEvent<PlaybackPositionChanged>();
            var subscription = positionChanged.Subscribe(OnPlaybackPositionChanged);

            /*const string prefix = "AudioBookPlayer.App.Resources.";
            var assembly = typeof(App).Assembly;
            var files = new List<string>();

            foreach(var name in assembly.GetManifestResourceNames())
            {
                if (false == name.StartsWith(prefix))
                {
                    continue;
                }

                var filename = name.Substring(prefix.Length);
                var ext = Path.GetExtension(filename);

                if (String.Equals(ext, ".ttf"))
                {
                    continue;
                }

                Debug.WriteLine($"File: '{filename}'");

                files.Add(filename);
            }

            Filenames = files;
            SelectedFilename = files[0];*/
        }

        private async void DoPlayCommand()
        {
            //[0:] File: 'ff-16b-2c-44100hz.mp3'
            //[0:] File: 'a2002011001-e02.wav'
            //[0:] File: 'ff-16b-2c-44100hz.wav'
            //[0:] File: 'ff-16b-2c-44100hz.aac'
            //[0:] File: 'ff-16b-2c-44100hz.ac3'
            //var temp = await FileSystem.OpenAppPackageFileAsync("ff-16b-2c-44100hz.wav");

            /*var audioInfo = GetAudioInfo();

            if (null == audioInfo)
            {
                return;
            }

            var assembly = typeof(App).Assembly;
            var mediaResourceName = $"AudioBookPlayer.App.Resources.{selectedFilename}";

            using (var audio = assembly.GetManifestResourceStream(mediaResourceName))
            {
                await playbackService.PlayAsync(audio, audioInfo.AudioEncoding, audioInfo.SampleRate, audioInfo.Channels);
            }*/

            var status = await CheckAndRequestPermissionAsync(new Permissions.StorageRead());

            if (PermissionStatus.Denied == status)
            {
                return;
            }

            playbackControl.StartPlay("/storage/emulated/0/Download/book.m4b");
        }

        private async void DoPlayCommand1()
        {
            //var image = await FileSystem.OpenAppPackageFileAsync("sample-200.png");

            var source = await FileSystem.OpenAppPackageFileAsync("ff-16b-2c-44100.wav");

            // 0F1A-3D0D:Audio/Books/book.m4b
            var drives = Directory.GetLogicalDrives();

            foreach(var drive in drives)
            {
                Debug.WriteLine($"Drive: '{drive}'");
            }

            /*var directories = Directory.EnumerateDirectories("/storage/0F1A-3D0D");
            
            foreach (var directory in directories)
            {
                Debug.WriteLine($"Directory: '{directory}'");
            }*/

            // /storage/emulated/0/Download/book.m4b
            // /storage/0F1A-3D0D/Audio/Books/book.m4b
            // content://com.android.providers.downloads.documents/document/1
            // FileSystem.CacheDirectory

            //var path= Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);

            var status = await CheckAndRequestPermissionAsync(new Permissions.StorageRead());

            if (PermissionStatus.Denied == status)
            {
                return;
            }

            var path1 = Path.Combine(streamProvider.GetBookPath(), "book.m4b");

            /*using (var stream = File.OpenRead("/storage/emulated/0/Download/book.m4b"))
            {
                IMediaTrack track = null;

                using (var extractor = QuickTimeMediaExtractor.CreateFrom(stream))
                {
                    // get meta
                    var meta = extractor.GetMeta();

                    foreach (var item in meta.Items)
                    {
                        switch (item)
                        {
                            case MetaInformationStreamItem streamItem:
                            {
                                if (WellKnownMetaItemNames.Cover.Equals(streamItem.Key))
                                {
                                    ImageSource = ImageSource.FromStream(() => streamItem.Stream);
                                }

                                break;
                            }

                            case MetaInformationTextItem textItem:
                            {
                                if (WellKnownMetaItemNames.Title.Equals(textItem.Key))
                                {
                                    BookTitle = textItem.Text;
                                }
                                else if (WellKnownMetaItemNames.Subtitle.Equals(textItem.Key))
                                {
                                    BookSubtitle = textItem.Text;
                                }

                                break;
                            }
                        }
                    }

                    // get tracks
                    var tracks = extractor.GetTracks();

                    track = tracks.First();
                }

                await playbackService.PlayAsync(track.GetMediaStream());
            }*/
        }

        private void DoChangeCoverCommand()
        {
            Debug.WriteLine("[MainPageViewModel] [DoChangeCoverCommand]");
        }

        private void OnPlaybackPositionChanged(double value)
        {
            ChapterPosition = value;
        }

        private static async Task<PermissionStatus> CheckAndRequestPermissionAsync<T>(T permission)
            where T : BasePermission
        {
            var status = await permission.CheckStatusAsync();

            if (PermissionStatus.Granted == status)
            {
                return status;
            }

            if (PermissionStatus.Denied == status && DevicePlatform.iOS == DeviceInfo.Platform)
            {
                return status;
            }

            status = await permission.RequestAsync();

            return status;
        }

        /// <summary>
        /// 
        /// </summary>
        private sealed class SourceFileInfo
        {
            public string AudioEncoding
            {
                get;
                set;
            }

            public int SampleRate
            {
                get;
                set;
            }

            public string Channels
            {
                get;
                set;
            }
        }

        private static readonly KeyValuePair<string, SourceFileInfo>[] SourceFileInfos = new[]
        {
            new KeyValuePair<string, SourceFileInfo>(
                "ff-16b-2c-44100hz.mp3",
                new SourceFileInfo
                {
                    AudioEncoding = "Encoding.Mp3",
                    SampleRate = 44100,
                    Channels = "ChannelOut.Stereo"
                }
            ),
            new KeyValuePair<string, SourceFileInfo>(
                "a2002011001-e02.wav",
                new SourceFileInfo
                {
                    AudioEncoding = "Encoding.Pcm16",
                    SampleRate = 44100,
                    Channels = "ChannelOut.Stereo"
                }
            ),
            new KeyValuePair<string, SourceFileInfo>(
                "ff-16b-2c-44100hz.wav",
                new SourceFileInfo
                {
                    AudioEncoding = "Encoding.Pcm24",
                    SampleRate = 44100,
                    Channels = "ChannelOut.Stereo"
                }
            ),
            new KeyValuePair<string, SourceFileInfo>(
                "ff-16b-2c-44100hz.aac",
                new SourceFileInfo
                {
                    AudioEncoding = "Encoding.Aac",
                    SampleRate = 44100,
                    Channels = "ChannelOut.Stereo"
                }
            ),
            new KeyValuePair<string, SourceFileInfo>(
                "ff-16b-2c-44100hz.ac3",
                new SourceFileInfo
                {
                    AudioEncoding = "Encoding.Ac3",
                    SampleRate = 44100,
                    Channels = "ChannelOut.Stereo"
                }
            )
        };
    }
}
