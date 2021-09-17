using Android.App;
using Android.Content;
using Android.Media.Session;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using AndroidX.Media;
using AudioBookPlayer.App.Android.Core;
using AudioBookPlayer.App.Android.Services.Builders;
using AudioBookPlayer.App.Domain.Services;
using System;
using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Persistence.LiteDb;
using Application = Android.App.Application;

// https://developer.android.com/guide/topics/media/media-controls
// https://github.com/android/uamp/blob/f60b902643407ba234a316abe91410da7c08a4af/common/src/main/java/com/example/android/uamp/media/MusicService.kt
// https://github.com/xamarin/monodroid-samples/tree/master/android5.0/MediaBrowserService

namespace AudioBookPlayer.App.Android.Services
{
    /// <summary>
    /// Provides audiobooks library and handles playing them.
    /// </summary>
    [Service(Enabled = true, Exported = true)]
    [IntentFilter(new []{ ServiceInterface })]
    public sealed class AudioBookMediaBrowserService : MediaBrowserServiceCompat, IAudioBooksMediaBrowserService
    {
        private readonly ICoverService coverService;
        private readonly ComponentName componentName;
        private readonly PackageValidator packageValidator;
        private MediaSessionCompat mediaSession;
        private MediaControllerCompat mediaController;
        private PlaybackStateCompat.Builder playbackState;
        private MediaSessionCallback mediaSessionCallback;
        private MediaControllerCallback mediaControllerCallback;
        private BooksService booksService;

        // ReSharper disable once UnusedMember.Global
        public AudioBookMediaBrowserService()
            : this(
                AudioBookPlayerApplication.Instance.DependencyContainer.GetInstance<ICoverService>()
            )
        {
        }

        private AudioBookMediaBrowserService(ICoverService coverService)
        {
            this.coverService = coverService;

            componentName = new ComponentName(Application.Context, Class);
            packageValidator = new PackageValidator(Application.Context);
        }

        public override void OnCreate()
        {
            base.OnCreate();

            var intent = PackageManager.GetLaunchIntentForPackage(componentName.PackageName);
            var pendingIntent = PendingIntent.GetActivity(Application.Context, 0, intent, PendingIntentFlags.UpdateCurrent);

            mediaSession = new MediaSessionCompat(Application.Context, nameof(AudioBookMediaBrowserService), componentName, pendingIntent);
            mediaSession.SetFlags((int)(MediaSessionFlags.HandlesMediaButtons | MediaSessionFlags.HandlesTransportControls));

            playbackState = new PlaybackStateCompat.Builder().SetActions(PlaybackStateCompat.ActionPlay | PlaybackStateCompat.ActionPlayPause);
            mediaSessionCallback = new MediaSessionCallback(this, mediaSession);

            mediaSession.SetPlaybackState(playbackState.Build());
            mediaSession.SetCallback(mediaSessionCallback);

            SessionToken = mediaSession.SessionToken;

            mediaController = new MediaControllerCompat(Application.Context, mediaSession);
            mediaControllerCallback = new MediaControllerCallback();
            mediaController.RegisterCallback(mediaControllerCallback);

            var dbContext = new LiteDbContext(new DatabasePathProvider());

            booksService = new BooksService(dbContext, coverService);

            // notificationBuilder = new NotificationBuilder();
            // notificationManager = NotificationManagerCompat.From(Application.Context);

        }

        public override BrowserRoot OnGetRoot(string clientPackageName, int clientUid, Bundle rootHints)
        {
            if (packageValidator.IsCallerAllowed(clientPackageName, clientUid))
            {
                if (null == rootHints || rootHints.IsEmpty)
                {
                    return new BrowserRoot(IAudioBooksMediaBrowserService.LibraryRoot, null);
                }

                if (rootHints.GetBoolean(BrowserRoot.ExtraRecent))
                {
                    var bundle = new Bundle();

                    bundle.PutBoolean(BrowserRoot.ExtraRecent, true);
                    bundle.PutBoolean("android.media.browse.SEARCH_SUPPORTED", true);

                    return new BrowserRoot("__RECENT__", bundle);
                }
            }

            return new BrowserRoot("@empty@", null);
        }

