using Android.Content;
using Android.OS;
using AudioBookPlayer.App.Android.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(AudioBookPlaybackServiceConnection))]

namespace AudioBookPlayer.App.Android.Services
{
    public sealed class AudioBookPlaybackServiceConnection : Java.Lang.Object, IServiceConnection
    {
        public bool IsConnected
        {
            get;
            private set;
        }

        public AudioBookPlaybackServiceBinder Binder
        {
            get;
            private set;
        }

        public AudioBookPlaybackServiceConnection()
        {
            IsConnected = false;
            Binder = null;
        }

#nullable enable
        public void OnServiceConnected(ComponentName? name, IBinder? binder)
        {
            Binder = binder as AudioBookPlaybackServiceBinder;
            IsConnected = null != Binder;

            if (IsConnected)
            {
                System.Diagnostics.Debug.WriteLine("[AudioBookPlaybackServiceConnection] [OnServiceConnected] Connecting");
            }
        }

        public void OnServiceDisconnected(ComponentName? name)
        {
            if (IsConnected)
            {
                System.Diagnostics.Debug.WriteLine("[AudioBookPlaybackServiceConnection] [OnServiceDisconnected] Executed");
            }

            Binder = null;
            IsConnected = false;
        }
#nullable restore
    }
}