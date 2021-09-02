using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Domain.Data;
using AudioBookPlayer.App.Domain.Services;
using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Dependency.Container.Attributes;
using LibraProgramming.Xamarin.Interaction.Contracts;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AudioBookPlayer.App.ViewModels
{
    public sealed class LibraryViewModel : ViewModelBase, IInitialize
    {
        private readonly IBooksProvider booksProvider;
        private readonly IBooksService booksService;
        private readonly AudioBooksLibrary booksLibrary;
        private readonly IAudioBooksPublisher booksPublisher;
        private readonly IPermissionRequestor permissionRequestor;
        private readonly ITaskExecutionMonitor updateExecutionMonitor;
        private readonly ITaskExecutionMonitor refreshExecutionMonitor;
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
            IBooksProvider booksProvider,
            IBooksService booksService,
            AudioBooksLibrary booksLibrary,
            IAudioBooksPublisher booksPublisher,
            IPermissionRequestor permissionRequestor)
        {
            this.booksProvider = booksProvider;
            this.booksService = booksService;
            this.booksLibrary = booksLibrary;
            this.booksPublisher = booksPublisher;
            this.permissionRequestor = permissionRequestor;

            updateExecutionMonitor = new TaskExecutionMonitor(ExecuteQueryLibraryAsync);
            refreshExecutionMonitor = new TaskExecutionMonitor(ExecuteLibraryRefreshAsync);

            Refresh = new Command(DoLibraryRefreshAsync);
        }

        void IInitialize.OnInitialize()
        {
            updateExecutionMonitor.Start();
        }

        private async Task ExecuteQueryLibraryAsync()
        {
            IsBusy = true;

            try
            {
                await DoQueryLibraryAsync(CancellationToken.None);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ExecuteLibraryRefreshAsync()
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
                        await DoQueryLibraryAsync(CancellationToken.None);
                    }
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void DoLibraryRefreshAsync()
        {
            var status = await permissionRequestor.CheckAndRequestMediaPermissionsAsync();

            if (PermissionStatus.Denied == status)
            {
                return;
            }

            refreshExecutionMonitor.Start();
        }

        private async Task DoQueryLibraryAsync(CancellationToken cancellationToken = default)
        {
            var books = await booksService.QueryBooksAsync(cancellationToken);
            booksPublisher.OnNext(books);
        }
    }
}
