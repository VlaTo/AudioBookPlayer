using System;
using System.Collections.Generic;

namespace AudioBookPlayer.App.Core.Extensions
{
    internal static class ListExtensions
    {
        public static int FindIndex<T>(this IList<T> list, T item, Func<T, T, int> compare)
        {
            for (var index = 0; index < list.Count; index++)
            {
                var result = compare.Invoke(list[index], item);

                if (0 >= result)
                {
                    continue;
                }

                return index;
            }

            return list.Count;
        }
    }
}