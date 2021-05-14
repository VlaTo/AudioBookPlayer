using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AudioBookPlayer.App.Core
{
    public sealed class WeakEventManager
    {
        private readonly Dictionary<string, List<Subscription>> handlers;

        public WeakEventManager()
        {
            handlers = new Dictionary<string, List<Subscription>>();
        }

        public void AddEventHandler<TEventArgs>(EventHandler<TEventArgs> handler, [CallerMemberName] string eventName = "")
            where TEventArgs : EventArgs
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

        public void AddEventHandler<TEventHandler>(TEventHandler handler, [CallerMemberName] string eventName = "")
            where TEventHandler : Delegate
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

        public void AddEventHandler<TEventArgs>(Action<TEventArgs> action, [CallerMemberName] string eventName = "")
            where TEventArgs : EventArgs
        {
            if (String.IsNullOrWhiteSpace(eventName))
            {
                throw new ArgumentNullException(nameof(eventName));
            }

            if (null == action)
            {
                throw new ArgumentNullException(nameof(action));
            }

            EventManagerService.AddEventHandler(eventName, action.Target, action.Method, handlers);
        }

        public void AddEventHandler(Action<EventArgs> action, [CallerMemberName] string eventName = "")
        {
            if (String.IsNullOrWhiteSpace(eventName))
            {
                throw new ArgumentNullException(nameof(eventName));
            }

            if (null == action)
            {
                throw new ArgumentNullException(nameof(action));
            }

            EventManagerService.AddEventHandler(eventName, action.Target, action.Method, handlers);
        }

        public void AddEventHandler(EventHandler handler, [CallerMemberName] string eventName = "")
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

        public void RemoveEventHandler<TEventArgs>(EventHandler<TEventArgs> handler, [CallerMemberName] string eventName = "")
            where TEventArgs : EventArgs
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

        public void RemoveEventHandler<TEventHandler>(TEventHandler handler, [CallerMemberName] string eventName = "")
            where TEventHandler : Delegate
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

        public void RemoveEventHandler<TEventArgs>(Action<TEventArgs> action, [CallerMemberName] string eventName = "")
            where TEventArgs : EventArgs
        {
            if (String.IsNullOrWhiteSpace(eventName))
            {
                throw new ArgumentNullException(nameof(eventName));
            }

            if (null == action)
            {
                throw new ArgumentNullException(nameof(action));
            }

            EventManagerService.RemoveEventHandler(eventName, action.Target, action.Method, handlers);
        }

        public void RemoveEventHandler(EventHandler action, [CallerMemberName] string eventName = "")
        {
            if (String.IsNullOrWhiteSpace(eventName))
            {
                throw new ArgumentNullException(nameof(eventName));
            }

            if (null == action)
            {
                throw new ArgumentNullException(nameof(action));
            }

            EventManagerService.RemoveEventHandler(eventName, action.Target, action.Method, handlers);
        }

        public void RemoveEventHandler(Action<EventArgs> action, [CallerMemberName] string eventName = "")
        {
            if (String.IsNullOrWhiteSpace(eventName))
            {
                throw new ArgumentNullException(nameof(eventName));
            }

            if (null == action)
            {
                throw new ArgumentNullException(nameof(action));
            }

            EventManagerService.RemoveEventHandler(eventName, action.Target, action.Method, handlers);
        }

        public void HandleEvent(object sender, EventArgs eventArgs, string eventName) => RaiseEvent(sender, eventArgs, eventName);

        public void HandleEvent(EventArgs eventArgs, string eventName) => RaiseEvent(eventArgs, eventName);

        public void RaiseEvent(object sender, EventArgs eventArgs, string eventName) => EventManagerService.HandleEvent(eventName, sender, eventArgs, handlers);

        public void RaiseEvent(EventArgs eventArgs, string eventName) => EventManagerService.HandleEvent(eventName, eventArgs, handlers);
    }
}
