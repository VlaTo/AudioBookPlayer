using System;

namespace AudioBookPlayer.MediaBrowserService.Core.Extensions
{
    internal static class TypeExtensions
    {
        public static bool IsString(this Type type) => null != type && typeof(string) == type;

        public static bool IsInt(this Type type) => null != type && typeof(int) == type;

        public static bool IsLong(this Type type) => null != type && typeof(long) == type;

        public static bool IsBoolean(this Type type) => null != type && typeof(bool) == type;

        public static bool IsTimeSpan(this Type type) => null != type && typeof(TimeSpan) == type;

        public static bool IsShort(this Type type) => null != type && typeof(short) == type;
    }
}