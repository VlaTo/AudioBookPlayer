using System;

namespace AudioBookPlayer.App.Android.Core.Extensions
{
    internal static class FileDateTime
    {
        public static DateTime ToFileTime(this long value)
        {
            return DateTime.UnixEpoch + TimeSpan.FromMilliseconds(value);
        }
    }
}