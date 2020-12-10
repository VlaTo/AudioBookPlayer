using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Services;
using AudioBookPlayer.App.Views;
using LibraProgramming.Xamarin.Dependency.Container.Attributes;
using LibraProgramming.Xamarin.Interaction;
using LibraProgramming.Xamarin.Popups.Services;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AudioBookPlayer.App.ViewModels
{
    internal class BooksLibraryViewModel : ViewModelBase, IInitialize
    {
        private readonly IBookShelfProvider bookShelf;
        private readonly IPermissionRequestor permissionRequestor;
        private readonly IPopupService popupService;
        private readonly ApplicationSettings settings;
        private bool isBusy;

        public bool IsBusy
        {
            get => isBusy;
            set => SetProperty(ref isBusy, value);
        }

        public ICommand Refresh
        {
            get;
        }

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

            Refresh = new Command(OnRefreshCommand);
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
                var page = new ChooseLibraryFolderPopup();

                //await popupService.ShowPopupAsync(page);
                await Shell.Current.Navigation.PushModalAsync(page);

                //await Task.CompletedTask;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
