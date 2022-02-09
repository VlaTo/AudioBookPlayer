using Android.OS;
using Android.Views;
using AudioBookPlayer.App.Presenters;
using AudioBookPlayer.App.Views.Activities;
using Fragment = AndroidX.Fragment.App.Fragment;

#nullable enable

namespace AudioBookPlayer.App.Views.Fragments
{
    // https://stackoverflow.com/questions/41693154/custom-seekbar-thumb-size-color-and-background
    public sealed class NowPlayingFragment : Fragment, NowPlayingFragment.IArgumentKeys
    {
        public const string BookMediaIdArgumentKey = "Book.MediaID";

        private NowPlayingPresenter? presenter;

        public static NowPlayingFragment NewInstance(string mediaId)
        {
            var bundle = new Bundle();

            bundle.PutString(IArgumentKeys.BookId, mediaId);

            return new NowPlayingFragment
            {
                Arguments = bundle
            };
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            var activity = (MainActivity)Activity;

            base.OnCreate(savedInstanceState);

            if (null != activity.SupportActionBar)
            {
                activity.SupportActionBar.Title = null;
            }

            HasOptionsMenu = true;

            var mediaId = Arguments?.GetString(IArgumentKeys.BookId);

            presenter = new NowPlayingPresenter(mediaId, activity);
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.fragment_menu_now_playing, menu);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_now_playing, container, false);

            if (null != view)
            {
                presenter?.AttachView(view);
                return view;
            }

            return base.OnCreateView(inflater, container, savedInstanceState);
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
            presenter?.DetachView();
        }

        //
        public interface IArgumentKeys
        {
            public const string BookId = BookMediaIdArgumentKey;
        }
    }
}

#nullable restore