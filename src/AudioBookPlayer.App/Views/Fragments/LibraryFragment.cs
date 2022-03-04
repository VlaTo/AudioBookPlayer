using Android.OS;
using Android.Views;
using AndroidX.Fragment.App;
using AudioBookPlayer.App.Presenters;

#nullable enable

namespace AudioBookPlayer.App.Views.Fragments
{
    public class LibraryFragment : Fragment
    {
        private LibraryPresenter? presenter;

        public static LibraryFragment NewInstance()
        {
            var bundle = new Bundle();

            return new LibraryFragment
            {
                Arguments = bundle
            };
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            presenter = new LibraryPresenter(Activity);
            
            HasOptionsMenu = true;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var _ = base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.fragment_library, container, false);

            if (null != presenter)
            {
                presenter.AttachView(view);
            }

            return view ?? _;
        }

        public override void OnDetach()
        {
            if (null != presenter)
            {
                presenter.DetachView();
            }

            base.OnDetach();
        }
    }
}

#nullable restore