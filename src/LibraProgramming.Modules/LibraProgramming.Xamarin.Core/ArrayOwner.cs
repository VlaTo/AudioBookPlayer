using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using LibraProgramming.Xamarin.Core.Extensions;

namespace LibraProgramming.Xamarin.Core
{
    internal readonly struct ArrayOwner : ISpanOwner
    {
        private readonly byte[] array;
        private readonly int offset;
        private readonly int length;
        public static ArrayOwner Empty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new ArrayOwner(Array.Empty<byte>(), 0, 0);
        }

        public int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => length;
        }

        public Span<byte> Span
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                ref byte r0 = ref array.DangerousGetReferenceAt(offset);
                return MemoryMarshal.CreateSpan(ref r0, length);
            }
        }

        public ArrayOwner(byte[] array, int offset, int length)
        {
            this.array = array;
            this.offset = offset;
            this.length = length;
        }
    }
}