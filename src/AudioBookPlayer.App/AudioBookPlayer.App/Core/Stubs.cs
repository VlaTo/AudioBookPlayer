using System;

namespace AudioBookPlayer.App.Core
{
    internal static class Stubs
    {
        public static readonly Predicate<object> True;

        static Stubs()
        {
            True = _ => true;
        }
    }
}