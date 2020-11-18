using System;

namespace LibraProgramming.Media.QuickTime.Extensions
{
    internal static class StringExtensions
    {
        public static string ToVariableName(this string str)
        {
            if (String.IsNullOrEmpty(str))
            {
                throw new ArgumentException("", nameof(str));
            }

            return Char.ToUpper(str[0]) + str.Substring(1).ToLowerInvariant();
        }
    }
}