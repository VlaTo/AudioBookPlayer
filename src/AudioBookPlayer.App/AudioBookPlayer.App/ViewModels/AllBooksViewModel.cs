using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Domain.Services;
using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Dependency.Container.Attributes;
using System.Collections.Generic;

namespace AudioBookPlayer.App.ViewModels
{
    internal sealed class AllBooksViewModel : BooksViewModelBase
    {
        [PrefferedConstructor]
        public AllBooksViewModel(
            IMediaBrowserServiceConnector serviceConnector,
            ICoverService coverService)
            : base(serviceConnector, coverService)
        {
        }

        protected override bool FilterSourceBook(BookItem bookItem) => true;

        protected override void BindBooks(IReadOnlyList<BookItem> bookItems)
        {
            try
            {
                IsBusy = true;

                base.BindBooks(bookItems);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
