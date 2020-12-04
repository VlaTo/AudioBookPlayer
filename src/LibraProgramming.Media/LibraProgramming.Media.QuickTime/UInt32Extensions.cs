using System;

namespace LibraProgramming.Media.QuickTime
{
    internal static class UInt32Extensions
    {
        public static readonly DateTime UnixEpoch;

        public static DateTime FromUnixTime(this uint unixTime)
        {
            return UnixEpoch;
        }

        static UInt32Extensions()
        {
            UnixEpoch = new DateTime(1970, 1, 1);
        }
    }
}