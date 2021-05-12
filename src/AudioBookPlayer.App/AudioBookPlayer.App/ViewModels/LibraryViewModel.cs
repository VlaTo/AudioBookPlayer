using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Dependency.Container.Attributes;
using LibraProgramming.Xamarin.Interaction.Contracts;
using Xamarin.Forms;

namespace AudioBookPlayer.App.ViewModels
{
    public sealed class LibraryViewModel : ViewModelBase, IInitialize
    {
        private readonly IBookShelfProvider provider;
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
        public LibraryViewModel(IBookShelfProvider provider)
        {
            this.provider = provider;

            Refresh = new Command(DoRefreshLibrary);
        }

        void IInitialize.OnInitialize()
        {
            System.Diagnostics.Debug.WriteLine($"[LibraryViewModel] [OnInitialize] Executed");

            //await provider.QueryBooksAsync();
        }

        private async void DoRefreshLibrary()
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
