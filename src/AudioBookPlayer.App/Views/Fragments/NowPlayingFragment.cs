#nullable enable

using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Reactive.Linq;
using AudioBookPlayer.App.Views.Activities;
using Fragment = AndroidX.Fragment.App.Fragment;

namespace AudioBookPlayer.App.Views.Fragments
{
    public sealed class NowPlayingFragment : Fragment, NowPlayingFragment.IArgumentKeys
    {
        public const string BookMediaIdArgumentKey = "Book.MediaID";

        private IDisposable? button1ClickSubscription;

        public string? MediaId => Arguments?.GetString(IArgumentKeys.BookId);

        public static NowPlayingFragment NewInstance(string mediaId)
        {
            var bundle = new Bundle();

            bundle.PutString(IArgumentKeys.BookId, mediaId);

            return new NowPlayingFragment
            {
                Arguments = bundle
            };
        }

        public override void OnDestroy()
        {
            button1ClickSubscription?.Dispose();
            base.OnDestroy();
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            HasOptionsMenu = true;

            ((MainActivity)Activity).SupportActionBar.Title = null;
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.fragment_menu_now_playing, menu);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var _ = base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.fragment_now_playing, container, false);

            if (null != view)
            {
                var button1 = view.FindViewById<Button>(Resource.Id.button1);
                var hint = view.FindViewById<TextView>(Resource.Id.hint_text_1);

                // var preferences = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
                // var flag = preferences.GetBoolean("checkbox_preference", false);
                // System.Diagnostics.Debug.WriteLine($"[NowPlayingFragment] [OnCreateView] Preference Flag: {flag}");

                if (null != hint)
                {
                    hint.Text = MediaId;
                }

                if (null != button1)
                {
                    button1.Text = "bottom";

                    button1ClickSubscription = Observable.FromEventPattern(
                            handler => button1.Click += handler,
                            handler => button1.Click -= handler
                        )
                        .Subscribe(pattern =>
                        {
                            var fragment = ChapterSelectionFragment.NewInstance(10);
                            fragment.Show(Activity.SupportFragmentManager, "dialog");
                        });
                }
            }

            return view;
        }

        //
        public interface IArgumentKeys
        {
            public const string BookId = BookMediaIdArgumentKey;
        }
    }
}