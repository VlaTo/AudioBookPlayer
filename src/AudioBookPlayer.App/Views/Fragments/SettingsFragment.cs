using System;
using Android.OS;
using Android.Views;
using AndroidX.Preference;
using AudioBookPlayer.App.Views.Activities;
using Debug = System.Diagnostics.Debug;
using Object = Java.Lang.Object;

namespace AudioBookPlayer.App.Views.Fragments
{
    public class SettingsFragment : PreferenceFragmentCompat, Preference.IOnPreferenceChangeListener
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

            if (null != view)
            {
                var seekBar = (SeekBarPreference)FindPreference("Playback.SeekStep");

                seekBar.SeekBarIncrement = 5;
                seekBar.ShowSeekBarValue = true;
                seekBar.OnPreferenceChangeListener = this;
            }

            //((MainActivity)Activity).SupportActionBar.SetTitle(Resource.String.title_settings);

            return view;
        }

        #region IOnPreferenceChangeListener

        bool Preference.IOnPreferenceChangeListener.OnPreferenceChange(Preference preference, Object newValue)
        {
            Debug.WriteLine($"Rewind value: {newValue}");
            return true;
        }

        #endregion
    }
}