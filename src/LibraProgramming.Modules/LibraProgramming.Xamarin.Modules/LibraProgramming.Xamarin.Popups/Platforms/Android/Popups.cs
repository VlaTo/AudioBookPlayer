using Android.Content;
using System;
using System.Threading;

namespace LibraProgramming.Xamarin.Popups.Platforms.Android
{
    public static class Popups
    {
        private const int NotSet = 0;
        private const int Set = 1;

        private static int gate;

        internal static bool IsInitialized
        {
            get;
            private set;
        }

        internal static Context Context
        {
            get;
            private set;
        }

        internal static event EventHandler OnInitialized;

        public static void Init(Context context)
        {
            if (Set == Interlocked.CompareExchange(ref gate, Set, NotSet))
            {
                return;
            }

            Context = context;

            IsInitialized = true;

            OnInitialized?.Invoke(null, EventArgs.Empty);
        }
    }
}
