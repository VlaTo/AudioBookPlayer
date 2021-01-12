using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Interaction;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AudioBookPlayer.App.ViewModels
{
    public sealed class SourceFolderRequestContext : InteractionRequestContext
    {
        private readonly TaskCompletionSource<string> tcs;

        public SourceFolderRequestContext()
        {
            tcs = new TaskCompletionSource<string>();
        }

        public Deferral<string> GetDeferral()
        {
            return new Deferral<string>(tcs);
        }

        public Task<string> WaitAsync() => tcs.Task;
    }

    public sealed class SettingsViewModel : ViewModelBase
    {
        //private readonly InteractionRequest<SourceFolderRequestContext> selectSourceFolderRequest;
        //private readonly Command selectLibraryRootFolder;
        private readonly ApplicationSettings settings;
        private readonly IPermissionRequestor permissions;
        //private readonly IMediaService mediaService;

        public string LibraryRootFolder
        {
            get
            {
                return settings.LibraryRootPath;
            }
            set
            {
                settings.LibraryRootPath = value;
                OnPropertyChanged();
            }
        }

        public Command SelectLibraryRootFolder
        {
            get;
        }

        public InteractionRequest<SourceFolderRequestContext> SelectLibraryRootFolderRequest
        {
            get;
        }

        public SettingsViewModel(
            ApplicationSettings settings,
            IPermissionRequestor permissions)
        {
            this.settings = settings;
            this.permissions = permissions;
            //this.mediaService = mediaService;

            SelectLibraryRootFolderRequest = new InteractionRequest<SourceFolderRequestContext>();
            SelectLibraryRootFolder = new Command(OnSelectLibraryRootFolderCommand);
        }

        private async void OnSelectLibraryRootFolderCommand(object obj)
        {
            var status = await permissions.CheckAndRequestMediaPermissionsAsync();

            if (PermissionStatus.Denied == status)
            {
                return;
            }

            var context = new SourceFolderRequestContext();

            SelectLibraryRootFolderRequest.Raise(context);

            try
            {
                var path = await context.WaitAsync();

                if (false == String.IsNullOrEmpty(path))
                {
                    LibraryRootFolder = path;
                    System.Diagnostics.Debug.WriteLine($"[SettingsViewModel] [OnSelectLibraryRootFolderCommand] Path: \"{path}\"");
                }
            }
            catch (TaskCanceledException)
            {
                System.Diagnostics.Debug.WriteLine($"[SettingsViewModel] [OnSelectLibraryRootFolderCommand] Selection canceled");
            }
        }
    }
}
