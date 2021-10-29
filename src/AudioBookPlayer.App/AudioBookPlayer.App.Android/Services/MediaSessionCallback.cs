using System;
using Android.OS;
using Android.Support.V4.Media.Session;
using LibraProgramming.Xamarin.Core;

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

        public Action OnPauseImpl
        {
            get;
            set;
        }

        public Action OnSkipToNextImpl
        {
            get;
            set;
        }

        public Action OnSkipToPreviousImpl
        {
            get;
            set;
        }

        public Action<long> OnSkipToQueueItemImpl
        {
            get;
            set;
        }

        public Action OnStopImpl
        {
            get;
            set;
        }

        public Action OnFastForwardImpl
        {
            get;
            set;
        }

        public Action OnRewindImpl
        {
            get;
            set;
        }

        public Action<long> OnSeekToImpl
        {
            get;
            set;
        }

        public MediaSessionCallback()
        {
            OnPrepareImpl = Stub.Nop();
            OnPlayImpl = Stub.Nop();
            OnPauseImpl = Stub.Nop();
            OnStopImpl = Stub.Nop();
            OnFastForwardImpl = Stub.Nop();
            OnRewindImpl = Stub.Nop();
            OnSkipToNextImpl = Stub.Nop();
            OnSkipToPreviousImpl = Stub.Nop();
            OnSeekToImpl = Stub.Nop<long>();
            OnSkipToQueueItemImpl = Stub.Nop<long>();
            OnPrepareFromMediaIdImpl = Stub.Nop<string, Bundle>();
            OnPlayFromMediaIdImpl = Stub.Nop<string, Bundle>();
            OnCustomActionImpl = Stub.Nop<string, Bundle>();
            OnCommandImpl = Stub.Nop<string, Bundle, ResultReceiver>();
        }

        public override void OnCommand(string command, Bundle options, ResultReceiver cb) => OnCommandImpl.Invoke(command, options, cb);

        public override void OnCustomAction(string action, Bundle extras) => OnCustomActionImpl.Invoke(action, extras);

        public override void OnPrepare() => OnPrepareImpl.Invoke();

        public override void OnPrepareFromMediaId(string mediaId, Bundle extras) => OnPrepareFromMediaIdImpl.Invoke(mediaId, extras);

        public override void OnPlay() => OnPlayImpl.Invoke();

        public override void OnPause() => OnPauseImpl.Invoke();

        public override void OnStop() => OnStopImpl.Invoke();

        public override void OnPlayFromMediaId(string mediaId, Bundle extras) => OnPlayFromMediaIdImpl.Invoke(mediaId, extras);

        public override void OnSkipToNext() => OnSkipToNextImpl.Invoke();

        public override void OnSkipToPrevious() => OnSkipToPreviousImpl.Invoke();

        public override void OnSkipToQueueItem(long id) => OnSkipToQueueItemImpl.Invoke(id);

        public override void OnFastForward() => OnFastForwardImpl.Invoke();

        public override void OnRewind() => OnRewindImpl.Invoke();

        public override void OnSeekTo(long pos) => OnSeekToImpl.Invoke(pos);
    }
}