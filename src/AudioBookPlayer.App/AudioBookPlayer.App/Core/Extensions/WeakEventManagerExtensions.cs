using System;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace AudioBookPlayer.App.Core.Extensions
{
    internal static class WeakEventManagerExtensions
    {
        /// <summary>
        /// Invokes the event EventHandler
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="handler">Event arguments</param>
        /// <param name="eventName"></param>
        public static void AddEventHandler<TEventHandler, TEventArgs>(
            this WeakEventManager manager,
            TEventHandler handler,
            [CallerMemberName] string eventName = null)
            where TEventHandler : Delegate
            where TEventArgs : EventArgs
        {
            var @delegate = (EventHandler<TEventArgs>) Delegate.CreateDelegate(typeof(EventHandler), handler.Target, handler.Method);
            //var @delegate = new EventHandler<TEventArgs>((sender, e) => handler.DynamicInvoke(sender, e));
            manager.AddEventHandler(@delegate, eventName);
        }
        
        public static void RemoveEventHandler<TEventHandler, TEventArgs>(
            this WeakEventManager manager,
            TEventHandler handler,
            [CallerMemberName] string eventName = null)
            where TEventHandler : Delegate
            where TEventArgs : EventArgs
        {
            var @delegate = (EventHandler<TEventArgs>) Delegate.CreateDelegate(typeof(EventHandler), handler.Target, handler.Method);
            //var @delegate = new EventHandler<TEventArgs>((sender, e) => handler.DynamicInvoke(sender, e));
            manager.RemoveEventHandler(@delegate, eventName);
        }
    }
}
