using System;
using System.Collections;
using System.Collections.Generic;

namespace LibraProgramming.Media.Common
{
    public sealed class MediaTags : IEnumerable<KeyValuePair<string, TagsCollection>>
    {
        private readonly Dictionary<string, TagsCollection> items;

        public TagsCollection this[string key] => items.TryGetValue(key, out var collection) ? collection : null;

        public MediaTags()
        {
            items = new Dictionary<string, TagsCollection>();
        }

        public void Add(string key, TagValue item)
        {
            if (null == item)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var collection = GetOdAddCollection(key);

            collection.Add(item);
        }

        public IEnumerator<KeyValuePair<string, TagsCollection>> GetEnumerator() => items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private TagsCollection GetOdAddCollection(string key)
        {
            if (false == items.TryGetValue(key, out var collection))
            {
                collection = new TagsCollection();
                items.Add(key, collection);
            }

            return collection;
        }


    }
}
