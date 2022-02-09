using Android.Graphics;
using Android.OS;
using Android.Support.V4.Media;
using Android.Views;
using Android.Widget;
using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Views.Activities;
using AudioBookPlayer.App.Views.Fragments;
using AudioBookPlayer.Core;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Android.Support.V4.Media.Session;
using Uri = Android.Net.Uri;

#nullable enable

namespace AudioBookPlayer.App.Presenters
{
    internal sealed class NowPlayingPresenter: MediaBrowserServiceConnector.IConnectCallback, MediaBrowserServiceConnector.IMediaServiceObserver
    {
        private readonly string? mediaId;
        private readonly MainActivity mainActivity;
        private ImageButton? chapterSelectionButton;
        private TextView? titleView;
        private TextView? authorsView;
        private ImageView? bookImage;
        private MediaBrowserServiceConnector.IMediaBrowserService? browserService;
        private IDisposable? chapterSelectionButtonSubscription;
        private IList<MediaSessionCompat.QueueItem> queueItems;

        public NowPlayingPresenter(string? mediaId, MainActivity mainActivity)
        {
            this.mediaId = mediaId;
            this.mainActivity = mainActivity;

            chapterSelectionButton = null;
        }

        public void AttachView(View? view)
        {
            chapterSelectionButton = view?.FindViewById<ImageButton>(Resource.Id.chapter_selection_button);

            titleView = view?.FindViewById<TextView>(Resource.Id.book_title);
            authorsView = view?.FindViewById<TextView>(Resource.Id.book_authors);
            bookImage = view?.FindViewById<ImageView>(Resource.Id.book_image);

            //var hint = view.FindViewById<TextView>(Resource.Id.hint_text_1);

            // var preferences = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
            // var flag = preferences.GetBoolean("checkbox_preference", false);
            // System.Diagnostics.Debug.WriteLine($"[NowPlayingFragment] [OnCreateView] Preference Flag: {flag}");

            if (null != chapterSelectionButton)
            {
                chapterSelectionButtonSubscription = Observable.FromEventPattern(
                        handler => chapterSelectionButton.Click += handler,
                        handler => chapterSelectionButton.Click -= handler
                    )
                    .Subscribe(pattern =>
                    {
                        var fragment = ChapterSelectionFragment.NewInstance(queueItems);
                        fragment.Show(mainActivity.SupportFragmentManager, "dialog");
                    });
            }

            var connector = mainActivity.ServiceConnector;

            if (null != connector)
            {
                connector.Connect(this);
                ;
            }
        }

        public void DetachView()
        {
            //imageLoadingTask.ClearQueue();
            //button1ClickSubscription?.Dispose();
        }

        #region MediaBrowserServiceConnector.IConnectCallback

        void MediaBrowserServiceConnector.IConnectCallback.OnConnected(MediaBrowserServiceConnector.IMediaBrowserService service)
        {
            browserService = service;
            var subscription = browserService.Subscribe(this);
            browserService.PrepareFromMediaId(mediaId, Bundle.Empty);
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

        #region MediaBrowserServiceConnector.IMediaServiceObserver

        void MediaBrowserServiceConnector.IMediaServiceObserver.OnMetadataChanged(MediaMetadataCompat metadata)
        {
            if (null != authorsView)
            {
               authorsView.Text = metadata.GetString(MediaMetadataCompat.MetadataKeyAlbumArtist);
            }

            if (null != bookImage)
            {
                var contentUri = metadata.GetString(MediaMetadataCompat.MetadataKeyAlbumArtUri);
                var imageUri = Uri.Parse(contentUri);

                if (null != imageUri)
                {
                    AlbumArt.GetInstance().Fetch(imageUri, bookImage, OnImageLoaded);
                }
            }
        }

        void MediaBrowserServiceConnector.IMediaServiceObserver.OnQueueTitleChanged(string title)
        {
            if (null != titleView)
            {
                titleView.Text = title;
            }
        }

        void MediaBrowserServiceConnector.IMediaServiceObserver.OnQueueChanged(IList<MediaSessionCompat.QueueItem> queue)
        {
            queueItems = queue;
        }

        #endregion

        private static void OnImageLoaded(ImageView imageView, Uri imageUri, Bitmap? _, Bitmap? cover)
        {
            imageView.SetImageBitmap(cover);
        }
    }
}

#nullable restore