#nullable enable

using Android;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.Media;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using AndroidX.ViewPager2.Adapter;
using AndroidX.ViewPager2.Widget;
using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Views.Activities;
using AudioBookPlayer.Domain;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Tabs;
using Java.Util.Concurrent;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using PermissionChecker = AudioBookPlayer.Core.PermissionChecker;

namespace AudioBookPlayer.App.Views.Fragments
{
    public class LibraryFragment : Fragment,
        TabLayoutMediator.ITabConfigurationStrategy,
        MediaBrowserServiceConnector.IConnectCallback,
        MediaBrowserServiceConnector.IAudioBooksCallback,
        MediaBrowserServiceConnector.IUpdateCallback
    {
        //private static readonly long PreloaderDelay = TimeUnit.Milliseconds.ToMillis(200L);

        private FloatingActionButton? fab;
        private ViewPager2? viewPager;
        private TabLayout? tabsLayout;
        //private CircularProgressIndicator? busyIndicator;
        //private LinearLayout? busyIndicatorLayout;
        //private FrameLayout? overlayLayout;
        private TabLayoutMediator? layoutMediator;
        // private TextView? busyIndicatorText;
        private IDisposable? fabClickSubscription;
        private MediaBrowserServiceConnector.IMediaBrowserService? browserService;
        //private WaitIndicator? indicator;
        //private Timer? preloaderTimer;

        internal MainActivity MainActivity => (MainActivity)Activity;

        internal MediaBrowserServiceConnector? ServiceConnector => MainActivity.ServiceConnector;

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

            //indicator = new WaitIndicator(Activity);
            HasOptionsMenu = true;

            /*var loader = BookImageTask<ImageView>()

            loader.Start();
            var _ = loader.Looper;*/
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            //var old = base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.fragment_library, container, false);

            if (null != view)
            {
                viewPager = view.FindViewById<ViewPager2>(Resource.Id.tabs_pager);
                tabsLayout = view.FindViewById<TabLayout>(Resource.Id.tabs_layout);
                fab = view.FindViewById<FloatingActionButton>(Resource.Id.fab);

                //overlayLayout = view.FindViewById<FrameLayout>(Resource.Id.overlay_layout);

                /*if (null != indicator)
                {
                    indicator.ParseLayout(view);
                }*/

                if (null != viewPager)
                {
                    viewPager.Adapter = new ViewsAdapter(this);
                }

                /*if (null != overlayLayout)
                {
                    busyIndicatorLayout = view.FindViewById<LinearLayout>(Resource.Id.busy_indicator_layout);

                    if (null != busyIndicatorLayout)
                    {
                        //busyIndicator = view.FindViewById<CircularProgressIndicator>(Resource.Id.busy_indicator);
                        busyIndicatorText = view.FindViewById<TextView>(Resource.Id.busy_indicator_text);
                    }
                }*/

                if (null != fab)
                {
                    fabClickSubscription = Observable
                        .FromEventPattern(
                            handler => fab.Click += handler,
                            handler => fab.Click -= handler)
                        .Subscribe(
                            pattern => OnFabClick((EventArgs)pattern.EventArgs)
                        );
                }

                if (null != tabsLayout)
                {
                    layoutMediator = new TabLayoutMediator(tabsLayout, viewPager, true, true, this);
                    layoutMediator.Attach();
                }
            }

            MainActivity.SupportActionBar.SetTitle(Resource.String.title_library);

            return view; //?? old;
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.fragment_menu_options, menu);
        }

        public override void OnStart()
        {
            base.OnStart();

            /*preloaderTimer = new Timer();
            preloaderTimer.Schedule(new PreloaderTimerTask(preloaderTimer, ShowPreloader), PreloaderDelay);*/

            if (null != ServiceConnector)
            {
                ServiceConnector.Connect(this);
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            var id = item.ItemId;

            if (id == Resource.Id.action_library_update)
            {
                if (null == browserService)
                {
                    return false;
                }

                PermissionChecker.CheckPermissions(View, new[] { Manifest.Permission.ReadExternalStorage }, DoRequestPermissionsResult);

                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void OnFabClick(EventArgs _)
        {
            var fragment = NowPlayingFragment.NewInstance(MediaID.Root);
            Activity.SupportFragmentManager
                .BeginTransaction()
                .Replace(Resource.Id.nav_host_frame, fragment)
                .AddToBackStack(null)
                .SetTransition(FragmentTransaction.TransitFragmentFade)
                .Commit();
        }

        private void DoRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            //indicator?.Show();
            browserService?.UpdateLibrary(this);
        }

        #region TabLayoutMediator.ITabConfigurationStrategy

        void TabLayoutMediator.ITabConfigurationStrategy.OnConfigureTab(TabLayout.Tab tab, int index)
        {
            var resourceIds = new[]
            {
                Resource.String.tab_all_books,
                Resource.String.tab_recent_books
            };

            tab.SetText(resourceIds[index]);
        }

        #endregion

        #region MediaBrowserServiceConnector.IAudioBooksResultCallback

        void MediaBrowserServiceConnector.IAudioBooksCallback.OnAudioBooksReady(IList<MediaBrowserCompat.MediaItem> list, Bundle options)
        {
            /*indicator?.Hide();

            for (var index = 0; index < list.Count; index++)
            {
                var book = list[index];
            }*/
        }

        void MediaBrowserServiceConnector.IAudioBooksCallback.OnAudioBooksError(Bundle options)
        {
            //indicator?.Hide();
        }

        #endregion

        #region MediaBrowserServiceConnector.IConnectCallback

        void MediaBrowserServiceConnector.IConnectCallback.OnConnected(MediaBrowserServiceConnector.IMediaBrowserService service)
        {
            browserService = service;
            browserService.GetAudioBooks(this);
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

        #region MediaBrawserServiceConnector.IUpdateCallback

        void MediaBrowserServiceConnector.IUpdateCallback.OnUpdateProgress(int step, float progress)
        {
            /*var textId = Resource.String.library_update_collecting;

            switch (step)
            {
                case MediaBrowserService.MediaBrowserService.IUpdateLibrarySteps.Reading:
                {
                    textId = Resource.String.library_update_collecting;
                    break;
                }

                case MediaBrowserService.MediaBrowserService.IUpdateLibrarySteps.Processing:
                {
                    textId = Resource.String.library_update_processing;
                    break;
                }
            }

            Activity.RunOnUiThread(() =>
            {
                var text = GetString(textId, progress * 100.0f);
                indicator?.Show(text);
            });*/
        }

        void MediaBrowserServiceConnector.IUpdateCallback.OnUpdateResult()
        {
            ;
        }

        #endregion

        /*private void ShowPreloader()
        {
            if (null != overlayLayout)
            {
                Activity.RunOnUiThread(() =>
                {
                    busyIndicatorText.Text = String.Empty;
                    overlayLayout.Visibility = ViewStates.Visible;
                });
            }
        }

        private void HidePreloader()
        {
            if (overlayLayout is { Visibility: ViewStates.Visible })
            {
                Activity.RunOnUiThread(() => overlayLayout.Visibility = ViewStates.Gone);
            }
        }*/

        /// <summary>
        /// ViewsAdapter
        /// </summary>
        private sealed class ViewsAdapter : FragmentStateAdapter
        {
            private readonly Func<Fragment>[] creators;

            public override int ItemCount => creators.Length;

            public ViewsAdapter(Fragment fragment)
                : base(fragment)
            {
                creators = new Func<Fragment>[]
                {
                    AllBooksFragment.NewInstance,
                    RecentBooksFragment.NewInstance
                };
            }

            public override Fragment CreateFragment(int index) => creators[index].Invoke();
        }
    }
}