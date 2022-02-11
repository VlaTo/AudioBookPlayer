using Android.Graphics;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using Android.Widget;
using AndroidX.AppCompat.App;
using AudioBookPlayer.Core;
using AudioBookPlayer.MediaBrowserConnector;
using System.Collections.Generic;
using Android.Views;
using AudioBookPlayer.App.Core.Internal;
using Uri = Android.Net.Uri;

#nullable enable

namespace AudioBookPlayer.App.Presenters
{
    internal sealed class NowPlayingPresenter : MediaService.IMediaServiceListener
    {
        private readonly MainActivityPresenter ownerPresenter;
        private TextView? collapsedBookTitle;
        private TextView? collapsedChapterTitle;
        private TextView? expandedBookTitle;
        private TextView? expandedBookAuthor;
        private ImageView? coverImage;
        private ImageButton? playButton;
        private ImageButton? playPauseButton;
        private ImageButton? chapterSelectionButton;
        private ImageButton? rewindButton;
        private ImageButton? fastForwardButton;

        public NowPlayingPresenter(MainActivityPresenter ownerPresenter)
        {
            this.ownerPresenter = ownerPresenter;
        }

        public void AttachView(AppCompatActivity activity)
        {
            playPauseButton = activity.FindViewById<ImageButton>(Resource.Id.play_pause_button);
            chapterSelectionButton = activity.FindViewById<ImageButton>(Resource.Id.chapter_selection_button);
            collapsedBookTitle = activity.FindViewById<TextView>(Resource.Id.collapsed_book_title);
            collapsedChapterTitle = activity.FindViewById<TextView>(Resource.Id.collapsed_chapter_title);
            expandedBookTitle = activity.FindViewById<TextView>(Resource.Id.expanded_book_title);
            expandedBookAuthor = activity.FindViewById<TextView>(Resource.Id.expanded_book_author);
            coverImage = activity.FindViewById<ImageView>(Resource.Id.book_cover_image);
            
            rewindButton = activity.FindViewById<ImageButton>(Resource.Id.rewind_button);
            playButton = activity.FindViewById<ImageButton>(Resource.Id.play_button);
            fastForwardButton = activity.FindViewById<ImageButton>(Resource.Id.fast_forward_button);

            if (null != playPauseButton)
            {
                var listener = ClickListener.Create(OnStartPlayButtonClick);
                playPauseButton.SetOnClickListener(listener);
            }

            // playback controls
            if (null != rewindButton)
            {
                var listener = ClickListener.Create(OnStartPlayButtonClick);
                rewindButton.SetOnClickListener(listener);
            }

            if (null != playButton)
            {
                var listener = ClickListener.Create(OnStartPlayButtonClick);
                playButton.SetOnClickListener(listener);
            }

            if (null != fastForwardButton)
            {
                var listener = ClickListener.Create(OnStartPlayButtonClick);
                fastForwardButton.SetOnClickListener(listener);
            }
        }

        public void DetachView()
        {
            //imageLoadingTask.ClearQueue();
            //button1ClickSubscription?.Dispose();
        }

        private void OnStartPlayButtonClick(View? button)
        {
            //playbackToast?.Show();
        }

        #region MediaBrowserServiceConnector.IMediaServiceObserver

        void MediaService.IMediaServiceListener.OnMetadataChanged(MediaMetadataCompat metadata)
        {
            if (null != expandedBookAuthor)
            {
                expandedBookAuthor.Text = metadata.GetString(MediaMetadataCompat.MetadataKeyAlbumArtist);
            }

            if (null != coverImage)
            {
                var contentUri = metadata.GetString(MediaMetadataCompat.MetadataKeyAlbumArtUri);
                var imageUri = Uri.Parse(contentUri);

                if (null != imageUri)
                {
                    AlbumArt.GetInstance().Fetch(imageUri, coverImage, OnImageLoaded);
                }
            }
        }

        void MediaService.IMediaServiceListener.OnQueueTitleChanged(string title)
        {
            if (null != collapsedBookTitle)
            {
                collapsedBookTitle.Text = title;
            }

            if (null != expandedBookTitle)
            {
                expandedBookTitle.Text = title;
            }
        }

        void MediaService.IMediaServiceListener.OnQueueChanged(IList<MediaSessionCompat.QueueItem> queue)
        {
            if (0 < queue.Count)
            {
                if (null != collapsedChapterTitle)
                {
                    collapsedChapterTitle.Text = queue[0].Description.Title;
                }
            }
        }

        #endregion

        private static void OnImageLoaded(ImageView imageView, Uri imageUri, Bitmap? _, Bitmap? cover)
        {
            imageView.SetImageBitmap(cover);
        }
    }
}

#nullable restore