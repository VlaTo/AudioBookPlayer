using System;
using Android.Widget;
using AudioBookPlayer.Core;

#nullable enable

namespace AudioBookPlayer.App.Core.Internal
{
    internal sealed class ToastCallback : Toast.Callback
    {
        private readonly Action onShown;
        private readonly Action onHidden;

        public ToastCallback(Action? onShown = null, Action? onHidden = null)
        {
            this.onShown = onShown ?? Stub.Nop();
            this.onHidden = onHidden ?? Stub.Nop();
        }

        public override void OnToastHidden() => onHidden.Invoke();

        public override void OnToastShown() => onShown.Invoke();
    }
}

#nullable restore