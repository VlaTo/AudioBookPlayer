using System;
using System.Text;

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

        public static string ToChunkKey(this byte[] bytes)
        {
            var offset = 0;

            if (bytes[offset] == 0xA9)
            {
                offset++;
            }

            return Encoding.ASCII.GetString(bytes, offset, bytes.Length - offset);
        }
    }
}