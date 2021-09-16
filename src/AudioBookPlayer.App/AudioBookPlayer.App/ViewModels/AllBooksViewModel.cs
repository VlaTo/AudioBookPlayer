using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Dependency.Container.Attributes;

namespace AudioBookPlayer.App.ViewModels
{
    internal sealed class AllBooksViewModel : BooksViewModelBase
    {
        [PrefferedConstructor]
        public AllBooksViewModel(IMediaBrowserServiceConnector browserServiceConnector)
            : base(browserServiceConnector)
        {
        }

        protected override bool FilterSourceBook(AudioBook source) => true;

        protected override void BindSourceBooks(AudioBook[] audioBooks)
        {
            try
            {
                IsBusy = true;

                base.BindSourceBooks(audioBooks);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
