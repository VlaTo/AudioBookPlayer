using Android.App;
using AudioBookPlayer.App.Views;
using AudioBookPlayer.App.Views.Activities;

#nullable enable

namespace AudioBookPlayer.App.Presenters
{
    internal sealed class RecentBooksPresenter : BooksPresenter
    {
        public RecentBooksPresenter(MainActivity mainActivity)
            : base(mainActivity)
        {
        }

        protected override BooksListAdapter CreateBookListAdapter() =>
            new RecentBookListAdapter(Application.Context, Application.Context.Resources);
    }
}

#nullable restore