#nullable enable

using System;
using Android.App;
using Android.Views;
using Android.Widget;
using Java.Util;
using Java.Util.Concurrent;

namespace AudioBookPlayer.App.Views
{
    internal class WaitIndicator
    {
        private readonly Activity activity;
        private Timer timer;
        private long delay;
        private string? label;
        private TextView? description;
        //private LinearLayout? indicatorLayout;
        private FrameLayout? overlay;

        public WaitIndicator(Activity activity)
        {
            this.activity = activity;

            timer = new Timer();
            label = null;
            delay = TimeUnit.Milliseconds?.ToMillis(200L) ?? 200L;
        }

        public void ParseLayout(View view)
        {
            overlay = view.FindViewById<FrameLayout>(Resource.Id.overlay_layout);
            description = view.FindViewById<TextView>(Resource.Id.busy_indicator_text);
        }

        public void Show(string? text = null)
        {
            label = String.IsNullOrEmpty(text) ? String.Empty : text;

            if (null == overlay)
            {
                timer.Schedule(new PendingTask(timer, DoShow), delay);
            }
            else
            {
                activity.RunOnUiThread(() => description.Text = label);
            }
        }

        public void Hide()
        {
            if (overlay is { Visibility: ViewStates.Visible })
            {
                activity.RunOnUiThread(() => overlay.Visibility = ViewStates.Gone);
            }
        }

        private void DoShow()
        {
            activity.RunOnUiThread(() =>
            {
                description.Text = label;
                overlay.Visibility = ViewStates.Visible;
            });
        }

        private sealed class PendingTask : TimerTask
        {
            private readonly Timer timer;
            private readonly Action action;

            public PendingTask(Timer timer, Action action)
            {
                this.timer = timer;
                this.action = action;
            }

            public override void Run()
            {
                timer.Cancel();
                action.Invoke();
            }
        }
    }
}