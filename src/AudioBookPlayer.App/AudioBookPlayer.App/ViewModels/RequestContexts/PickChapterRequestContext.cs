using LibraProgramming.Xamarin.Interaction;

namespace AudioBookPlayer.App.ViewModels.RequestContexts
{
    public sealed class PickChapterRequestContext : InteractionRequestContext
    {
        public long BookId
        {
            get;
        }

        public PickChapterRequestContext(long bookId)
        {
            BookId = bookId;
        }
    }
}