        // Root: "/"
        // Concrete book: "/audiobook:1" (preview)
        // Book chapters: "/audiobook:1/chapters" (list of chapters)
        // Book sections: "/audiobook:1/sections" (list of sections)
        // Concrete section: "/audiobook:1/section:0" (preview)
        // Section chapters: "/audiobook:1/section:0/chapters" (list of chapters)
        // Concrete chapter: "/audiobook:1/chapter:0" (preview)
        public override void OnLoadChildren(string parentId, Result result)
        {
            //System.Diagnostics.Debug.WriteLine($"[{nameof(AudioBookPlaybackService)}.OnLoadChildren] parent id: \"{parentId}\"");

            if (String.Equals(parentId, IAudioBooksMediaBrowserService.LibraryRoot))
            {
                var books = booksService.QueryBooks();
                var builder = new PublicMediaItemBuilder();
                var list = new JavaList<MediaBrowserCompat.MediaItem>();
                var flags = MediaBrowserCompat.MediaItem.FlagBrowsable | MediaBrowserCompat.MediaItem.FlagPlayable;

                for (var index = 0; index < books.Count; index++)
                {
                    var item = builder.BuildBookPreviewMediaItem(books[index], flags);
                    list.Add(item);
                }

                result.SendResult(list);

                return;
            }

            if (parentId.StartsWith(IAudioBooksMediaBrowserService.LibraryRoot))
            {
                var path = parentId.Substring(IAudioBooksMediaBrowserService.LibraryRoot.Length);

                if (MediaBookId.TryParse(path, out var mediaBookId))
                {
                    var book = booksService.GetBook(mediaBookId.Id);
                    var builder = new PublicMediaItemBuilder();
                    var list = new JavaList<MediaBrowserCompat.MediaItem>();

                    for (var index = 0; index < book.Chapters.Count; index++)
                    {
                        var item = builder.BuildMediaItemFrom(
                            book.Chapters[index],
                            MediaBrowserCompat.MediaItem.FlagBrowsable | MediaBrowserCompat.MediaItem.FlagPlayable
                        );
                        list.Add(item);
                    }
                }

            }

            result.SendResult(null);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            mediaSession.Active = false;
            mediaSession.Dispose();
        }

        // MediaSessionCallback
        private sealed class MediaSessionCallback : MediaSessionCompat.Callback
        {
            private readonly AudioBookMediaBrowserService mediaBrowserService;
            private readonly MediaSessionCompat mediaSession;

            public MediaSessionCallback(AudioBookMediaBrowserService mediaBrowserService, MediaSessionCompat mediaSession)
            {
                this.mediaBrowserService = mediaBrowserService;
                this.mediaSession = mediaSession;
            }

            public override void OnCommand(string command, Bundle extras, ResultReceiver cb)
            {
                base.OnCommand(command, extras, cb);
            }

            public override void OnCustomAction(string action, Bundle extras)
            {
                base.OnCustomAction(action, extras);
            }

            public override void OnPrepare()
            {
                System.Diagnostics.Debug.WriteLine("[MediaSessionCallback] [OnPrepare] Execute");

                var metadata = new MediaMetadataCompat.Builder();
                metadata.PutString(MediaMetadataCompat.MetadataKeyMediaId, "media_1");
                metadata.PutString(MediaMetadataCompat.MetadataKeyAlbum, "Sample AudioBook");
                metadata.PutString(MediaMetadataCompat.MetadataKeyArtist, "Sample AudioBook Author");
                metadata.PutString(MediaMetadataCompat.MetadataKeyGenre, "Sample Genre");
                metadata.PutString(MediaMetadataCompat.MetadataKeyTitle, "Sample AudioBook Title");
                metadata.PutLong(MediaMetadataCompat.MetadataKeyDuration, (long)TimeSpan.FromHours(4.23d).TotalMilliseconds);

                mediaSession.SetMetadata(metadata.Build());

                if (false == mediaSession.Active)
                {
                    mediaSession.Active = true;
                }
            }
        }

        // MediaControllerCallback class
        private sealed class MediaControllerCallback : MediaControllerCompat.Callback
        {
            public override void OnSessionReady()
            {
                System.Diagnostics.Debug.WriteLine("[AudioBookPlaybackService.MediaControllerCallback] [OnSessionReady] Execute");
            }

            public override void OnPlaybackStateChanged(PlaybackStateCompat state)
            {
                System.Diagnostics.Debug.WriteLine($"[AudioBookPlaybackService.MediaControllerCallback] [OnPlaybackStateChanged] Playback state: \"{state.State}\"");
            }
        }
    }
}