using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.CompilerServices;

namespace LibraProgramming.Xamarin.Core.Extensions
{
    public static class ReadOnlyMemoryExtensions
    {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Stream AsStream(this ReadOnlyMemory<byte> memory)
        {
            return  MemoryStream.Create(memory, true);
        }
    }
}