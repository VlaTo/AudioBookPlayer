using Android.Views;
using System.Collections.Generic;

namespace LibraProgramming.Xamarin.Controls.Platforms.Shared
{
    internal delegate void TouchEventHandler(View.TouchEventArgs e);

    internal static class TouchController
    {
        private static readonly Dictionary<View, List<TouchEventHandler>> handlers;

        static TouchController()
        {
            handlers = new Dictionary<View, List<TouchEventHandler>>();
        }

        public static void Add(View view, TouchEventHandler handler)
        {
            if (false == handlers.TryGetValue(view, out var actions))
            {
                actions = new List<TouchEventHandler>();
                view.Touch += OnViewTouch;
                handlers.Add(view, actions);
            }

            actions.Add(handler);
        }

        public static void Delete(View view, TouchEventHandler handler)
        {
            if (false == handlers.TryGetValue(view, out var actions))
            {
                return;
            }

            actions.Remove(handler);

            if (0 < actions.Count)
            {
                return;
            }

            view.Touch -= OnViewTouch;

            handlers.Remove(view);
        }

        private static void OnViewTouch(object sender, View.TouchEventArgs e)
        {
            var view = (View)sender;

            //if (!Collection.ContainsKey(view) || (_activeView != null && _activeView != view)) return;
            if (false == handlers.ContainsKey(view))
            {
                return;
            }

            switch (e.Event.Action)
            {
                case MotionEventActions.Down:
                {
                    //_activeView = view;
                    view.PlaySoundEffect(SoundEffects.Click);

                    break;
                }

                case MotionEventActions.Up:
                case MotionEventActions.Cancel:
                {
                    //_activeView = null;
                    e.Handled = true;

                    break;
                }
            }

            RaiseHandlers(view, e);
        }

        private static void RaiseHandlers(View view, View.TouchEventArgs e)
        {
            if (handlers.TryGetValue(view, out var actions))
            {
                var handlers = actions.ToArray();

                foreach (var handler in handlers)
                {
                    handler.Invoke(e);
                }
            }
        }
    }
}
