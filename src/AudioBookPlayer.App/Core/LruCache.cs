using System.Collections.Generic;

namespace AudioBookPlayer.App.Core
{
    internal class LruCache<TKey, TValue>
    {
        private readonly IEqualityComparer<TKey> comparer;
        private readonly CacheEntry[] entries;
        private int count;

        public int HitCount
        {
            get;
            private set;
        }

        public int MissCount
        {
            get;
            private set;
        }

        protected LruCache(int maxSize)
        {
            comparer = EqualityComparer<TKey>.Default;
            entries = new CacheEntry[maxSize];
            count = 0;

            HitCount = 0;
            MissCount = 0;
        }

        public TValue Get(TKey key)
        {
            var position = FindIndex(key);

            if (0 > position)
            {
                MissCount++;
                return default;
            }

            var entry = entries[position];

            for (var index = position; index > 0; index--)
            {
                entries[index] = entries[index - 1];
            }

            entries[0] = entry;

            HitCount++;

            return entry.Value;
        }

        public void Put(TKey key, TValue value)
        {
            var position = FindIndex(key);
            
            if (-1 < position)
            {
                var entry = entries[position];

                for (var index = position; index > 0; index--)
                {
                    entries[index] = entries[index - 1];
                }

                Release(entry.Value);

                entries[0] = new CacheEntry(key, value);

                return;
            }

            if (count == entries.Length)
            {
                Release(entries[count - 1].Value);

                for (var index = (count - 1); index > 0; index--)
                {
                    entries[index] = entries[index - 1];
                }
            }
            else
            {
                for (var index = count; index > 0; index--)
                {
                    entries[index] = entries[index - 1];
                }

                count++;
            }

            entries[0] = new CacheEntry(key, value);
        }

        protected virtual void Release(TValue value)
        {
            ;
        }

        private int FindIndex(TKey key)
        {
            for (var index = 0; index < count; index++)
            {
                if (comparer.Equals(key, entries[index].Key))
                {
                    return index;
                }
            }

            return -1;
        }

        private readonly struct CacheEntry
        {
            public TKey Key
            {
                get;
            }

            public TValue Value
            {
                get;
            }

            public CacheEntry(TKey key, TValue value)
            {
                Key = key;
                Value = value;
            }
        }
    }
}