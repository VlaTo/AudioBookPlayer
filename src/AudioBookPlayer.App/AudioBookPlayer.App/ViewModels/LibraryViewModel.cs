using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Dependency.Container.Attributes;
using LibraProgramming.Xamarin.Interaction.Contracts;
using System.Threading.Tasks;

namespace AudioBookPlayer.App.ViewModels
{
    public sealed class LibraryViewModel : ViewModelBase, IInitialize
    {
        private readonly IBookShelfProvider provider;
        private readonly ITaskExecutionMonitor executionMonitor;
        private bool isBusy;

        public bool IsBusy
        {
            get => isBusy;
            set => SetProperty(ref isBusy, value);
        }

        public TaskCommand Refresh
        {
            get;
        }

        [PrefferedConstructor]
        public LibraryViewModel(IBookShelfProvider provider)
        {
            this.provider = provider;

            executionMonitor = new TaskExecutionMonitor(DoQueryLibrary);

            Refresh = new TaskCommand(DoRefreshLibrary);
        }

        void IInitialize.OnInitialize()
        {
            System.Diagnostics.Debug.WriteLine($"[LibraryViewModel] [OnInitialize] Executed");

            executionMonitor.Start();
        }

        private async Task DoQueryLibrary()
        {
            IsBusy = true;

            try
            {
                await provider.QueryBooksAsync();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task DoRefreshLibrary()
        {
            IsBusy = true;

            try
            {
                await provider.RefreshBooksAsync();
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
