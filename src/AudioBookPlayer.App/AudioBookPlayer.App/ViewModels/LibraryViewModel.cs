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
        // private readonly IBookShelfProvider provider;
        private readonly IMediaService mediaService;
        private readonly IAudioBooksPublisher booksPublisher;
        private readonly ITaskExecutionMonitor executionMonitor;
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
            //IBookShelfProvider provider,
            IMediaService mediaService,
            IAudioBooksPublisher booksPublisher)
        {
            // this.provider = provider;
            this.mediaService = mediaService;
            this.booksPublisher = booksPublisher;

            executionMonitor = new TaskExecutionMonitor(DoQueryLibraryAsync);

            Refresh = new Command(DoRefreshLibrary);
        }

        void IInitialize.OnInitialize()
        {
            System.Diagnostics.Debug.WriteLine($"[LibraryViewModel] [OnInitialize] Executed");

            executionMonitor.Start();
        }

        private async Task DoQueryLibraryAsync()
        {
            IsBusy = true;

            try
            {
                var audioBooks = await mediaService.QueryBooksAsync();
                booksPublisher.OnNext(audioBooks);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void DoRefreshLibrary()
        {
            executionMonitor.Start();
        }
    }
}
