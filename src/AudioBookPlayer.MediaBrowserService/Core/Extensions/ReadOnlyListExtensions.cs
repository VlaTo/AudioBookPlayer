using System.Collections.Generic;

namespace AudioBookPlayer.MediaBrowserService.Core.Extensions
{
    internal static class ReadOnlyListExtensions
    {
        public static IReadOnlyList<T> Append<T>(this IReadOnlyList<T> source, T item)
        {
            if (source is IList<T> list)
            {
                list.Add(item);
                return source;
            }

            var array = new T[source.Count + 1];

            for (var index = 0; index < source.Count; index++)
            {
                array[index] = source[index];
            }

            array[source.Count] = item;

            return array;
        }
    }
}