using Android.Support.V4.Media.Session;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AudioBookPlayer.App.Android.Core
{
    internal sealed class PlaybackQueue : IEnumerable<MediaSessionCompat.QueueItem>
    {
        private readonly List<MediaSessionCompat.QueueItem> queue;
        private int currentIndex;

        public MediaSessionCompat.QueueItem Current
        {
            get
            {
                if (0 == queue.Count || 0 > CurrentIndex || CurrentIndex >= queue.Count)
                {
                    return null;
                }

                return queue[CurrentIndex];
            }
        }

        public int Count => queue.Count;

        public int CurrentIndex
        {
            get => currentIndex;
            set
            {
                if (value == currentIndex)
                {
                    return;
                }

                if (0 == queue.Count)
                {
                    throw new Exception();
                }

                if (0 > value || value >= queue.Count)
                {
                    throw new Exception();
                }

                currentIndex = value;
            }
        }

        public bool IsEmpty => 0 == queue.Count;

        public MediaSessionCompat.QueueItem this[int index]
        {
            get
            {
                if (IsValidIndex(index))
                {
                    return queue[index];
                }

                throw new IndexOutOfRangeException();
            }
        }

        public PlaybackQueue()
        {
            queue = new List<MediaSessionCompat.QueueItem>();
            currentIndex = -1;
        }

        public void SetQueue(IEnumerable<MediaSessionCompat.QueueItem> value)
        {
            queue.Clear();
            queue.AddRange(value);
            
            currentIndex = 0 < queue.Count ? 0 : -1;
        }

        public void Clear()
        {
            queue.Clear();
            currentIndex = -1;
        }

        public bool MovePrevious()
        {
            var index = currentIndex;

            if (0 < Count && 0 < index)
            {
                currentIndex = index - 1;
                return true;
            }

            return false;
        }

        public bool MoveNext()
        {
            var index = currentIndex;

            if (0 < Count && index < (Count - 1))
            {
                currentIndex = index + 1;
                return true;
            }

            return false;
        }

        public int FindIndex(long queueId) => queue.FindIndex(entry => entry.QueueId == queueId);

        public bool IsValidIndex(int index)
        {
            if (0 == queue.Count)
            {
                return false;
            }

            return -1 < index && index < queue.Count;
        }

        public IList<MediaSessionCompat.QueueItem> AsQueue() => queue.AsReadOnly();

        public IEnumerator<MediaSessionCompat.QueueItem> GetEnumerator() => queue.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}