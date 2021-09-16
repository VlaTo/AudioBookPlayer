using Android.App;
using Android.Content;
using Android.Media.Session;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using AndroidX.Core.App;
using AndroidX.Media;
using AudioBookPlayer.App.Android.Core;
using AudioBookPlayer.App.Domain.Services;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using AudioBookPlayer.App.Android.Services.Builders;
using AudioBookPlayer.App.Domain.Data;
using AudioBookPlayer.App.Domain.Models;
using Application = Android.App.Application;

// https://developer.android.com/guide/topics/media/media-controls
// https://github.com/android/uamp/blob/f60b902643407ba234a316abe91410da7c08a4af/common/src/main/java/com/example/android/uamp/media/MusicService.kt

namespace AudioBookPlayer.App.Android.Services
{
    /// <summary>
    /// 
    /// </summary>
    [Service(Enabled = true, Exported = true)]
    [IntentFilter(new []{ ServiceInterface })]
    public sealed class AudioBookPlaybackService : MediaBrowserServiceCompat
    {
        private readonly IBooksService booksService;
        private readonly ComponentName componentName;
        private readonly PackageValidator packageValidator;
        private MediaSessionCompat mediaSession;
        private MediaControllerCompat mediaController;
        private PlaybackStateCompat.Builder playbackState;
        private MediaSessionCallback mediaSessionCallback;
        private MediaControllerCallback mediaControllerCallback;

        // ReSharper disable once UnusedMember.Global
        public AudioBookPlaybackService()
            : this(
                AudioBookPlayerApplication.Instance.DependencyContainer.GetInstance<IBooksService>()
            )
        {
        }

        private AudioBookPlaybackService(
            IBooksService booksService)
        {
            this.booksService = booksService;

            componentName = new ComponentName(Application.Context, Class);
            packageValidator = new PackageValidator(Application.Context);
        }

        public override void OnCreate()
        {
            base.OnCreate();

            var intent = PackageManager.GetLaunchIntentForPackage(componentName.PackageName);
            var pendingIntent = PendingIntent.GetActivity(Application.Context, 0, intent, PendingIntentFlags.UpdateCurrent);

            mediaSession = new MediaSessionCompat(Application.Context, nameof(AudioBookPlaybackService), componentName, pendingIntent);
            mediaSession.SetFlags((int)(MediaSessionFlags.HandlesMediaButtons | MediaSessionFlags.HandlesTransportControls));

            playbackState = new PlaybackStateCompat.Builder().SetActions(PlaybackStateCompat.ActionPlay | PlaybackStateCompat.ActionPlayPause);
            mediaSessionCallback = new MediaSessionCallback(this, mediaSession);

            mediaSession.SetPlaybackState(playbackState.Build());
            mediaSession.SetCallback(mediaSessionCallback);

            SessionToken = mediaSession.SessionToken;

            mediaController = new MediaControllerCompat(Application.Context, mediaSession);
            mediaControllerCallback = new MediaControllerCallback();
            mediaController.RegisterCallback(mediaControllerCallback);

            // notificationBuilder = new NotificationBuilder();
            // notificationManager = NotificationManagerCompat.From(Application.Context);

        }

        public override BrowserRoot OnGetRoot(string clientPackageName, int clientUid, Bundle rootHints)
        {
            if (packageValidator.IsCallerAllowed(clientPackageName, clientUid))
            {
                if (null == rootHints || rootHints.IsEmpty)
                {
                    return new BrowserRoot("/", null);
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

        public override void OnLoadChildren(string parentId, Result result)
        {
            //System.Diagnostics.Debug.WriteLine($"[{nameof(AudioBookPlaybackService)}.OnLoadChildren] parent id: \"{parentId}\"");

            if (String.Equals(parentId, "/"))
            {
                var books = booksService.QueryBooks();
                var list = new JavaList<MediaBrowserCompat.MediaItem>();

                for (var index = 0; index < books.Count; index++)
                {
                    var book = books[index];
                    var item = MediaItemBuilder.From(book, MediaBrowserCompat.MediaItem.FlagBrowsable | MediaBrowserCompat.MediaItem.FlagPlayable);

                    list.Add(item);
                }

                result.SendResult(list);

                return;
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
            private readonly AudioBookPlaybackService playbackService;
            private readonly MediaSessionCompat mediaSession;

            public MediaSessionCallback(AudioBookPlaybackService playbackService, MediaSessionCompat mediaSession)
            {
                this.playbackService = playbackService;
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