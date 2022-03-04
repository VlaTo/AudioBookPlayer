using System;
using Android.Views;

#nullable enable

namespace AudioBookPlayer.App.Core.Internal
{
    internal class ClickListener : Java.Lang.Object, View.IOnClickListener
    {
        private Action<View?> callback;

        public static readonly ClickListener Empty;

        public static ClickListener Create(Action<View?> callback)
        {
            if (null == callback)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            return new ClickListener(callback);
        }

        private ClickListener(Action<View?> callback)
        {
            this.callback = callback;
        }

        static ClickListener()
        {
            Empty = new EmptyClickListener();
        }

        public void AddCallback(Action<View?> handler)
        {
            AddCallbackInternal(handler);
        }

        protected virtual void AddCallbackInternal(Action<View?> handler)
        {
            callback = (Action<View?>)Delegate.Combine(callback, handler);
        }

        void View.IOnClickListener.OnClick(View? view)
        {
            callback.Invoke(view);
        }

        private sealed class EmptyClickListener : ClickListener
        {
            public EmptyClickListener()
                : base(Nop)
            {
            }

            private static void Nop(View? _)
            {
                ;
            }

            protected override void AddCallbackInternal(Action<View?> handler)
            {
                throw new InvalidOperationException();
            }
        }
    }
}

#nullable restore