using System;
using System.Threading;
using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Dependency.Container.Attributes;
using LibraProgramming.Xamarin.Interaction.Contracts;
using System.Threading.Tasks;
using AudioBookPlayer.App.Domain.Services;
using LibraProgramming.Media.Common;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AudioBookPlayer.App.ViewModels
{
    public sealed class LibraryViewModel : ViewModelBase, IInitialize
    {
        private readonly IBooksProvider booksProvider;
        private readonly IMediaLibrary mediaLibrary;
        private readonly IAudioBooksPublisher booksPublisher;
        private readonly ICoverProvider coverProvider;
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
            IMediaLibrary mediaLibrary,
            IAudioBooksPublisher booksPublisher,
            ICoverProvider coverProvider,
            IPermissionRequestor permissionRequestor)
        {
            this.booksProvider = booksProvider;
            this.mediaLibrary = mediaLibrary;
            this.booksPublisher = booksPublisher;
            this.coverProvider = coverProvider;
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
                var libraryBooks = await mediaLibrary.QueryBooksAsync();
                var actualBooks = await booksProvider.QueryBooksAsync();
                var comparer = new AudioBooksComparer();

                var changes = comparer.GetChanges(libraryBooks, actualBooks);

                var manager = new AudioBooksManager(mediaLibrary, coverProvider);

                await manager.ApplyChangesAsync(changes, CancellationToken.None);
                await DoQueryLibraryAsync(CancellationToken.None);
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
            var books = await mediaLibrary.QueryBooksAsync(cancellationToken);

            /*foreach (var book in books)
            {
                foreach (var cover in book.Images)
                {
                    if(String.Equals(WellKnownMediaTags.Cover, )) cover.Key
                }
            }*/

            booksPublisher.OnNext(books);
        }
    }
}
