using Android.OS;
using Android.Support.V4.Media;
using Android.Views;
using AndroidX.Fragment.App;
using AndroidX.ViewPager2.Adapter;
using AndroidX.ViewPager2.Widget;
using AudioBookPlayer.App.Views.Fragments;
using AudioBookPlayer.MediaBrowserConnector;
using Google.Android.Material.Tabs;
using System;
using System.Collections.Generic;

#nullable enable

namespace AudioBookPlayer.App.Presenters
{
    internal sealed class LibraryPresenter : MediaBrowserServiceConnector.IConnectCallback, MediaService.IAudioBooksListener
    {
        private readonly MediaBrowserServiceConnector connector;
        private readonly FragmentActivity activity;
        private MediaService? mediaService;
        private TabLayout? tabsLayout;
        private ViewPager2? viewPager;
        private TabLayoutMediator? layoutMediator;

        public LibraryPresenter(FragmentActivity activity)
        {
            this.activity = activity;
            connector = MediaBrowserServiceConnector.GetInstance();
        }

        public void AttachView(View? view)
        {
            if (null == view)
            {
                return;
            }

            viewPager = view.FindViewById<ViewPager2>(Resource.Id.tabs_pager);
            tabsLayout = view.FindViewById<TabLayout>(Resource.Id.tabs_layout);

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

            connector.Connect(this);
        }

        public void DetachView()
        {
            connector.Disconnect(this);
        }

        #region MediaBrowserServiceConnector.IConnectCallback

        void MediaBrowserServiceConnector.IConnectCallback.OnConnected(MediaService service)
        {
            mediaService = service;

            if (null != mediaService)
            {
                mediaService.GetAudioBooks(this);
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


        #region MediaService.IAudioBooksListener

        void MediaService.IAudioBooksListener.OnReady(IList<MediaBrowserCompat.MediaItem> list, Bundle options)
        {
            ;
        }

        void MediaService.IAudioBooksListener.OnError(Bundle options)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region ViewsAdapter

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

        #endregion

        #region TabLayoutConfigurator

        private sealed class TabLayoutConfigurator : Java.Lang.Object, TabLayoutMediator.ITabConfigurationStrategy
        {
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

        #endregion
    }
}

#nullable restore