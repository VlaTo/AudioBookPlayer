using Android.OS;
using Android.Support.V4.Media;
using Android.Views;
using Android.Widget;
using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Models;
using AudioBookPlayer.App.Views;
using AudioBookPlayer.App.Views.Activities;
using AudioBookPlayer.App.Views.Fragments;
using System;
using System.Collections.Generic;
using FragmentManager = AndroidX.Fragment.App.FragmentManager;
using FragmentTransaction = AndroidX.Fragment.App.FragmentTransaction;

#nullable enable

namespace AudioBookPlayer.App.Presenters
{
    internal abstract class BooksPresenter : MediaBrowserServiceConnector.IConnectCallback, MediaBrowserServiceConnector.IAudioBooksCallback
    {
        protected MainActivity MainActivity
        {
            get;
        }

        protected MediaBrowserServiceConnector? ServiceConnector => MainActivity.ServiceConnector;

        protected BooksListAdapter? Adapter
        {
            get;
            private set;
        }

        protected MediaBrowserServiceConnector.IMediaBrowserService? BrowserService
        {
            get;
            private set;
        }

        protected ListView? ListView
        {
            get;
            private set;
        }

        protected BooksPresenter(MainActivity mainActivity)
        {
            MainActivity = mainActivity;

            BrowserService = null;
            ListView = null;
            Adapter = null;
        }

        public virtual void AttachView(View? view)
        {
            ListView = view?.FindViewById<ListView>(Resource.Id.books_list);

            if (null != ListView)
            {
                var adapter = CreateBookListAdapter();

                if (null != adapter)
                {
                    Adapter = adapter;
                }

                ListView.EmptyView = view?.FindViewById<ViewStub>(Resource.Id.empty_books_list);
                ListView.OnItemClickListener = new ItemClickListener(MainActivity.SupportFragmentManager);
                ListView.Adapter = Adapter;
            }

            ServiceConnector?.Connect(this);
        }

        public virtual void DetachView()
        {
            Adapter?.Detach();
        }

        #region MediaBrowserServiceConnector.IConnectCallback

        void MediaBrowserServiceConnector.IConnectCallback.OnConnected(MediaBrowserServiceConnector.IMediaBrowserService service)
        {
            BrowserService = service;
            BrowserService.GetAudioBooks(this);
            
            DoBrowserServiceConnected();
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

        #region MediaBrowserServiceConnector.IAudioBooksCallback

        void MediaBrowserServiceConnector.IAudioBooksCallback.OnAudioBooksReady(IList<MediaBrowserCompat.MediaItem> mediaItems, Bundle options)
        {
            if (null == Adapter)
            {
                return;
            }

            var books = new List<BookItem>();

            for (var index = 0; index < mediaItems.Count; index++)
            {
                var mediaItem = mediaItems[index];

                if (mediaItem.IsBrowsable)
                {
                    books.Add(BookItem.From(mediaItem));
                }
            }

            Adapter.Clear();
            Adapter.AddRange(books);
        }

        void MediaBrowserServiceConnector.IAudioBooksCallback.OnAudioBooksError(Bundle options)
        {
            throw new NotImplementedException();
        }

        #endregion

        protected virtual void DoBrowserServiceConnected()
        {
            ;
        }

        protected abstract BooksListAdapter? CreateBookListAdapter();

        protected sealed class ItemClickListener : Java.Lang.Object, AdapterView.IOnItemClickListener
        {
            private readonly FragmentManager fragmentManager;

            public ItemClickListener(FragmentManager fragmentManager)
            {
                this.fragmentManager = fragmentManager;
            }

            public void OnItemClick(AdapterView? parent, View? view, int position, long id)
            {
                var item = (BookItem?)parent?.GetItemAtPosition(position);

                if (null != item)
                {
                    var fragment = NowPlayingFragment.NewInstance(item.MediaId);

                    fragmentManager
                        .BeginTransaction()
                        .Replace(Resource.Id.nav_host_frame, fragment)
                        .AddToBackStack(null)
                        .SetTransition(FragmentTransaction.TransitFragmentOpen)
                        .Commit();
                }
            }
        }
    }
}

#nullable restore