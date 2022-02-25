using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;

#nullable enable

namespace AudioBookPlayer.App.Views.Fragments
{
    public sealed class ChapterDialogFragment : AppCompatDialogFragment
    {
        private RecyclerView? recyclerView;

        public static ChapterDialogFragment NewInstance()
        {
            var fragment = new ChapterDialogFragment();
            
            ;
            
            return fragment;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var _ = base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.fragment_chapter_selection, container, false);
            
            if (null != view)
            {
                recyclerView = view.FindViewById<RecyclerView>(Resource.Id.list_view_recycler);
            }

            var window = Dialog.Window;

            if (null != window)
            {
                window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
                window.RequestFeature(WindowFeatures.NoTitle);
            }

            return view ?? _;
        }
    }
}

#nullable restore