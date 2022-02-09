using Android.OS;
using Java.Lang;
using System;
using System.Collections.Generic;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;

namespace AudioBookPlayer.App.Core
{
    internal partial class MediaBrowserServiceConnector
    {
        public class MediaControllerCallback : MediaControllerCompat.Callback
        {
            public Action<PlaybackStateCompat> OnPlaybackStateChangedImpl
            {
                get;
                set;
            }

            public Action<MediaMetadataCompat> OnMetadataChangedImpl
            {
                get;
                set;
            }

            public Action<IList<MediaSessionCompat.QueueItem>> OnQueueChangedImpl
            {
                get;
                set;
            }

            public Action<string> OnQueueTitleChangedImpl
            {
                get;
                set;
            }

            public Action<MediaControllerCompat.PlaybackInfo> OnAudioInfoChangedImpl
            {
                get;
                set;
            }

            public Action<Bundle> OnExtrasChangedImpl
            {
                get;
                set;
            }

            public MediaControllerCallback()
            {
                OnPlaybackStateChangedImpl = Stub.Nop<PlaybackStateCompat>();
                OnMetadataChangedImpl = Stub.Nop<MediaMetadataCompat>();
                OnQueueChangedImpl = Stub.Nop<IList<MediaSessionCompat.QueueItem>>();
                OnQueueTitleChangedImpl = Stub.Nop<string>();
                OnAudioInfoChangedImpl = Stub.Nop<MediaControllerCompat.PlaybackInfo>();
                OnExtrasChangedImpl = Stub.Nop<Bundle>();
            }

            public override void OnAudioInfoChanged(MediaControllerCompat.PlaybackInfo info) => OnAudioInfoChangedImpl.Invoke(info);

            public override void OnCaptioningEnabledChanged(bool enabled)
            {
                base.OnCaptioningEnabledChanged(enabled);
            }

            public override void OnExtrasChanged(Bundle extras) => OnExtrasChangedImpl.Invoke(extras);

            public override void OnMetadataChanged(MediaMetadataCompat metadata) => OnMetadataChangedImpl.Invoke(metadata);

            public override void OnPlaybackStateChanged(PlaybackStateCompat state) => OnPlaybackStateChangedImpl.Invoke(state);

            public override void OnQueueChanged(IList<MediaSessionCompat.QueueItem> queue) => OnQueueChangedImpl.Invoke(queue);

            public override void OnQueueTitleChanged(ICharSequence title) => OnQueueTitleChangedImpl.Invoke(title.ToString());

            public override void OnRepeatModeChanged(int repeatMode)
            {
                base.OnRepeatModeChanged(repeatMode);
            }

            public override void OnSessionDestroyed()
            {
                base.OnSessionDestroyed();
            }

            public override void OnSessionEvent(string e, Bundle extras)
            {
                base.OnSessionEvent(e, extras);
            }

            public override void OnSessionReady()
            {
                base.OnSessionReady();
            }

            public override void OnShuffleModeChanged(int shuffleMode)
            {
                base.OnShuffleModeChanged(shuffleMode);
            }
        }
    }
}