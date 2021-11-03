using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Dependency.Container.Attributes;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AudioBookPlayer.App.ViewModels
{
    public sealed class LibraryViewModel : ViewModelBase
    {
        private readonly IMediaBrowserServiceConnector connector;
        private readonly IUpdateLibraryService updateLibraryService;
        //private readonly IPermissionRequestor permissions;
        //private readonly ITaskExecutionMonitor libraryUpdateMonitor;
        private bool isBusy;

        public bool IsBusy
        {
            get => isBusy;
            set => SetProperty(ref isBusy, value);
        }

        public Command UpdateLibrary
        {
            get;
        }

        [PrefferedConstructor]
        public LibraryViewModel(
            IMediaBrowserServiceConnector connector,
            IUpdateLibraryService updateLibraryService
            //IPermissionRequestor permissions
            )
        {
            this.connector = connector;
            this.updateLibraryService = updateLibraryService;
            //this.permissions = permissions;

            //libraryUpdateMonitor = new TaskExecutionMonitor(ExecuteLibraryRefreshAsync);

            UpdateLibrary = new Command(DoUpdateLibrary);
        }

        /*private Task ExecuteLibraryRefreshAsync()
        {
            IsBusy = true;

            try
            {
                // 1. Get books from device and library
                var actualBooks = await booksProvider.QueryBooksAsync();
                var libraryBooks = await booksService.QueryBooksAsync();
                // 2. Compare collections, get differences
                var changes = booksLibrary.GetChanges(libraryBooks, actualBooks);
                // 3. Apply differences to library
                if (0 < changes.Count)
                {
                    var success = await booksLibrary.TryApplyChangesAsync(booksService, changes, CancellationToken.None);

                    if (success)
                    {
                        //await DoQueryLibraryAsync(CancellationToken.None);
                    }
                }
            }
            finally
            {
                IsBusy = false;
            }
        }*/

        private void DoUpdateLibrary()
        {
            updateLibraryService.StartUpdate();

            /*var status = await permissions.CheckAndRequestMediaPermissionsAsync();

            if (PermissionStatus.Denied == status)
            {
                return;
            }

            refreshExecution.Start();*/
        }
    }
}
