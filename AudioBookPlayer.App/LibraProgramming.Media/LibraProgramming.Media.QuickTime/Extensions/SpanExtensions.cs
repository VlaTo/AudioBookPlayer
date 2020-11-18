using LibraProgramming.Media.QuickTime.Extensions;
using System;

namespace LibraProgramming.Media.QuickTime.Helpers
{
    internal static class SpanExtensions
    {
        public static Span<byte> ToBigEndian(this Span<byte> original)
        {
            var array = original.ToArray();
            return new Span<byte>(array.ToBigEndian());
        }
    }
}