using System.Collections.Generic;
using System.Collections.ObjectModel;
using Android.Support.V4.Media.Session;

namespace AudioBookPlayer.App.Android.Core
{
    internal sealed class PlaybackQueue
    {
        private readonly List<MediaSessionCompat.QueueItem> queue;
        private int queueIndex;

        public bool CanPlayCurrent => -1 < queueIndex;

        public MediaSessionCompat.QueueItem Current
        {
            get
            {
                if (0 == queue.Count || 0 > queueIndex || queueIndex >= queue.Count)
                {
                    return null;
                }

                return queue[queueIndex];
            }
        }

        public int Count => queue.Count;

        public int CurrentIndex => queueIndex;

        public bool IsEmpty => 0 == queue.Count;

        public PlaybackQueue()
        {
            queue = new List<MediaSessionCompat.QueueItem>();
            queueIndex = -1;
        }

        public void SetQueue(IEnumerable<MediaSessionCompat.QueueItem> value)
        {
            queue.Clear();
            queue.AddRange(value);
            queueIndex = 0 < queue.Count ? 0 : -1;
        }

        public IList<MediaSessionCompat.QueueItem> AsReadOnlyList() => queue.AsReadOnly();

    }
}