using System;

namespace AudioBookPlayer.App.Core
{
    public enum PlaybackState : short
    {
        Failed = -1,
        None,
        Stopped,
        Buffering,
        Playing,
        Paused
    }

    public sealed class PlaybackStateEventArgs : EventArgs
    {
        public PlaybackState State
        {
            get;
        }

        public PlaybackStateEventArgs(PlaybackState state)
        {
            State = state;
        }
    }
}