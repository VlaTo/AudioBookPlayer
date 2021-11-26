#nullable enable

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Fragment = AndroidX.Fragment.App.Fragment;

namespace AudioBookPlayer.App.Views.Fragments
{
    public class RecentBooksFragment : Fragment
    {
        private ListView? listView;

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

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var _ = base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.fragment_books_list, container, false);

            if (null != view)
            {
                listView = view.FindViewById<ListView>(Resource.Id.books_list);

                if (null != listView)
                {
                    var books = new[]
                    {
                        "Recent book #1",
                        "Recent book #2",
                        "Recent book #3",
                        "Recent book #4",
                        "Recent book #5"
                    };

                    listView.Adapter = new ArrayAdapter<string>(
                        Application.Context,
                        Android.Resource.Layout.SimpleListItem1,
                        books
                    );
                }
            }

            return view;
        }
    }
}