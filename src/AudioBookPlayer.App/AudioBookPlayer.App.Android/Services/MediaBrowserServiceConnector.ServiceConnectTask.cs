using System;
using LibraProgramming.Xamarin.Core;

namespace AudioBookPlayer.App.Android.Services
{
    internal partial class MediaBrowserServiceConnector
    {
        private sealed class ServiceConnectTask : TaskExecutor
        {
            public Action<TaskCompletionSource, Action> OnExecuteImpl
            {
                get;
                set;
            }

            public ServiceConnectTask()
                : base(new InterlockedTaskLock())
            {
            }

            protected override void DoExecute(TaskCompletionSource tcs, Action callback) => OnExecuteImpl.Invoke(tcs, callback);
        }
    }
}