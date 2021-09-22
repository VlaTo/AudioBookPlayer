using System;

namespace LibraProgramming.Xamarin.Core
{
    public sealed class Stub
    {
        public static Action<T> Empty<T>() => InnerEmpty;
        
        public static Action<T1, T2> Empty<T1, T2>() => InnerEmpty;

        private static void InnerEmpty<T1>(T1 _) { }
        
        private static void InnerEmpty<T1, T2>(T1 t1, T2 t2) { }
    }
}