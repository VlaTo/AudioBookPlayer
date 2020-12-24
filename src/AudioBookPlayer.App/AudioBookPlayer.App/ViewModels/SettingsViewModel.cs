using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Interaction;
using LibraProgramming.Xamarin.Interaction.Extensions;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AudioBookPlayer.App.ViewModels
{
    public sealed class SourceFolderRequestContext : InteractionRequestContext
    {
        public string LibraryRootFolder
        {
            get;
            set;
        }

        public SourceFolderRequestContext()
        {
        }

        public Deferral GetDeferral()
        {
            var tcs = new TaskCompletionSource();
            return new Deferral(tcs);
        }
    }

    public sealed class SettingsViewModel : ViewModelBase
    {
        //private readonly InteractionRequest<SourceFolderRequestContext> selectSourceFolderRequest;
        //private readonly Command selectLibraryRootFolder;
        private readonly ApplicationSettings settings;
        private readonly IPermissionRequestor permissions;
        private readonly IMediaService mediaService;

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
            IPermissionRequestor permissions,
            IMediaService mediaService)
        {
            this.settings = settings;
            this.permissions = permissions;
            this.mediaService = mediaService;

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

            var path = settings.LibraryRootPath;

            if (String.IsNullOrEmpty(path))
            {
                path = await mediaService.GetRootFolderAsync();
            }

            var context = new SourceFolderRequestContext
            {
                LibraryRootFolder = path
            };

            await SelectLibraryRootFolderRequest.RaiseAsync(context);
        }
    }
}
