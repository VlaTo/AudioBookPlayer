using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using AndroidX.AppCompat.App;
using AudioBookPlayer.App.Presenters;

#nullable enable

namespace AudioBookPlayer.App.Views.Fragments
{
    public sealed class ChapterDialogFragment : AppCompatDialogFragment
    {
        private readonly ChapterDialogPresenter presenter;

        public static ChapterDialogFragment NewInstance()
        {
            var fragment = new ChapterDialogFragment();
            return fragment;
        }

        public ChapterDialogFragment()
        {
            presenter = new ChapterDialogPresenter(() => Dialog);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var _ = base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.fragment_chapter_selection, container, false);

            presenter.AttachView(view);

            return view ?? _;
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
            presenter.DetachView();
        }

        public override void OnStart()
        {
            base.OnStart();
            
            var window = Dialog.Window;

            if (null != window)
            {
                var displayMetrics = Application.Context.Resources?.DisplayMetrics;

                if (null != displayMetrics)
                {
                    var width = displayMetrics.WidthPixels - (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 52.0f, displayMetrics);
                    var height = displayMetrics.HeightPixels - (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 104.0f, displayMetrics);
                    var layoutParams = window.Attributes;

                    if (null != layoutParams)
                    {
                        layoutParams.Width = width;
                        layoutParams.Height = height;
                        
                        window.Attributes = layoutParams;
                    }
                }
            }
        }
    }
}

#nullable restore