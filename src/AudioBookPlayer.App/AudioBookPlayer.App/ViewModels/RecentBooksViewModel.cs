using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Domain.Services;
using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Dependency.Container.Attributes;
using System.Collections.Generic;

namespace AudioBookPlayer.App.ViewModels
{
    internal sealed class RecentBooksViewModel : BooksViewModelBase
    {
        [PrefferedConstructor]
        public RecentBooksViewModel(
            IMediaBrowserServiceConnector browserServiceConnector,
            ICoverService coverService)
            : base(browserServiceConnector, coverService)
        {
        }

        protected override bool FilterSourceBook(BookItem bookItem) => true;

        protected override void BindSourceBooks(IReadOnlyList<BookItem> bookItems)
        {
            try
            {
                IsBusy = true;

                base.BindSourceBooks(bookItems);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
