using Android.Media;
using Android.Media.Session;
using Android.Support.V4.Media.Session;
using AudioBookPlayer.App.Domain.Services;

namespace AudioBookPlayer.App.Android.Core
{
    internal sealed class Playback
    {
        private readonly MediaSessionCompat mediaSession;
        private readonly IBooksService booksService;
        private MediaPlayer mediaPlayer;
        private PlaybackStateCompat.Builder playbackState;
        private PlaybackStateCode state;
        private bool connected;

        public PlaybackStateCode State
        {
            get => state;
            set
            {
                if (state == value)
                {
                    return;
                }

                state = value;
            }
        }

        public bool IsConnected
        {
            get
            {
                return false;
            }
        }

        public bool IsPlaying
        {
            get
            {
                return false;
            }
        }

        public long CurrentMediaPosition
        {
            get
            {
                return 0L;
            }
        }

        public Playback(MediaSessionCompat mediaSession, IBooksService booksService)
        {
            this.mediaSession = mediaSession;
            this.booksService = booksService;

            state = PlaybackStateCode.None;
        }

        public void Play(MediaSessionCompat.QueueItem item)
        {
            var mediaId = item.Description.MediaId;

            if (false == mediaSession.Active)
            {
                mediaSession.Active = true;
            }

            // playbackState.SetState(PlaybackStateCompat.StatePlaying, 0L, 1.0f);
            // mediaSession.SetPlaybackState(playbackState.Build());
        }
    }
}