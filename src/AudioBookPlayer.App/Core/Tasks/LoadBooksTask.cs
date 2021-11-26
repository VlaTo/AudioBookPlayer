#nullable enable

using Android.OS;
using Android.Views;
using Android.Widget;
using AudioBookPlayer.Domain;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Object = Java.Lang.Object;
using Void = Java.Lang.Void;

namespace AudioBookPlayer.App.Core.Tasks
{
    internal sealed class LoadBooksTask : AsyncTask<Void, Void, IReadOnlyList<AudioBookDescription>>,
        MediaBrowserServiceConnector.IConnectCallback,
        MediaBrowserServiceConnector.IAudioBooksResultCallback
    {
        private readonly MediaBrowserServiceConnector? connector;
        private readonly FrameLayout? overlay;
        private readonly TaskCompletionSource<IReadOnlyList<AudioBookDescription>> tcs;

        public LoadBooksTask(MediaBrowserServiceConnector? connector, FrameLayout? overlay)
        {
            this.connector = connector;
            this.overlay = overlay;
            tcs = new TaskCompletionSource<IReadOnlyList<AudioBookDescription>>();
        }

        protected override void OnPreExecute()
        {
            if (null != connector)
            {
                connector?.Connect(this);

                if (null != overlay)
                {
                    overlay.Visibility = ViewStates.Visible;
                    //System.Diagnostics.Debug.WriteLine("[LoadBooksTask] [OnPreExecute] Showing Indicator");
                }
            }
        }

        protected override void OnPostExecute(Object? result)
        {
            if (null != overlay)
            {
                overlay.Visibility = ViewStates.Gone;
                System.Diagnostics.Debug.WriteLine("[LoadBooksTask] [OnPostExecute] Hiding Indicator");
            }
        }

        protected override IReadOnlyList<AudioBookDescription> RunInBackground(params Void[] @params)
        {
            var tasks = new[] { Task.Delay(Timeout.Infinite), tcs.Task };
            var position = Task.WaitAny(tasks);
            return (1 == position) ? tcs.Task.Result : Array.Empty<AudioBookDescription>();
        }

        void MediaBrowserServiceConnector.IConnectCallback.OnConnected()
        {
            connector?.GetAudioBooks(this);
        }

        void MediaBrowserServiceConnector.IAudioBooksResultCallback.OnAudioBooksResult(IReadOnlyList<AudioBookDescription> list)
        {
            tcs.SetResult(list);
        }

        void MediaBrowserServiceConnector.IAudioBooksResultCallback.OnAudioBooksError()
        {
            tcs.SetCanceled();
        }
    }
}