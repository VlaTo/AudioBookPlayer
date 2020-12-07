using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Dependency.Container.Attributes;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AudioBookPlayer.App.ViewModels
{
    internal class BooksLibraryViewModel : ViewModelBase
    {
        private readonly IBookShelfProvider bookShelf;
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
        public BooksLibraryViewModel(IBookShelfProvider bookShelf)
        {
            this.bookShelf = bookShelf;

            Refresh = new Command(OnRefreshCommand);
        }

        private async void OnRefreshCommand()
        {
            try
            {
                IsBusy = true;

                await Task.CompletedTask;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
