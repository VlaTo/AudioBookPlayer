using Android.Support.V4.Media.Session;
using AudioBookPlayer.App.Core;

namespace AudioBookPlayer.App.Android.Extensions
{
    internal static class PlaybackStateExtensions
    {
        public static PlaybackState ToPlaybackState(this int value)
        {
            switch (value)
            {
                case PlaybackStateCompat.StateNone:
                {
                    return PlaybackState.None;
                }

                case PlaybackStateCompat.StateError:
                {
                    return PlaybackState.Failed;
                }

                case PlaybackStateCompat.StateBuffering:
                {
                    return PlaybackState.Buffering;
                }

                case PlaybackStateCompat.StatePlaying:
                {
                    return PlaybackState.Playing;
                }

                case PlaybackStateCompat.StatePaused:
                {
                    return PlaybackState.Paused;
                }

                case PlaybackStateCompat.StateStopped:
                {
                    return PlaybackState.Stopped;
                }

                default:
                {
                    return PlaybackState.None;
                }
            }
        }
    }
}