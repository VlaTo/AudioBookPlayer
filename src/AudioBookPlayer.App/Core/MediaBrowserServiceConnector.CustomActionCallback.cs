using Android.OS;
using Android.Support.V4.Media;

namespace AudioBookPlayer.App.Core
{
    internal sealed partial class ConnectExecutor
    {
        private sealed class CustomActionCallback : MediaBrowserCompat.CustomActionCallback
        {
            public System.Action<string, Bundle, Bundle> OnErrorImpl
            {
                get; 
                set;
            }

            public System.Action<string, Bundle, Bundle> OnProgressUpdateImpl
            {
                get; 
                set;
            }

            public System.Action<string, Bundle, Bundle> OnResultImpl
            {
                get;
                set;
            }

            public CustomActionCallback()
            {
                OnErrorImpl = Stub.Nop<string, Bundle, Bundle>();
                OnProgressUpdateImpl = Stub.Nop<string, Bundle, Bundle>();
                OnResultImpl = Stub.Nop<string, Bundle, Bundle>();
            }

            public override void OnError(string action, Bundle extras, Bundle data) =>
                OnErrorImpl.Invoke(action, extras, data);

            public override void OnProgressUpdate(string action, Bundle extras, Bundle data) =>
                OnProgressUpdateImpl.Invoke(action, extras, data);

            public override void OnResult(string action, Bundle extras, Bundle result) =>
                OnResultImpl.Invoke(action, extras, result);
        }
    }
}