using System.Collections.Generic;

namespace AudioBookPlayer.App.Core.Extensions
{
    internal static class ListExtensions
    {
        public static void Swap<T>(this List<T> list, int sourceIndex, int targetIndex)
        {
            (list[sourceIndex], list[targetIndex]) = (list[targetIndex], list[sourceIndex]);
        }
    }
}