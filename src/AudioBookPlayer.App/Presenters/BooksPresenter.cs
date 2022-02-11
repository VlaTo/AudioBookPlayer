using Android.OS;
using Android.Support.V4.Media;
using Android.Views;
using Android.Widget;
using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Models;
using AudioBookPlayer.App.Views;
using AudioBookPlayer.App.Views.Activities;
using System;
using System.Collections.Generic;
using AudioBookPlayer.MediaBrowserConnector;
using MediaBrowserServiceConnector = AudioBookPlayer.MediaBrowserConnector.MediaBrowserServiceConnector;

#nullable enable

namespace AudioBookPlayer.App.Presenters
{
    internal abstract class BooksPresenter : MediaBrowserServiceConnector.IConnectCallback, MediaService.IAudioBooksListener
    {
        protected MainActivity MainActivity
        {
            get;
        }

        public MediaBrowserServiceConnector? Connector
        {
            get;
        }

        protected BooksListAdapter? Adapter
        {
            get;
            private set;
        }

        protected MediaService? BrowserService
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
            Connector = MediaBrowserServiceConnector.GetInstance();

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
                ListView.Adapter = Adapter;
            }

            Connector?.Connect(this);
        }

        public virtual void DetachView()
        {
            Adapter?.Detach();
        }

        protected abstract BooksListAdapter? CreateBookListAdapter();

        #region MediaBrowserServiceConnector.IConnectCallback

        void MediaBrowserServiceConnector.IConnectCallback.OnConnected(MediaService service)
        {
            BrowserService = service;
            BrowserService.GetAudioBooks(this);

            if (null != ListView)
            {
                ListView.OnItemClickListener = new ItemClickListener(BrowserService);
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

        #region MediaBrowserServiceConnector.IAudioBooksCallback

        void MediaService.IAudioBooksListener.OnReady(IList<MediaBrowserCompat.MediaItem> mediaItems, Bundle options)
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

        void MediaService.IAudioBooksListener.OnError(Bundle options)
        {
            throw new NotImplementedException();
        }

        #endregion

        protected sealed class ItemClickListener : Java.Lang.Object, AdapterView.IOnItemClickListener
        {
            private readonly MediaService browserService;

            public ItemClickListener(MediaService browserService)
            {
                this.browserService = browserService;
            }

            public void OnItemClick(AdapterView? parent, View? view, int position, long id)
            {
                var item = (BookItem?)parent?.GetItemAtPosition(position);

                if (null != item)
                {
                    browserService.PrepareFromMediaId(item.MediaId, Bundle.Empty);
                }
            }
        }
    }
}

#nullable restore