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

        private MediaBrowserServiceConnector.IMediaBrowserService? browserService;

        public ObservableList<AudioBookViewModel> BookItems
        {
            get;
        }

        public bool HasBookItems => false;

        private AllBooksViewModel()
        {
            BookItems = new ObservableList<AudioBookViewModel>();
            browserService = null;
        }

        public static AllBooksViewModel Instance()
        {
            if (null == instance)
            {
                instance = new AllBooksViewModel();
            }

            return instance;
        }

        public MediaBrowserServiceConnector.IMediaBrowserService? GetBrowserService() => browserService;

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

        #region MediaBrowserServiceConnector.IAudioBooksResultCallback

        void MediaBrowserServiceConnector.IAudioBooksCallback.OnAudioBooksReady(IList<MediaBrowserCompat.MediaItem> mediaItems, Bundle options)
        {
            var books = new List<AudioBookViewModel>();
            var builder = new AudioBookViewModelBuilder();

            for (var index = 0; index < mediaItems.Count; index++)
            {
                var mediaItem = mediaItems[index];

                if (mediaItem.IsBrowsable)
                {
                    books.Add(builder.Create(mediaItem));
                }
            }

            BookItems.Clear();
            BookItems.AddRange(books);
        }

        void MediaBrowserServiceConnector.IAudioBooksCallback.OnAudioBooksError(Bundle options)
        {
            ;
        }

        #endregion
    }
}