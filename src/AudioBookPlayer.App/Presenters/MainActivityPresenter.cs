using Android;
using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.Support.V4.Media;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Core.Internal;
using AudioBookPlayer.App.Views.Fragments;
using AudioBookPlayer.MediaBrowserConnector;
using Google.Android.Material.BottomSheet;
using System;
using FragmentManager = AndroidX.Fragment.App.FragmentManager;
using FragmentTransaction = AndroidX.Fragment.App.FragmentTransaction;
using MediaBrowserServiceConnector = AudioBookPlayer.MediaBrowserConnector.MediaBrowserServiceConnector;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

#nullable enable

namespace AudioBookPlayer.App.Presenters
{
    internal sealed class MainActivityPresenter : MediaBrowserServiceConnector.IConnectCallback, MediaService.IMediaServiceListener, MediaService.IUpdateListener
    {
        private readonly Toast? exitHintToast;
        private readonly NowPlayingPresenter nowPlayingPresenter;
        private AppCompatActivity? activityCompat;
        private Toolbar? toolbar;
        private BottomSheetBehavior? bottomBehavior;
        private Space? bottomSpacer;
        //private TabLayout? tabsLayout;
        //private TabLayoutMediator? layoutMediator;
        //private ViewPager2? viewPager;
        private bool isExitHintShown;

        public FragmentManager? SupportFragmentManager => activityCompat?.SupportFragmentManager;

        public MediaBrowserServiceConnector Connector
        {
            get;
        }

        public MediaService? MediaService
        {
            get;
            private set;
        }

        public MainActivityPresenter()
        {
            nowPlayingPresenter = new NowPlayingPresenter(this);
            exitHintToast = Toast.MakeText(Application.Context, Resource.String.exit_hint, ToastLength.Short);

            if (null != exitHintToast)
            {
                exitHintToast.AddCallback(new ToastCallback(OnExitHintShown, OnExitHintHidden));
            }

            Connector = MediaBrowserServiceConnector.GetInstance();
            MediaService = null;
        }

