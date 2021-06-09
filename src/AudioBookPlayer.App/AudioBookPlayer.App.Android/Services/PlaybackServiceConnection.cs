using System;
using Android.Content;
using Android.OS;
using AudioBookPlayer.App.Models;

namespace AudioBookPlayer.App.Android.Services
{
    public class PlaybackServiceConnection : Java.Lang.Object, IServiceConnection, IPlaybackService
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
                mainActivity.OnPlaybackServiceConnected();
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

            mainActivity.OnPlaybackServiceDisconnected();
        }
#nullable restore
        public void SetBook(AudioBook audioBook)
        {
            if (false == IsConnected)
            {
                return;
            }

            var service = Binder.Service;

            service.SetBook(audioBook);
        }

        public void Play(int chapterIndex, TimeSpan position)
        {
            if (false == IsConnected)
            {
                return;
            }

            var service = Binder.Service;

            service.Play(chapterIndex, position);
        }
    }
}