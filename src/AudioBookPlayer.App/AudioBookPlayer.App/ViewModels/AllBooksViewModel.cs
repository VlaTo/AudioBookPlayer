using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Dependency.Container.Attributes;

namespace AudioBookPlayer.App.ViewModels
{
    internal sealed class AllBooksViewModel : BooksViewModelBase
    {
        [PrefferedConstructor]
        public AllBooksViewModel(IBookShelfProvider bookShelf)
            : base(bookShelf)
        {
        }

        protected override void DoPlayBook(AudioBookViewModel book)
        {
            base.DoPlayBook(book);
        }
    }
}
