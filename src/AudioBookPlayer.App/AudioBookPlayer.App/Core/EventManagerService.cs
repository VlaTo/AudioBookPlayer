using System;
using System.Collections.Generic;
using System.Reflection;

namespace AudioBookPlayer.App.Core
{
    internal struct Subscription
    {
        public WeakReference Subscriber
        {
            get;
        }

        public MethodInfo Handler
        {
            get;
        }

        public Subscription(WeakReference subscriber, MethodInfo handler)
        {
            if (null == handler)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            Subscriber = subscriber;
            Handler = handler;
        }
    }

    internal static class EventManagerService
    {
        public static void AddEventHandler(
            in string eventName,
            in object target,
            in MethodInfo methodInfo,
            in Dictionary<string, List<Subscription>> handlers)
        {
            if (false == handlers.TryGetValue(eventName, out var subscriptions) || null == subscriptions)
            {
                subscriptions = new List<Subscription>();
                handlers.Add(eventName, subscriptions);
            }

            if (null == target)
            {
                subscriptions.Add(new Subscription(null, methodInfo));
            }
            else
            {
                subscriptions.Add(new Subscription(new WeakReference(target), methodInfo));
            }
        }
        
        public static void RemoveEventHandler(
            in string eventName,
            in object target,
            in MemberInfo methodInfo,
            in Dictionary<string, List<Subscription>> handlers)
        {
            //var doesContainSubscriptions = handlers.TryGetValue(eventName, out var subscriptions);
            if (false == handlers.TryGetValue(eventName, out var subscriptions) || null == subscriptions)
            {
                return;
            }

            for (var index = subscriptions.Count; index > 0; index--)
            {
                var current = subscriptions[index - 1];
                var subscriber = current.Subscriber?.Target;

                if (subscriber != target || current.Handler.Name != methodInfo?.Name)
                {
                    continue;
                }

                subscriptions.Remove(current);

                break;
            }
        }

        public static void HandleEvent(
            in string eventName,
            in object sender,
            in object eventArgs,
            in Dictionary<string, List<Subscription>> handlers)
        {
            AddRemoveEvents(eventName, handlers, out var toRaise);

            for (var index = 0; index < toRaise.Count; index++)
            {
                try
                {
                    var (instance, handler) = toRaise[index];
                    /*if (eventHandler.IsLightweightMethod())
                    {
                        var method = TryGetDynamicMethod(eventHandler);
                        method?.Invoke(instance, new[] { sender, eventArgs });
                    }
                    else
                    {*/
                    //handler.CreateDelegate()
                        handler.Invoke(instance, new[] { sender, eventArgs });
                    //}
                }
                catch (TargetParameterCountException e)
                {
                    throw new Exception("Parameter count mismatch. If invoking an `event Action` use `HandleEvent(string eventName)` or if invoking an `event Action<T>` use `HandleEvent(object eventArgs, string eventName)`instead.", e);
                }
            }
        }
        public static void HandleEvent(
            in string eventName,
            in object actionEventArgs,
            in Dictionary<string, List<Subscription>> handlers)
        {
            AddRemoveEvents(eventName, handlers, out var toRaise);

            for (var index = 0; index < toRaise.Count; index++)
            {
                try
                {
                    var (instance, handler) = toRaise[index];
                    /*if (eventHandler.IsLightweightMethod())
                    {
                        var method = TryGetDynamicMethod(eventHandler);
                        method?.Invoke(instance, new[] { actionEventArgs });
                    }
                    else
                    {*/
                        handler.Invoke(instance, new[] { actionEventArgs });
                    //}
                }
                catch (TargetParameterCountException e)
                {
                    throw new Exception("Parameter count mismatch. If invoking an `event EventHandler` use `HandleEvent(object sender, TEventArgs eventArgs, string eventName)` or if invoking an `event Action` use `HandleEvent(string eventName)`instead.", e);
                }
            }
        }

        public static void HandleEvent(
            in string eventName,
            in Dictionary<string, List<Subscription>> eventHandlers)
        {
            AddRemoveEvents(eventName, eventHandlers, out var toRaise);

            for (var index = 0; index < toRaise.Count; index++)
            {
                try
                {
                    var (instance, eventHandler) = toRaise[index];
                    /*if (eventHandler.IsLightweightMethod())
                    {
                        var method = TryGetDynamicMethod(eventHandler);
                        method?.Invoke(instance, null);
                    }
                    else
                    {*/
                        eventHandler.Invoke(instance, null);
                    //}
                }
                catch (TargetParameterCountException e)
                {
                    throw new Exception("Parameter count mismatch. If invoking an `event EventHandler` use `HandleEvent(object sender, TEventArgs eventArgs, string eventName)` or if invoking an `event Action<T>` use `HandleEvent(object eventArgs, string eventName)`instead.", e);
                }
            }
        }

        private static void AddRemoveEvents(
            in string eventName,
            in Dictionary<string, List<Subscription>> eventHandlers,
            out List<(object Instance, MethodInfo EventHandler)> toRaise)
        {
            var toRemove = new List<Subscription>();

            toRaise = new List<(object, MethodInfo)>();

            //var doesContainEventName = eventHandlers.TryGetValue(eventName, out var target);
            if (false == eventHandlers.TryGetValue(eventName, out var target) || null == target)
            {
                return;
            }

            for (var index = 0; index < target.Count; index++)
            {
                var subscription = target[index];
                var isStatic = null == subscription.Subscriber;

                if (isStatic)
                {
                    toRaise.Add((null, subscription.Handler));
                    continue;
                }

                var subscriber = subscription.Subscriber?.Target;

                if (subscriber == null)
                {
                    toRemove.Add(subscription);
                }
                else
                {
                    toRaise.Add((subscriber, subscription.Handler));
                }
            }

            for (var index = 0; index < toRemove.Count; index++)
            {
                var subscription = toRemove[index];
                target.Remove(subscription);
            }
        }
        
        /*private static DynamicMethod TryGetDynamicMethod(in MethodInfo rtDynamicMethod)
        {
            var typeInfoRTDynamicMethod = typeof(DynamicMethod).GetTypeInfo().GetDeclaredNestedType("RTDynamicMethod");
            var typeRTDynamicMethod = typeInfoRTDynamicMethod?.AsType();

            return (typeInfoRTDynamicMethod?.IsAssignableFrom(rtDynamicMethod.GetType().GetTypeInfo()) ?? false) ?
                 (DynamicMethod)typeRTDynamicMethod.GetRuntimeFields().First(f => f.Name is "m_owner").GetValue(rtDynamicMethod)
                : null;
        }*/
    }
}
