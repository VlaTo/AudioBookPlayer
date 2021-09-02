using System;
using System.Collections.Generic;
using LibraProgramming.Xamarin.Core;

namespace AudioBookPlayer.App.Services
{
    internal sealed class InMemoryCache<TKey, TValue> : ICache<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> cache;

        public InMemoryCache()
        {
            cache = new Dictionary<TKey, TValue>();
        }

        public void Put(TKey key, TValue value)
        {
            cache[key] = value;
        }

        public TValue Get(TKey key)
        {
            if (false == cache.TryGetValue(key, out var value))
            {
                throw new Exception();
            }

            return value;
        }

        public bool Has(TKey key) => cache.ContainsKey(key);

        public void Invalidate(TKey key) => cache.Remove(key);
    }
}