using System;

namespace AudioBookPlayer.App.Core
{
    internal sealed class Stub
    {
        public static Action Nop() => InnerEmpty;

        public static Action<T> Nop<T>() => InnerEmpty;

        public static Action<T1, T2> Nop<T1, T2>() => InnerEmpty;
        
        public static Action<T1, T2, T3> Nop<T1, T2, T3>() => InnerEmpty;

        #region Functions

        private static void InnerEmpty()
        {
        }

        private static void InnerEmpty<T>(T p1)
        {
        }

        private static void InnerEmpty<T1, T2>(T1 p1, T2 p2)
        {
        }

        private static void InnerEmpty<T1, T2, T3>(T1 p1, T2 p2, T3 p3)
        {
        }

        #endregion
    }
}