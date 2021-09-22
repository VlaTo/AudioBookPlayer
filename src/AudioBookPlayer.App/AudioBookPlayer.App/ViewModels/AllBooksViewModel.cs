using System.Collections.Generic;
using AudioBookPlayer.App.Domain.Services;
using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Dependency.Container.Attributes;

namespace AudioBookPlayer.App.ViewModels
{
    internal sealed class AllBooksViewModel : BooksViewModelBase
    {
        [PrefferedConstructor]
        public AllBooksViewModel(
            IMediaBrowserServiceConnector browserServiceConnector,
            ICoverService coverService)
            : base(browserServiceConnector, coverService)
        {
        }

        protected override bool FilterSourceBook(BookPreviewViewModel source) => true;

        protected override void BindSourceBooks(IReadOnlyList<BookPreviewViewModel> models)
        {
            try
            {
                IsBusy = true;

                base.BindSourceBooks(models);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
