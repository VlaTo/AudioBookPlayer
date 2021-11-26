using Android.OS;
using Android.Views;
using AndroidX.Preference;
using AudioBookPlayer.App.Views.Activities;

namespace AudioBookPlayer.App.Views.Fragments
{
    public class SettingsFragment : PreferenceFragmentCompat
    {
        public static SettingsFragment NewInstance()
        {
            var bundle = Bundle.Empty;
            return new SettingsFragment
            {
                Arguments = bundle
            };
        }

        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            AddPreferencesFromResource(Resource.Xml.fragment_settings);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);
            
            ((MainActivity)Activity).SupportActionBar.SetTitle(Resource.String.title_settings);

            return view;
        }
    }
}