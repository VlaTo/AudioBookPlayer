using System;
using Android.OS;
using Android.Support.V4.Media.Session;

namespace AudioBookPlayer.App.Android.Services
{
    internal sealed class MediaSessionCallback : MediaSessionCompat.Callback
    {
        public Action<string, Bundle, ResultReceiver> OnCommandImpl
        {
            get;
            set;
        }

        public Action<string, Bundle> OnCustomActionImpl
        {
            get;
            set;
        }

        public Action OnPrepareImpl
        {
            get;
            set;
        }

        public Action OnPlayImpl
        {
            get;
            set;
        }

        public Action<string, Bundle> OnPlayFromMediaIdImpl
        {
            get;
            set;
        }

        public Action<string, Bundle> OnPrepareFromMediaIdImpl
        {
            get;
            set;
        }

        public override void OnCommand(string command, Bundle options, ResultReceiver cb) => OnCommandImpl.Invoke(command, options, cb);

        public override void OnCustomAction(string action, Bundle extras) => OnCustomActionImpl.Invoke(action, extras);

        public override void OnPrepare() => OnPrepareImpl.Invoke();

        public override void OnPrepareFromMediaId(string mediaId, Bundle extras) => OnPrepareFromMediaIdImpl.Invoke(mediaId, extras);

        public override void OnPlay() => OnPlayImpl.Invoke();

        public override void OnPlayFromMediaId(string mediaId, Bundle extras) => OnPlayFromMediaIdImpl.Invoke(mediaId, extras);
    }
}