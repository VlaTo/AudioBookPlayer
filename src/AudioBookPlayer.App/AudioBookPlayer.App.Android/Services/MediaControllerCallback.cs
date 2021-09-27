using System;
using Android.OS;
using Android.Support.V4.Media.Session;

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

        public override void OnSessionReady() => OnSessionReadyImpl.Invoke();
        /*{
            System.Diagnostics.Debug.WriteLine("[AudioBookPlaybackService.MediaControllerCallback] [OnSessionReady] Execute");
        }*/

        public override void OnPlaybackStateChanged(PlaybackStateCompat state) => OnPlaybackStateChangedImpl.Invoke(state);
        /*{
            System.Diagnostics.Debug.WriteLine($"[AudioBookPlaybackService.MediaControllerCallback] [OnPlaybackStateChanged] Playback state: \"{state.State}\"");
        }*/

        public override void OnSessionEvent(string e, Bundle extras)
        {
            System.Diagnostics.Debug.WriteLine($"[AudioBookPlaybackService.MediaControllerCallback] [OnSessionEvent] Event: \"{e}\"");
        }
    }
}