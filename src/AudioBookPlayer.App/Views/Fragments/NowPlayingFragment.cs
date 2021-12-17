#nullable enable

using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Reactive.Linq;
using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Views.Activities;
using Fragment = AndroidX.Fragment.App.Fragment;

namespace AudioBookPlayer.App.Views.Fragments
{
    public sealed class NowPlayingFragment : Fragment, NowPlayingFragment.IArgumentKeys, MediaBrowserServiceConnector.IConnectCallback
    {
        public const string BookMediaIdArgumentKey = "Book.MediaID";

        private IDisposable? button1ClickSubscription;
        private MediaBrowserServiceConnector.IMediaBrowserService? browserService;

        public string? MediaId => Arguments?.GetString(IArgumentKeys.BookId);

        public MainActivity MainActivity => (MainActivity)Activity;

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
            base.OnCreate(savedInstanceState);

            HasOptionsMenu = true;
            MainActivity.SupportActionBar.Title = null;
            MainActivity.ServiceConnector?.Connect(this);
        }

        public override void OnDestroy()
        {
            button1ClickSubscription?.Dispose();
            base.OnDestroy();
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

        #region MediaBrowserServiceConnector.IConnectCallback

        void MediaBrowserServiceConnector.IConnectCallback.OnConnected(MediaBrowserServiceConnector.IMediaBrowserService service)
        {
            browserService = service;
            browserService.PrepareFromMediaId(MediaId, Bundle.Empty);
        }

        void MediaBrowserServiceConnector.IConnectCallback.OnSuspended()
        {
            throw new NotImplementedException();
        }

        void MediaBrowserServiceConnector.IConnectCallback.OnFailed()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}