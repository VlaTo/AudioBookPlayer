using System.Threading;
using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Dependency.Container.Attributes;
using LibraProgramming.Xamarin.Interaction.Contracts;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AudioBookPlayer.App.ViewModels
{
    public sealed class LibraryViewModel : ViewModelBase, IInitialize
    {
        private readonly IBooksProvider booksProvider;
        private readonly IMediaLibrary mediaLibrary;
        private readonly IAudioBooksPublisher booksPublisher;
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
            IAudioBooksPublisher booksPublisher)
        {
            this.booksProvider = booksProvider;
            this.mediaLibrary = mediaLibrary;
            this.booksPublisher = booksPublisher;

            updateExecutionMonitor = new TaskExecutionMonitor(ExecuteLibraryUpdateAsync);
            refreshExecutionMonitor = new TaskExecutionMonitor(ExecuteLibraryRefreshAsync);

            Refresh = new Command(refreshExecutionMonitor.Start);
        }

        void IInitialize.OnInitialize()
        {
            System.Diagnostics.Debug.WriteLine($"[LibraryViewModel] [OnInitialize] Executed");

            updateExecutionMonitor.Start();
        }

        private async Task ExecuteLibraryUpdateAsync()
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
                var patcher = new AudioBooksPatcher();

                var changes = patcher.GetChanges(libraryBooks, actualBooks);

                var manager = new AudioBooksManager(mediaLibrary);

                await manager.ApplyChangesAsync(changes, CancellationToken.None);
                await DoQueryLibraryAsync(CancellationToken.None);
            }
            finally
            {
                IsBusy = false;
            }
        }

        /*private void DoRefreshLibrary()
        {
            refreshExecutionMonitor.Start();
        }*/

        private async Task DoQueryLibraryAsync(CancellationToken cancellationToken = default)
        {
            var books = await mediaLibrary.QueryBooksAsync(cancellationToken);
            booksPublisher.OnNext(books);
        }
    }
}
