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
        private Timer? timer;
        private TextView? description;
        //private LinearLayout? indicatorLayout;
        private FrameLayout? overlay;

        public WaitIndicator(Activity activity)
        {
            this.activity = activity;

            timer = new Timer();
        }

        public void ParseLayout(View view)
        {
            overlay = view.FindViewById<FrameLayout>(Resource.Id.overlay_layout);
            description = view.FindViewById<TextView>(Resource.Id.busy_indicator_text);
        }

        public void Show(string? text = null)
        {
            timer.Schedule(new Task(timer, Show), TimeUnit.Milliseconds?.ToMillis(200L));
            activity.RunOnUiThread(() =>
            {
                description.Text = String.IsNullOrEmpty(text) ? String.Empty : text;
                overlay.Visibility = ViewStates.Visible;
            });
        }

        public void Hide()
        {
            if (overlay is { Visibility: ViewStates.Visible })
            {
                activity.RunOnUiThread(() => overlay.Visibility = ViewStates.Gone);
            }
        }
        
        private sealed class Task : TimerTask
        {
            private readonly Timer timer;
            private readonly Action action;

            public Task(Timer timer, Action action)
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