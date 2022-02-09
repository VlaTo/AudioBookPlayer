#nullable enable

using Android.Support.V4.Media.Session;
using AndroidX.Media;

namespace AudioBookPlayer.MediaBrowserService.Core
{
    internal sealed class Playback
    {
        private readonly MediaBrowserServiceCompat browserService;
        private readonly MediaSessionCompat mediaSession;
        private readonly IPlaybackCallback callback;
        private int state;

        public Playback(MediaBrowserServiceCompat browserService, MediaSessionCompat mediaSession, IPlaybackCallback callback)
        {
            this.browserService = browserService;
            this.mediaSession = mediaSession;
            this.callback = callback;

            state = PlaybackStateCompat.StateNone;
        }

        /// <summary>
        /// IPlaybackCallback interface.
        /// </summary>
        public interface IPlaybackCallback
        {
            void StateChanged();

            void PositionChanged();
        }
    }
}