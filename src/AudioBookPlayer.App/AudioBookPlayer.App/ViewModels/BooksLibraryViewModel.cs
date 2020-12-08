using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Dependency.Container.Attributes;
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
            ApplicationSettings settings)
        {
            this.bookShelf = bookShelf;
            this.permissionRequestor = permissionRequestor;
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

                await Task.CompletedTask;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
