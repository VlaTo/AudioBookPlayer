using Android.OS;
using Android.Views;
using AudioBookPlayer.App.Presenters;
using AudioBookPlayer.App.Views.Activities;
using Fragment = AndroidX.Fragment.App.Fragment;

#nullable enable

namespace AudioBookPlayer.App.Views.Fragments
{
    public class RecentBooksFragment : Fragment
    {
        private RecentBooksPresenter? presenter;

        public static RecentBooksFragment NewInstance()
        {
            var bundle = new Bundle();

            return new RecentBooksFragment
            {
                Arguments = bundle
            };
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            presenter = new RecentBooksPresenter((MainActivity)Activity);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_books_list, container, false);

            if (null != view)
            {
                presenter?.AttachView(view);
                return view;
            }

            return base.OnCreateView(inflater, container, savedInstanceState);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            presenter?.DetachView();
        }
    }
}

#nullable restore