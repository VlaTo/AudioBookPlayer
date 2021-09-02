using System;

namespace LibraProgramming.Xamarin.Controls.Effects.Ripple
{
    /// <summary>
    /// 
    /// </summary>
    public class TouchStateChangedEventArgs : EventArgs
    {
        public TouchState State
        {
            get;
        }

        internal TouchStateChangedEventArgs(TouchState state)
        {
            State = state;
        }
    }
}