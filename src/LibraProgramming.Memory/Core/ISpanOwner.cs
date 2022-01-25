using System;

namespace LibraProgramming.Memory.Core
{
    internal interface ISpanOwner
    {
        int Length
        {
            get;
        }

        Span<byte> Span
        {
            get;
        }
    }
}