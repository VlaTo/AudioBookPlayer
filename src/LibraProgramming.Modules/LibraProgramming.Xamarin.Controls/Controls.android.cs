using Android.Content;
using Android.OS;
using System.Threading;

namespace LibraProgramming.Xamarin.Controls
{
    public static class Controls
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

        public static void Init(Context context, Bundle bundle)
        {
            if (Set == Interlocked.CompareExchange(ref gate, Set, NotSet))
            {
                return;
            }

            Context = context;

            IsInitialized = true;
        }
        
        /*internal static FrameLayout GetContentView()
        {
            if (Context is Activity activity)
            {
                if (null != activity.Window && activity.Window.DecorView is FrameLayout layout)
                {
                    return layout;
                }
            }

            return null;
        }*/
    }
}
