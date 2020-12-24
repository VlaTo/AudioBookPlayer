using System;

namespace AudioBookPlayer.App.Droid.Core.Extensions
{
    internal static class FileDateTime
    {
        public static DateTime FromFileTime(long value)
        {
            return DateTime.UnixEpoch + TimeSpan.FromMilliseconds(value);
        }
    }
}