using Android.Content;
using Android.OS;

namespace AudioBookPlayer.App.Android.Services
{
    public class PlaybackServiceConnection : Java.Lang.Object, IServiceConnection
    {
        private readonly MainActivity mainActivity;

        public bool IsConnected
        {
            get;
            private set;
        }

        public PlaybackServiceBinder Binder
        {
            get;
            private set;
        }

        public PlaybackServiceConnection(MainActivity mainActivity)
        {
            this.mainActivity = mainActivity;

            IsConnected = false;
            Binder = null;
        }

#nullable enable
        public void OnServiceConnected(ComponentName? name, IBinder? service)
        {
            Binder = service as PlaybackServiceBinder;
            IsConnected = null != Binder;

            if (IsConnected)
            {
                //mainActivity.OnPlaybackServiceConnected();
            }
            else
            {
                ;
            }
        }

        public void OnServiceDisconnected(ComponentName? name)
        {
            Binder = null;
            IsConnected = false;

            //mainActivity.OnPlaybackServiceDisconnected();
        }
#nullable restore

    }
}