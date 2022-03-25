using System;
using Android.Views;

#nullable enable

namespace AudioBookPlayer.App.Core.Internal
{
    internal class LongClickListener : Java.Lang.Object, View.IOnLongClickListener
    {
        private Func<View?, bool> callback;

        public static readonly LongClickListener Empty;
        
        public static LongClickListener Create(Func<View?, bool> callback)
        {
            if (null == callback)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            return new LongClickListener(callback);
        }

        private LongClickListener(Func<View?, bool> callback)
        {
            this.callback = callback;
        }

        static LongClickListener()
        {
            Empty = new EmptyClickListener();
        }

        public void AddCallback(Func<View?, bool> handler)
        {
            AddCallbackInternal(handler);
        }

        private protected virtual void AddCallbackInternal(Func<View?, bool> handler)
        {
            callback = (Func<View?, bool>)Delegate.Combine(callback, handler);
        }

        bool View.IOnLongClickListener.OnLongClick(View? view)
        {
            return callback.Invoke(view);
        }

        private sealed class EmptyClickListener : LongClickListener
        {
            public EmptyClickListener()
                : base(Nop)
            {
            }

            private static bool Nop(View? _) => false;

            private protected override void AddCallbackInternal(Func<View?, bool> handler)
            {
                throw new InvalidOperationException();
            }
        }
    }
}

#nullable restore