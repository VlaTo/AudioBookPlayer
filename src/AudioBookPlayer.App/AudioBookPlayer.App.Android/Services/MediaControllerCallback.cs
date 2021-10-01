using System;
using Android.OS;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using Java.Lang;

namespace AudioBookPlayer.App.Android.Services
{
    internal sealed class MediaControllerCallback : MediaControllerCompat.Callback
    {
        public Action OnSessionReadyImpl
        {
            get;
            set;
        }

        public Action<PlaybackStateCompat> OnPlaybackStateChangedImpl
        {
            get;
            set;
        }

        public Action<MediaControllerCompat.PlaybackInfo> OnAudioInfoChangedImpl
        {
            get;
            set;
        }

        public Action<MediaMetadataCompat> OnMetadataChangedImpl
        {
            get;
            set;
        }

        public Action<string> OnQueueTitleChangedImpl
        {
            get;
            set;
        }

        public Action<string, Bundle> OnSessionEventImpl
        {
            get;
            set;
        }

        public Action OnSessionDestroyedImpl
        {
            get;
            set;
        }

        public override void OnSessionReady() => OnSessionReadyImpl?.Invoke();

        public override void OnPlaybackStateChanged(PlaybackStateCompat state) => OnPlaybackStateChangedImpl?.Invoke(state);

        public override void OnAudioInfoChanged(MediaControllerCompat.PlaybackInfo info) => OnAudioInfoChangedImpl?.Invoke(info);

        public override void OnMetadataChanged(MediaMetadataCompat metadata) => OnMetadataChangedImpl?.Invoke(metadata);

        public override void OnQueueTitleChanged(ICharSequence title) => OnQueueTitleChangedImpl?.Invoke(title.ToString());

        public override void OnSessionEvent(string e, Bundle extras) => OnSessionEventImpl?.Invoke(e, extras);

        public override void OnSessionDestroyed() => OnSessionDestroyedImpl?.Invoke();
    }
}