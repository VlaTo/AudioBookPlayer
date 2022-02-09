using Android.App;
using AudioBookPlayer.App.Views;
using AudioBookPlayer.App.Views.Activities;

#nullable enable

namespace AudioBookPlayer.App.Presenters
{
    internal sealed class AllBooksPresenter : BooksPresenter
    {
        public AllBooksPresenter(MainActivity mainActivity)
            : base(mainActivity)
        {
        }

        protected override BooksListAdapter CreateBookListAdapter() =>
            new AllBooksListAdapter(Application.Context, Application.Context.Resources);
    }
}

#nullable restore