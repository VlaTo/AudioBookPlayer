using LibraProgramming.Xamarin.Interaction;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AudioBookPlayer.App.Core
{
    public sealed class DelegateWeakEventManager
    {
        readonly Dictionary<string, List<Subscription>> handlers;

        public DelegateWeakEventManager()
        {
            handlers = new Dictionary<string, List<Subscription>>();
        }

        public void AddEventHandler(Delegate handler, [CallerMemberName] string eventName = "")
        {
            if (String.IsNullOrWhiteSpace(eventName))
            {
                throw new ArgumentNullException(nameof(eventName));
            }

            if (null == handler)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            EventManagerService.AddEventHandler(eventName, handler.Target, handler.Method, handlers);
        }
        
        public void RemoveEventHandler(Delegate handler, [CallerMemberName] string eventName = "")
        {
            if (String.IsNullOrWhiteSpace(eventName))
            {
                throw new ArgumentNullException(nameof(eventName));
            }

            if (null == handler)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            EventManagerService.RemoveEventHandler(eventName, handler.Target, handler.Method, handlers);
        }
        
        public void HandleEvent(object sender, object eventArgs, string eventName) => RaiseEvent(sender, eventArgs, eventName);
        
        public void HandleEvent(string eventName) => RaiseEvent(eventName);

        public void RaiseEvent(object sender, object eventArgs, string eventName) => EventManagerService.HandleEvent(eventName, sender, eventArgs, handlers);

        public void RaiseEvent(string eventName) => EventManagerService.HandleEvent(eventName, handlers);
    }
}
