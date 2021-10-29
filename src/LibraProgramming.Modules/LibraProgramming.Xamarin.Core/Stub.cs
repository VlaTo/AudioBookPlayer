using System;

namespace LibraProgramming.Xamarin.Core
{
    public sealed class Stub
    {
        public static Action Nop() => InnerEmpty;

        public static Action<T> Nop<T>() => InnerEmpty;
        
        public static Action<T1, T2> Nop<T1, T2>() => InnerEmpty;
        
        public static Action<T1, T2, T3> Nop<T1, T2, T3>() => InnerEmpty;

        private static void InnerEmpty() { }

        private static void InnerEmpty<T1>(T1 _) { }
        
        private static void InnerEmpty<T1, T2>(T1 t1, T2 t2) { }

        private static void InnerEmpty<T1, T2, T3>(T1 t1, T2 t2, T3 t3) { }
    }
}