        public void AttachView(AppCompatActivity activity)
        {
            activityCompat = activity;

            activityCompat.SetContentView(Resource.Layout.activity_main);

            toolbar = activity.FindViewById<Toolbar>(Resource.Id.toolbar);
            bottomSpacer = activity.FindViewById<Space>(Resource.Id.bottom_spacer);
            //viewPager = activity.FindViewById<ViewPager2>(Resource.Id.tabs_pager);
            //tabsLayout = activity.FindViewById<TabLayout>(Resource.Id.tabs_layout);

            nowPlayingPresenter.AttachView(activity);

            //var appBarLayout = activity.FindViewById<AppBarLayout>(Resource.Id.app_bar_layout);
            var bottomContainer = activity.FindViewById<RelativeLayout>(Resource.Id.bottom_navigation_container);
            //var drawerOverlay = activity.FindViewById<FrameLayout>(Resource.Id.drawer_overlay);
            var collapsedDrawer = activity.FindViewById<RelativeLayout>(Resource.Id.collapsed_drawer);
            var expandedDrawer = activity.FindViewById<LinearLayout>(Resource.Id.expanded_drawer);
            var behaviorCallback = new BehaviorCallback(collapsedDrawer, expandedDrawer);
            var peekHeight = TypedValue.ApplyDimension(ComplexUnitType.Dip, 52.0f, activity.Resources?.DisplayMetrics);

            bottomBehavior = BottomSheetBehavior.From(bottomContainer);
            bottomBehavior.AddBottomSheetCallback(behaviorCallback);
            bottomBehavior.SetPeekHeight((int)peekHeight, true);
            bottomBehavior.State = BottomSheetBehavior.StateHidden;

            if (null != bottomContainer)
            {
                bottomContainer.BackgroundTintBlendMode = BlendMode.Src;
                bottomContainer.BackgroundTintMode = PorterDuff.Mode.Overlay;
                //bottomContainer.BackgroundTintList = ColorStateList.ValueOf(Color.Aquamarine);
            }

            if (null != toolbar)
            {
                activity.SetSupportActionBar(toolbar);
                activity.SupportActionBar.SetHomeButtonEnabled(false);
                activity.SupportActionBar.SetDisplayUseLogoEnabled(true);
                activity.SupportActionBar.SetLogo(Resource.Drawable.ic_logo_small);
            }

            /*if (null != viewPager)
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
            }*/

            if (null != SupportFragmentManager)
            {
                var navigation = SupportFragmentManager.BeginTransaction();
                navigation
                    .SetReorderingAllowed(true)
                    .AddToBackStack(null)
                    .Add(Resource.Id.navigation_host, LibraryFragment.NewInstance())
                    .Commit();
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

            if (null != SupportFragmentManager)
            {
                var count = SupportFragmentManager.BackStackEntryCount;

                if (1 < count)
                {
                    SupportFragmentManager.PopBackStack();
                    return;
                }
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
                MediaService.UpdateLibrary(this);
            }

            switch (menuItem.ItemId)
            {
                case Resource.Id.action_bookmark:
                {
                    return true;
                }

                case Resource.Id.action_library_update:
                {
                    if (null != MediaService)
                    {
                        var view = activityCompat?.Window?.DecorView.RootView;
                        var permissions = new[] { Manifest.Permission.ReadExternalStorage };

                        PermissionChecker.CheckPermissions(view, permissions, DoRequestPermissionsResult);
                    }

                    return true;
                }

                case Resource.Id.action_settings:
                {
                    var fragmentManager = activityCompat?.SupportFragmentManager;

                    if (null != fragmentManager)
                    {
                        fragmentManager.BeginTransaction()
                            .SetReorderingAllowed(true)
                            .AddToBackStack(null)
                            .Replace(Resource.Id.navigation_host, SettingsFragment.NewInstance())
                            .SetTransition(FragmentTransaction.TransitFragmentOpen)
                            .Commit();

                        return true;
                    }

                    return false;
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
            MediaService = service;

            if (null != MediaService)
            {
                MediaService.AddListener(this);
                nowPlayingPresenter.AddListener(MediaService);
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

        void MediaService.IMediaServiceListener.OnQueueChanged()
        {
            if (null != MediaService && 0 < MediaService.MediaQueue.Count)
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

        void MediaService.IMediaServiceListener.OnPlaybackStateChanged()
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

        #region BehaviorCallback

        private sealed class BehaviorCallback : BottomSheetBehavior.BottomSheetCallback
        {
            private readonly ViewGroup? collapsedDrawer;
            private readonly ViewGroup? expandedDrawer;
            //private readonly ViewGroup? drawerOverlay;

            public BehaviorCallback(
                ViewGroup? collapsedDrawer,
                ViewGroup? expandedDrawer)
            {
                this.collapsedDrawer = collapsedDrawer;
                this.expandedDrawer = expandedDrawer;
                //this.drawerOverlay = drawerOverlay;
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
                //drawerOverlay.Alpha = slideOffset * slideOffset;

                // Когда оффсет превышает половину, мы скрываем collapsed layout и делаем видимым expanded
                if (0.15f < slideOffset)
                {
                    collapsedDrawer.Visibility = ViewStates.Gone;
                    expandedDrawer.Visibility = ViewStates.Visible;
                    //drawerOverlay.Visibility = ViewStates.Visible;
                }

                // Если же оффсет меньше половины, а expanded layout всё ещё виден, то нужно скрывать его и показывать collapsed
                if (0.15f > slideOffset && ViewStates.Visible == expandedDrawer.Visibility)
                {
                    collapsedDrawer.Visibility = ViewStates.Visible;
                    expandedDrawer.Visibility = ViewStates.Invisible;
                    //drawerOverlay.Visibility = ViewStates.Invisible;
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

        #endregion
    }
}

#nullable restore