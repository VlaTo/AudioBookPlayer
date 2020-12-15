using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Dependency.Container.Attributes;
using LibraProgramming.Xamarin.Interaction;
using LibraProgramming.Xamarin.Interaction.Contracts;
using LibraProgramming.Xamarin.Interaction.Extensions;
using LibraProgramming.Xamarin.Popups.Services;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AudioBookPlayer.App.ViewModels
{
    public sealed class SourceFolderRequestContext : InteractionRequestContext
    {

    }

    internal class BooksLibraryViewModel : ViewModelBase, IInitialize
    {
        private readonly IBookShelfProvider bookShelf;
        private readonly IPermissionRequestor permissionRequestor;
        private readonly IPopupService popupService;
        private readonly ApplicationSettings settings;
        private readonly Command refresh;
        private readonly InteractionRequest<SourceFolderRequestContext> selectSourceFolder;
        private bool isBusy;

        public bool IsBusy
        {
            get => isBusy;
            set => SetProperty(ref isBusy, value);
        }

        public ICommand Refresh => refresh;

        public IInteractionRequest SelectSourceFolder => selectSourceFolder;

        [PrefferedConstructor]
        public BooksLibraryViewModel(
            IBookShelfProvider bookShelf,
            IPermissionRequestor permissionRequestor,
            IPopupService popupService,
            ApplicationSettings settings)
        {
            this.bookShelf = bookShelf;
            this.permissionRequestor = permissionRequestor;
            this.popupService = popupService;
            this.settings = settings;

            selectSourceFolder = new InteractionRequest<SourceFolderRequestContext>();
            refresh = new Command(OnRefreshCommand);
        }

        void IInitialize.OnInitialize()
        {
            ;
        }

        private async void OnRefreshCommand()
        {
            try
            {
                IsBusy = true;

                var status = await permissionRequestor.CheckAndRequestMediaPermissionsAsync();

                if (PermissionStatus.Denied == status)
                {
                    return;
                }

                var path = settings.LibraryRootPath;
                var context = new SourceFolderRequestContext();

                await selectSourceFolder.RaiseAsync(context);
                

                //await Task.CompletedTask;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
