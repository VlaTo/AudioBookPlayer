using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Domain.Services;
using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Dependency.Container.Attributes;
using System;
using System.Threading.Tasks;
using AudioBookPlayer.App.Domain.Providers;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AudioBookPlayer.App.ViewModels
{
    public sealed class LibraryViewModel : ViewModelBase//, IInitialize
    {
        private readonly IBooksProvider booksProvider;
        private readonly IBooksService booksService;
        //private readonly AudioBooksLibrary booksLibrary;
        private readonly IMediaBrowserServiceConnector connector;
        private readonly IPermissionRequestor permissions;
        private readonly ITaskExecutionMonitor refreshExecution;
        private IDisposable browserServiceSubscription;
        private bool isBusy;

        public bool IsBusy
        {
            get => isBusy;
            set => SetProperty(ref isBusy, value);
        }

        public Command Refresh
        {
            get;
        }

        [PrefferedConstructor]
        public LibraryViewModel(
            IMediaBrowserServiceConnector connector,
            IPermissionRequestor permissions)
        {
            //this.booksProvider = booksProvider;
            //this.booksService = booksService;
            //this.booksLibrary = booksLibrary;
            //this.booksPublisher = booksPublisher;

            this.connector = connector;
            this.permissions = permissions;

            refreshExecution = new TaskExecutionMonitor(ExecuteLibraryRefreshAsync);

            Refresh = new Command(DoLibraryRefreshAsync);
        }

        private Task ExecuteLibraryRefreshAsync()
        {
            return connector.UpdateLibraryAsync();

            /*IsBusy = true;

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
            }*/
        }

        private async void DoLibraryRefreshAsync()
        {
            var status = await permissions.CheckAndRequestMediaPermissionsAsync();

            if (PermissionStatus.Denied == status)
            {
                return;
            }

            refreshExecution.Start();
        }
    }
}
