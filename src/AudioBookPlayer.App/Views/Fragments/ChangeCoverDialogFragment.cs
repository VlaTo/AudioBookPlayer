using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using AndroidX.AppCompat.App;
using AudioBookPlayer.App.Presenters;

namespace AudioBookPlayer.App.Views.Fragments
{
    public class ChangeCoverDialogFragment : AppCompatDialogFragment
    {
        private readonly ChangeCoverDialogPresenter presenter;

        public static ChangeCoverDialogFragment NewInstance()
        {
            var fragment = new ChangeCoverDialogFragment
            {
                Arguments = new Bundle()
            };

            return fragment;
        }

        public ChangeCoverDialogFragment()
        {
            presenter = new ChangeCoverDialogPresenter(() => Dialog);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var _ = base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.fragment_section_reorder, container, false);

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