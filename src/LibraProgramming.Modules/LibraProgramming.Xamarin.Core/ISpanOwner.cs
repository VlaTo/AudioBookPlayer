using System;

namespace LibraProgramming.Xamarin.Core
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