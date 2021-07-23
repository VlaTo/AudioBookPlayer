using AudioBookPlayer.App.Models;
using LibraProgramming.Xamarin.Interaction;

namespace AudioBookPlayer.App.ViewModels.RequestContexts
{
    public sealed class BookmarkRequestContext : InteractionRequestContext
    {
        public BookPosition Bookmark
        {
            get;
        }

        public BookmarkRequestContext(BookPosition bookmark)
        {
            Bookmark = bookmark;
        }
    }
}