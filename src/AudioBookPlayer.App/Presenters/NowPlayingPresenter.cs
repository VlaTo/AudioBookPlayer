using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V4.Media;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Palette.Graphics;
using AudioBookPlayer.App.Core.Internal;
using AudioBookPlayer.App.Views.Fragments;
using AudioBookPlayer.Core;
using AudioBookPlayer.MediaBrowserConnector;
using Uri = Android.Net.Uri;

#nullable enable

namespace AudioBookPlayer.App.Presenters
{
    internal sealed class NowPlayingPresenter : MediaService.IMediaServiceListener
    {
        private readonly MainActivityPresenter ownerPresenter;
        private MediaService? mediaService;
        private RelativeLayout? containerLayout;
        private TextView? collapsedBookTitle;
        private TextView? collapsedChapterTitle;
        private TextView? expandedBookTitle;
        private TextView? expandedBookAuthor;
        private TextView? expandedChapterTitle;
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
            containerLayout = activity.FindViewById<RelativeLayout>(Resource.Id.bottom_navigation_container);
            playPauseButton = activity.FindViewById<ImageButton>(Resource.Id.play_pause_button);
            chapterSelectionButton = activity.FindViewById<ImageButton>(Resource.Id.chapter_selection_button);
            collapsedBookTitle = activity.FindViewById<TextView>(Resource.Id.collapsed_book_title);
            collapsedChapterTitle = activity.FindViewById<TextView>(Resource.Id.collapsed_chapter_title);
            expandedBookTitle = activity.FindViewById<TextView>(Resource.Id.expanded_book_title);
            expandedBookAuthor = activity.FindViewById<TextView>(Resource.Id.expanded_book_author);
            coverImage = activity.FindViewById<ImageView>(Resource.Id.book_cover_image);
            expandedChapterTitle = activity.FindViewById<TextView>(Resource.Id.chapter_title);

            rewindButton = activity.FindViewById<ImageButton>(Resource.Id.rewind_button);
            playButton = activity.FindViewById<ImageButton>(Resource.Id.play_button);
            fastForwardButton = activity.FindViewById<ImageButton>(Resource.Id.fast_forward_button);

            if (null != chapterSelectionButton)
            {
                var listener = ClickListener.Create(OnChapterSelectionButtonClick);
                chapterSelectionButton.SetOnClickListener(listener);
            }

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

        public void AddListener(MediaService service)
        {
            mediaService = service;
            mediaService.AddListener(this);
        }

        private void OnChapterSelectionButtonClick(View? button)
        {
            var dialog = ChapterDialogFragment.NewInstance();

            dialog.Show(ownerPresenter.SupportFragmentManager, null);
        }

        private void OnStartPlayButtonClick(View? button)
        {
            //playbackToast?.Show();
        }

        #region MediaService.IMediaServiceListener

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

        void MediaService.IMediaServiceListener.OnQueueChanged()
        {
            ;
        }

        void MediaService.IMediaServiceListener.OnPlaybackStateChanged()
        {
            if (null != mediaService && 0 < mediaService.MediaQueue.Count)
            {
                var queue = mediaService.MediaQueue;

                for (var index = 0; index < queue.Count; index++)
                {
                    var queueItem = queue[index];

                    if (queueItem.QueueId == mediaService.ActiveQueueItemId)
                    {
                        var description = queueItem.Description;

                        if (null != collapsedChapterTitle)
                        {
                            collapsedChapterTitle.Text = description.Title;
                        }

                        if (null != expandedChapterTitle)
                        {
                            expandedChapterTitle.Text = description.Title;
                        }

                        break;
                    }
                }
            }
        }

        #endregion

        private void OnImageLoaded(ImageView imageView, Uri imageUri, Bitmap? _, Bitmap? cover)
        {
            imageView.SetImageBitmap(cover);

            if (null != containerLayout)
            {
                var builder = new Palette.Builder(cover);
                var listener = new PaletteListener(containerLayout, expandedBookTitle, expandedChapterTitle);

                builder.Generate(listener);
            }
        }

        private sealed class PaletteListener : Java.Lang.Object, Palette.IPaletteAsyncListener
        {
            private readonly RelativeLayout layout;
            private readonly TextView? titleView;
            private readonly TextView? descriptionView;

            public PaletteListener(RelativeLayout layout, TextView? titleView, TextView? descriptionView)
            {
                this.layout = layout;
                this.titleView = titleView;
                this.descriptionView = descriptionView;
            }

            public void OnGenerated(Palette palette)
            {
                layout.Background = new ColorDrawable(new Color(palette.DarkMutedSwatch.Rgb));
                titleView?.SetTextColor(new Color(palette.MutedSwatch.TitleTextColor));
                descriptionView?.SetTextColor(new Color(palette.MutedSwatch.BodyTextColor));
            }
        }
    }
}

#nullable restore