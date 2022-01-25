using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace LibraProgramming.Memory.Core
{
    internal readonly struct MemoryManagerOwner : ISpanOwner
    {
        private readonly MemoryManager<byte> memoryManager;
        private readonly int offset;
        private readonly int length;
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
                // We can't use the same trick we use for arrays to optimize the creation of
                // the offset span, as otherwise a bugged MemoryManager<T> instance returning
                // a span of an incorrect size could cause an access violation. Instead, we just
                // get the span and then slice it, which will validate both offset and length.
                return memoryManager.GetSpan().Slice(offset, length);
            }
        }

        public MemoryManagerOwner(MemoryManager<byte> memoryManager, int offset, int length)
        {
            this.memoryManager = memoryManager;
            this.offset = offset;
            this.length = length;
        }
    }
}