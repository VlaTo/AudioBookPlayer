using Android;
using Android.App;
using Android.Content.PM;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Fragment.App;
using AndroidX.ViewPager2.Adapter;
using AndroidX.ViewPager2.Widget;
using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Core.Internal;
using AudioBookPlayer.App.Views.Fragments;
using AudioBookPlayer.MediaBrowserConnector;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.Tabs;
using System;
using System.Collections.Generic;
using Fragment = AndroidX.Fragment.App.Fragment;
using FragmentManager = AndroidX.Fragment.App.FragmentManager;
using MediaBrowserServiceConnector = AudioBookPlayer.MediaBrowserConnector.MediaBrowserServiceConnector;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

#nullable enable

namespace AudioBookPlayer.App.Presenters
{
    internal sealed class MainActivityPresenter
        : MediaBrowserServiceConnector.IConnectCallback, 
        MediaService.IMediaServiceListener, 
        MediaService.IUpdateListener
    {
        private readonly Toast? settingsToast;
        //private readonly Toast? playbackToast;
        private readonly Toast? exitHintToast;
        private readonly NowPlayingPresenter nowPlayingPresenter;
        private AppCompatActivity? activityCompat;
        private MediaService? browserService;
        private Toolbar? toolbar;
        private BottomSheetBehavior? bottomBehavior;
        private Space? bottomSpacer;
        private TabLayout? tabsLayout;
        private TabLayoutMediator? layoutMediator;
        private ViewPager2? viewPager;
        private bool isExitHintShown;

        public MediaBrowserServiceConnector Connector
        {
            get;
        }

        public FragmentManager? SupportFragmentManager => activityCompat?.SupportFragmentManager;

        public MainActivityPresenter()
        {
            Connector = MediaBrowserServiceConnector.GetInstance();

            nowPlayingPresenter = new NowPlayingPresenter(this);
            settingsToast = Toast.MakeText(Application.Context, "Settings was selected", ToastLength.Short);
            //playbackToast = Toast.MakeText(Application.Context, "You started a playback", ToastLength.Short);
            
            exitHintToast = Toast.MakeText(Application.Context, Resource.String.exit_hint, ToastLength.Short);

            if (null != exitHintToast)
            {
                exitHintToast.AddCallback(new ToastCallback(OnExitHintShown, OnExitHintHidden));
            }
        }

        public void AttachView(AppCompatActivity activity)
        {
            activityCompat = activity;

            activityCompat.SetContentView(Resource.Layout.activity_main);

            toolbar = activity.FindViewById<Toolbar>(Resource.Id.toolbar);
            bottomSpacer = activity.FindViewById<Space>(Resource.Id.bottom_spacer);
            viewPager = activity.FindViewById<ViewPager2>(Resource.Id.tabs_pager);
            tabsLayout = activity.FindViewById<TabLayout>(Resource.Id.tabs_layout);

            nowPlayingPresenter.AttachView(activity);

            var bottomContainer = activity.FindViewById<RelativeLayout>(Resource.Id.bottom_navigation_container);
            var density = activity.Resources.DisplayMetrics.Density;

            bottomBehavior = BottomSheetBehavior.From(bottomContainer);
            bottomBehavior.PeekHeight = Convert.ToInt32(52 * density);

            var drawerOverlay = activity.FindViewById<FrameLayout>(Resource.Id.drawer_overlay);
            var collapsedDrawer = activity.FindViewById<RelativeLayout>(Resource.Id.collapsed_drawer);
            var expandedDrawer = activity.FindViewById<LinearLayout>(Resource.Id.expanded_drawer);
            var behaviorCallback = new BehaviorCallback(collapsedDrawer, expandedDrawer, drawerOverlay);

            bottomBehavior.AddBottomSheetCallback(behaviorCallback);
            bottomBehavior.State = BottomSheetBehavior.StateHidden;

            if (null != toolbar)
            {
                activity.SetSupportActionBar(toolbar);
                activity.SupportActionBar.SetHomeButtonEnabled(false);
                activity.SupportActionBar.SetDisplayUseLogoEnabled(true);
                activity.SupportActionBar.SetLogo(Resource.Drawable.ic_logo_small);
            }

            if (null != viewPager)
            {
                 viewPager.Adapter = new ViewsAdapter(
                     activity,
                     AllBooksFragment.NewInstance,
                     RecentBooksFragment.NewInstance
                 );
            }

            if (null != tabsLayout)
            {
                var configurator = new TabLayoutConfigurator();

                layoutMediator = new TabLayoutMediator(tabsLayout, viewPager, true, true, configurator);
                layoutMediator.Attach();
            }

            Connector.Connect(this);
        }

        public void DetachView()
        {
            nowPlayingPresenter.DetachView();
        }

        public void OnBackPressed()
        {
            if (bottomBehavior is { State: BottomSheetBehavior.StateExpanded })
            {
                bottomBehavior.State = BottomSheetBehavior.StateCollapsed;
                return;
            }

            if (isExitHintShown)
            {
                exitHintToast?.Cancel();
                activityCompat?.Finish();
            }
            else
            {
                exitHintToast?.Show();
            }
        }

        public bool OnOptionsItemSelected(IMenuItem menuItem)
        {
            void DoRequestPermissionsResult(int arg1, string[] arg2, Permission[] arg3)
            {
                browserService.UpdateLibrary(this);
            }

            switch (menuItem.ItemId)
            {
                case Resource.Id.action_bookmark:
                {
                    return true;
                }

                case Resource.Id.action_library_update:
                {
                    if (null != browserService)
                    {
                        var view = activityCompat?.Window?.DecorView.RootView;
                        var permissions = new[] { Manifest.Permission.ReadExternalStorage };

                        PermissionChecker.CheckPermissions(view, permissions, DoRequestPermissionsResult);
                    }

                    return true;
                }

                case Resource.Id.action_settings:
                {
                    settingsToast?.Show();
                    return true;
                }
            }

            return false;
        }

        private void OnExitHintShown()
        {
            isExitHintShown = true;
        }

        private void OnExitHintHidden()
        {
            isExitHintShown = false;
        }

        #region MediaBrowserServiceConnector.IConnectCallback

        void MediaBrowserServiceConnector.IConnectCallback.OnConnected(MediaService service)
        {
            browserService = service;

            if (null != browserService)
            {
                browserService.AddListener(this);
                browserService.AddListener(nowPlayingPresenter);
            }
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

        #region MediaService.IMediaServiceObserver

        void MediaService.IMediaServiceListener.OnQueueTitleChanged(string title)
        {
            ;
        }

        void MediaService.IMediaServiceListener.OnQueueChanged(IList<MediaSessionCompat.QueueItem> queue)
        {
            if (0 < queue.Count)
            {
                if (null != bottomSpacer)
                {
                    bottomSpacer.Visibility = ViewStates.Visible;
                }

                if (null != bottomBehavior)
                {
                    bottomBehavior.State = BottomSheetBehavior.StateCollapsed;
                }
            }
            else
            {
                if (null != bottomSpacer)
                {
                    bottomSpacer.Visibility = ViewStates.Invisible;
                }

                if (null != bottomBehavior)
                {
                    bottomBehavior.State = BottomSheetBehavior.StateHidden;
                }
            }
        }

        void MediaService.IMediaServiceListener.OnMetadataChanged(MediaMetadataCompat metadata)
        {
            ;
        }

        #endregion

        #region MediaService.IUpdateListener

        void MediaService.IUpdateListener.OnUpdateProgress(int step, float progress)
        {
            ;
        }

        void MediaService.IUpdateListener.OnUpdateResult()
        {
            ;
        }

        #endregion

        private sealed class BehaviorCallback : BottomSheetBehavior.BottomSheetCallback
        {
            private readonly ViewGroup? collapsedDrawer;
            private readonly ViewGroup? expandedDrawer;
            private readonly ViewGroup? drawerOverlay;

            public BehaviorCallback(
                ViewGroup? collapsedDrawer,
                ViewGroup? expandedDrawer,
                ViewGroup? drawerOverlay)
            {
                this.collapsedDrawer = collapsedDrawer;
                this.expandedDrawer = expandedDrawer;
                this.drawerOverlay = drawerOverlay;
            }

            public override void OnSlide(View _, float slideOffset)
            {
                // do stuff during the actual drag event for example
                // animating a background color change based on the offset

                // or for example hidding or showing a fab
                if (0.0f > slideOffset)
                {
                    return;
                }

                collapsedDrawer.Alpha = 1 - 2 * slideOffset;
                expandedDrawer.Alpha = slideOffset * slideOffset;
                drawerOverlay.Alpha = slideOffset * slideOffset;

                // Когда оффсет превышает половину, мы скрываем collapsed layout и делаем видимым expanded
                if (0.15f < slideOffset)
                {
                    collapsedDrawer.Visibility = ViewStates.Gone;
                    expandedDrawer.Visibility = ViewStates.Visible;
                    drawerOverlay.Visibility = ViewStates.Visible;
                }

                // Если же оффсет меньше половины, а expanded layout всё ещё виден, то нужно скрывать его и показывать collapsed
                if (0.15f > slideOffset && ViewStates.Visible == expandedDrawer.Visibility)
                {
                    collapsedDrawer.Visibility = ViewStates.Visible;
                    expandedDrawer.Visibility = ViewStates.Invisible;
                    drawerOverlay.Visibility = ViewStates.Invisible;
                }
            }

            public override void OnStateChanged(View p0, int p1)
            {
                if (BottomSheetBehavior.StateExpanded == p1)
                {
                    // do stuff when the drawer is expanded
                }

                if (BottomSheetBehavior.StateCollapsed == p1)
                {
                    // do stuff when the drawer is collapsed
                }
            }
        }

        private sealed class ViewsAdapter : FragmentStateAdapter
        {
            private readonly Func<Fragment>[] builders;

            public override int ItemCount => builders.Length;

            public ViewsAdapter(FragmentActivity activity, params Func<Fragment>[] builders)
                : base(activity)
            {
                this.builders = builders;
            }

            public override Fragment CreateFragment(int index) => builders[index].Invoke();
        }

        private sealed class TabLayoutConfigurator : Java.Lang.Object, TabLayoutMediator.ITabConfigurationStrategy
        {
            public TabLayoutConfigurator()
            {
            }

            public void OnConfigureTab(TabLayout.Tab p0, int p1)
            {
                var resourceIds = new[]
                {
                    Resource.String.tab_all_books,
                    Resource.String.tab_recent_books
                };

                p0.SetText(resourceIds[p1]);
            }
        }
    }
}

#nullable restore