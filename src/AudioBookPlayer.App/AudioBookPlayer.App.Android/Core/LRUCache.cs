using System;
using System.Collections.Generic;

namespace AudioBookPlayer.App.Android.Core
{
    public class LRUCache<TKey, TValue> : ICacheable<TKey, TValue>, ISizable<TValue>
        where TKey : class
        where TValue : class
    {
        private readonly Dictionary<TKey, TValue> dictionary;
        private readonly Queue<TKey> queue;
        private int size;
        private int evictionCount;
        private int hitCount;
        private int missCount;

        public int MaxSize
        {
            get;
        }

        /// <summary>
        /// Current size of the cache.
        /// </summary>
        public int Size => size;

        /// <summary>
        /// The count of entries evicted 
        /// during the lifetime of the cache object's instance.
        /// </summary>
        public int EvictionCount => evictionCount;

        /// <summary>
        /// The count of successful entries retrieved.
        /// </summary>
        public int HitCount => hitCount;

        /// <summary>
        /// The count of entries tried to get retrieved 
        /// but had been evicted or did not exist.
        /// </summary>
        public int MissCount => missCount;

        public LRUCache(int maxSize)
        {
            if (0 >= maxSize)
            {
                throw new ArgumentException("maxSize <= 0");
            }

            this.MaxSize = maxSize;

            dictionary = new Dictionary<TKey, TValue>();
            queue = new Queue<TKey>();
        }

        #region ICache

        /// <summary>
        /// Returns true if the dictionary contains the given key.
        /// </summary>
        /// <param name="key">Key.</param>
        public bool Contains(TKey key) => dictionary.ContainsKey(key);

        /// <summary>
        /// Returns the value for the given key 
        /// found in the in-memory dictionary or 
        /// null if there isn't one.
        /// </summary>
        /// <param name="key">Key</param>
        public TValue Get(TKey key)
        {
            // Value for key is not in the dictionary.
            if (false == dictionary.TryGetValue(key, out var value))
            {
                missCount++;
                return null;
            }

            hitCount++;

            // If a value was found, we simply 
            // append the key to our queue.
            // Our eviction policy will 
            // take care of the duplicate keys by checking 
            // if the queue contains a key.
            // TODO : find a data structure better suited for this case.
            queue.Enqueue(key);

            return value;
        }

        /// <summary>
        /// Removes and returns the value for the specific key.
        /// If no value found for the specific key returns null instead.
        /// </summary>
        /// <param name="key">Key.</param>
        public TValue Remove(TKey key)
        {
            var val = dictionary[key];

            // If value is not null we 
            // remove it from the dictionary.
            if (val != null)
            {
                dictionary.Remove(key);
                size -= SizeOf(val);
            }

            return val;
        }

        /// <summary>
        /// Stores the value for the given key. 
        /// Both cannot be null.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">val</param>
        public void Put(TKey key, TValue value)
        {
            if (null == key)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (null == value)
            {
                throw new ArgumentNullException(nameof(value));
            }

            // If the dictionary already contains a value 
            // for the key we remove that value
            // so that it gets updated.
            // Important : We have to also reduce the total size.
            if (dictionary.TryGetValue(key, out var oldValue))
            {
                dictionary.Remove(key);
                size -= SizeOf(oldValue);
            }

            dictionary.Add(key, value);
            size += SizeOf(value);
            queue.Enqueue(key);

            Trim(MaxSize);
        }

        /// <summary>
        /// Clears all the entries cached.
        /// </summary>
        public void Clear() => Trim(-1);

        #endregion

        #region ISizeable

        /// <summary>
        /// Returns 1 as a default implementation for the size, 
        /// since we cannot measure the size of a class efficiently.
        /// </summary>
        /// <returns>1</returns>
        /// <param name="value">val</param>
        public virtual int SizeOf(TValue value)
        {
            return 1;
        }

        #endregion

        #region Private Functions

        private void Trim(int maxSize)
        {
            while (size > maxSize && dictionary.Count > 0)
            {
                // if queue is null we break the loop. 
                // We dont have any keys left.
                if (0 == queue.Count)
                {
                    break;
                }

                var keyToEvict = queue.Dequeue();

                // If the queue contains the keyToEvict, 
                // after it has been dequeued, 
                // we shouldn't remove it.
                if (queue.Contains(keyToEvict))
                {
                    continue;
                }

                Remove(keyToEvict);

                evictionCount++;
            }
        }

        #endregion
    }
}