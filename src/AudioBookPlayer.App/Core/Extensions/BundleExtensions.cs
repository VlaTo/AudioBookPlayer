using System.Collections.Generic;
using Android.OS;
using Android.Support.V4.Media.Session;

#nullable enable

namespace AudioBookPlayer.App.Core.Extensions
{
    internal static class BundleExtensions
    {
        public static void PutQueue(this Bundle bundle, string? key, IList<MediaSessionCompat.QueueItem> queue)
        {
            var items = new List<IParcelable>();

            for (var index = 0; index < queue.Count; index++)
            {
                items.Add(queue[index]);
            }

            bundle.PutParcelableArrayList(key, items);
        }

        public static IList<MediaSessionCompat.QueueItem>? GetQueue(this Bundle bundle, string? key)
        {
            var list = bundle.GetParcelableArrayList(key);

            if (null != list)
            {
                var queue = new MediaSessionCompat.QueueItem[list.Count];

                for (var index = 0; index < list.Count; index++)
                {
                    var queueItem = (MediaSessionCompat.QueueItem)list[index];
                    queue[index] = queueItem;
                }

                return queue;
            }

            return null;
        }
    }
}

#nullable restore