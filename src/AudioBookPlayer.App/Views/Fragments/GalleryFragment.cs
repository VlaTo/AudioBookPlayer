using Android.OS;
using Android.Views;
using AndroidX.Fragment.App;

namespace AudioBookPlayer.App.Views.Fragments
{
    public class GalleryFragment : Fragment
    {
        public bool Test => Arguments.GetBoolean("Test", false);

        public static GalleryFragment NewInstance(bool test)
        {
            var bundle = new Bundle();

            bundle.PutBoolean("Test", test);

            return new GalleryFragment
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
            // Use this to return your custom view for this Fragment
            var view = inflater.Inflate(Resource.Layout.fragment_library, container, false);
            /*var hintText = view?.FindViewById<TextView>(Resource.Id.hint_text);

            if (null != hintText)
            {
                hintText.Text = "Gallery fragment";
            }*/
            //return base.OnCreateView(inflater, container, savedInstanceState);

            return view;
        }
    }
}