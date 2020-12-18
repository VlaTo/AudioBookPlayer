using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Interaction;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AudioBookPlayer.App.ViewModels
{
    public sealed class SourceFolderRequestContext : InteractionRequestContext<string>
    {
        public SourceFolderRequestContext(string path)
            : base(path)
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
        private readonly IPermissionRequestor permissions;

        public Command SelectLibraryRootFolder
        {
            get;
        }

        public InteractionRequest<SourceFolderRequestContext> SelectLibraryRootFolderRequest
        {
            get;
        }

        public SettingsViewModel(IPermissionRequestor permissions)
        {
            this.permissions = permissions;

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

            var context = new SourceFolderRequestContext(String.Empty);

            SelectLibraryRootFolderRequest.Raise(context, () => { });
        }
    }
}
