#nullable enable

using System;
using System.Collections.Generic;
using Android.OS;
using Android.Support.V4.Media;
using AudioBookPlayer.App.Core;

namespace AudioBookPlayer.App.ViewModels
{
    internal sealed class AllBooksViewModel : MediaBrowserServiceConnector.IConnectCallback, MediaBrowserServiceConnector.IAudioBooksCallback
    {
        private static AllBooksViewModel? instance;

        public ObservableList<AudioBookViewModel> BookItems
        {
            get;
        }

        public MediaBrowserServiceConnector.IMediaBrowserService? BrowserService
        {
            get;
            private set;
        }

        public bool HasBookItems => false;

        private AllBooksViewModel()
        {
            BookItems = new ObservableList<AudioBookViewModel>();
            BrowserService = null;
        }

        public static AllBooksViewModel Instance()
        {
            if (null == instance)
            {
                instance = new AllBooksViewModel();
            }

            return instance;
        }

        #region MediaBrowserServiceConnector.IConnectCallback

        void MediaBrowserServiceConnector.IConnectCallback.OnConnected(MediaBrowserServiceConnector.IMediaBrowserService service)
        {
            BrowserService = service;
            BrowserService.GetAudioBooks(this);
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

        #region MediaBrowserServiceConnector.IAudioBooksResultCallback

        void MediaBrowserServiceConnector.IAudioBooksCallback.OnAudioBooksReady(IList<MediaBrowserCompat.MediaItem> list, Bundle options)
        {
            BookItems.Clear();

            for (var index = 0; index < list.Count; index++)
            {
                var book = list[index];
                BookItems.Add(AudioBookViewModel.From(book));
            }

            //ViewModel.SetBookItems(list);
            //BooksAdapter.SetItems(list);
        }

        void MediaBrowserServiceConnector.IAudioBooksCallback.OnAudioBooksError(Bundle options)
        {
            ;
        }

        #endregion
    }
}