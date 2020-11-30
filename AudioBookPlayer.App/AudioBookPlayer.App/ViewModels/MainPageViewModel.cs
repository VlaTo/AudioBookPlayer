using AudioBookPlayer.App.Core.Services;
using LibraProgramming.Media.Common;
using LibraProgramming.Media.QuickTime;
using Prism.Commands;
using Prism.Navigation;
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
        private readonly IPlaybackService playbackService;
        private ImageSource imageSource;
        private string bookTitle;
        private string bookSubtitle;

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
            IPlaybackService playbackService)
            : base(navigationService)
        {
            this.streamProvider = streamProvider;
            this.playbackService = playbackService;

            Title = "Main Page";
            Play = new DelegateCommand(DoPlayCommand);
            ChangeCover = new DelegateCommand(DoChangeCoverCommand);
            //ImageSource = new ImageSource();
        }

        private async void DoPlayCommand()
        {
            //var image = await FileSystem.OpenAppPackageFileAsync("sample-200.png");

            //ImageSource = ImageSource.FromStream(() => image);
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

            using (var stream = File.OpenRead("/storage/emulated/0/Download/book.m4b"))
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
            }
        }

        private void DoChangeCoverCommand()
        {
            Debug.WriteLine("[MainPageViewModel] [DoChangeCoverCommand]");
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
    }
}
