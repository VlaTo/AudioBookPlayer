using System;

namespace LibraProgramming.Xamarin.Controls.Effects.Ripple
{
    public class TouchStatusChangedEventArgs : EventArgs
    {
        public TouchStatus Status
        {
            get;
        }

        internal TouchStatusChangedEventArgs(TouchStatus status)
        {
            Status = status;
        }
    }
}