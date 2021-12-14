#nullable enable

using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Annotations;
using AndroidX.Fragment.App;
using AndroidX.ViewPager2.Adapter;
using AndroidX.ViewPager2.Widget;
using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Views.Activities;
using AudioBookPlayer.Domain;
using Google.Android.Material.ProgressIndicator;
using Google.Android.Material.Tabs;
using System;
using System.Collections.Generic;
using Android.Support.V4.Media;
using Java.Util;
using Java.Util.Concurrent;

namespace AudioBookPlayer.App.Views.Fragments
{
    public class LibraryFragment : Fragment,
        TabLayoutMediator.ITabConfigurationStrategy,
        MediaBrowserServiceConnector.IConnectCallback,
        MediaBrowserServiceConnector.IAudioBooksCallback
    {
        private static readonly Func<Fragment>[] fragmentCreators;
        private static readonly long PreloaderDelay = TimeUnit.Milliseconds.ToMillis(200L);

        private MediaBrowserServiceConnector.IMediaBrowserService? browserService;
        private ViewPager2? viewPager;
        private TabLayout? tabsLayout;
        private CircularProgressIndicator? busyIndicator;
        private FrameLayout? overlayLayout;
        private TabLayoutMediator? layoutMediator;
        private Timer? preloaderTimer;

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

        static LibraryFragment()
        {
            fragmentCreators = new Func<Fragment>[]
            {
                () =>
                {
                    var fragment = AllBooksFragment.NewInstance();
                    return fragment;
                },
                () =>
                {
                    var fragment = RecentBooksFragment.NewInstance();
                    return fragment;
                }
            };
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            HasOptionsMenu = true;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var _ = base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.fragment_library, container, false);

            viewPager = view?.FindViewById<ViewPager2>(Resource.Id.tabs_pager);
            tabsLayout = view?.FindViewById<TabLayout>(Resource.Id.tabs_layout);
            busyIndicator = view?.FindViewById<CircularProgressIndicator>(Resource.Id.busy_indicator);
            overlayLayout = view?.FindViewById<FrameLayout>(Resource.Id.overlay_layout);

            if (null != viewPager)
            {
                viewPager.Adapter = new ViewsAdapter(this, fragmentCreators);
                viewPager.SetPageTransformer(new ZoomOutPageTransformer());
            }

            if (null != tabsLayout)
            {
                //tabsLayout.AddOnTabSelectedListener(new TabSelectedListener());
                layoutMediator = new TabLayoutMediator(tabsLayout, viewPager, true, true, this);
                layoutMediator.Attach();
            }

            if (null != overlayLayout)
            {
                ;

                if (null != busyIndicator)
                {
                    //busyIndicator.ShowAnimationBehavior = BaseProgressIndicator.ShowInward;
                    //busyIndicator.HideAnimationBehavior = BaseProgressIndicator.HideOutward;
                    //busyIndicator.Show();
                    //busyIndicator.Visibility = ViewStates.Visible;
                }
            }

            MainActivity.SupportActionBar.SetTitle(Resource.String.title_library);

            return view;
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.fragment_menu_options, menu);
        }

        public override void OnStart()
        {
            base.OnStart();

            preloaderTimer = new Timer();
            preloaderTimer.Schedule(new PreloaderTimerTask(preloaderTimer, ShowPreloader), PreloaderDelay);

            if (null != ServiceConnector)
            {
                ServiceConnector.Connect(this);
            }
        }

        public override void OnResume()
        {
            base.OnResume();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            var id = item.ItemId;

            if (id == Resource.Id.action_library_update)
            {
                // ServiceConnector?.UpdateLibrary();
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        #region TabLayoutMediator.ITabConfigurationStrategy

        void TabLayoutMediator.ITabConfigurationStrategy.OnConfigureTab(TabLayout.Tab tab, int index)
        {
            var resourceIds = new[] { Resource.String.tab_all_books, Resource.String.tab_recent_books };
            tab.SetText(resourceIds[index]);
        }

        #endregion

        #region MediaBrowserServiceConnector.IAudioBooksResultCallback

        void MediaBrowserServiceConnector.IAudioBooksCallback.OnAudioBooksReady(IList<MediaBrowserCompat.MediaItem> list, Bundle options)
        {
            if (null != preloaderTimer)
            {
                preloaderTimer.Cancel();
            }

            for (var index = 0; index < list.Count; index++)
            {
                var book = list[index];
            }

            HidePreloader();
        }

        void MediaBrowserServiceConnector.IAudioBooksCallback.OnAudioBooksError(Bundle options)
        {
            if (null != preloaderTimer)
            {
                preloaderTimer.Cancel();
            }

            HidePreloader();
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

        private void ShowPreloader()
        {
            if (null != overlayLayout)
            {
                Activity.RunOnUiThread(() => overlayLayout.Visibility = ViewStates.Visible);
            }
        }

        private void HidePreloader()
        {
            if (overlayLayout is { Visibility: ViewStates.Visible })
            {
                Activity.RunOnUiThread(() => overlayLayout.Visibility = ViewStates.Gone);
            }
        }

        // 
        private sealed class ViewsAdapter : FragmentStateAdapter
        {
            private readonly Func<Fragment>[] fragmentCreators;

            public override int ItemCount => fragmentCreators.Length;

            public ViewsAdapter(Fragment fragment, Func<Fragment>[] fragmentCreators)
                : base(fragment)
            {
                this.fragmentCreators = fragmentCreators;
            }

            public override Fragment CreateFragment(int index)
            {
                return fragmentCreators[index].Invoke();
            }
        }

        //
        [RequiresApi(Api = 21)]
        private sealed class ZoomOutPageTransformer : Java.Lang.Object, ViewPager2.IPageTransformer
        {
            private const float MIN_SCALE = 0.85f;
            private const float MIN_ALPHA = 0.5f;

            public void TransformPage(View view, float position)
            {
                var pageWidth = view.Width;
                var pageHeight = view.Height;

                if (position < -1)
                { // [-Infinity,-1)
                    // This page is way off-screen to the left.
                    view.Alpha = 0.0f;

                }
                else if (position <= 1)
                { // [-1,1]
                    // Modify the default slide transition to shrink the page as well
                    var scaleFactor = MathF.Max(MIN_SCALE, 1 - MathF.Abs(position));
                    var vertMargin = pageHeight * (1.0f - scaleFactor) / 2.0f;
                    var horzMargin = pageWidth * (1.0f - scaleFactor) / 2.0f;

                    if (position < 0)
                    {
                        view.TranslationX = horzMargin - vertMargin / 2;
                    }
                    else
                    {
                        view.TranslationX = -horzMargin + vertMargin / 2;
                    }

                    // Scale the page down (between MIN_SCALE and 1)
                    view.ScaleX = scaleFactor;
                    view.ScaleY = scaleFactor;

                    // Fade the page relative to its size.
                    view.Alpha = (MIN_ALPHA + (scaleFactor - MIN_SCALE) / (1 - MIN_SCALE) * (1 - MIN_ALPHA));

                }
                else
                { // (1,+Infinity]
                    // This page is way off-screen to the right.
                    view.Alpha = 0.0f;
                }
            }
        }

        //
        private sealed class PreloaderTimerTask : TimerTask
        {
            private readonly Timer timer;
            private readonly Action action;

            public PreloaderTimerTask(Timer timer, Action action)
            {
                this.timer = timer;
                this.action = action;
            }

            public override void Run()
            {
                timer.Cancel();
                action.Invoke();
            }
        }
    }
}