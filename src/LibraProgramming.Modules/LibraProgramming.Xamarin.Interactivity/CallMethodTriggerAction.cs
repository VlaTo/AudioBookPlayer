using LibraProgramming.Xamarin.Interaction;
using LibraProgramming.Xamarin.Interaction.Contracts;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xamarin.Forms;

namespace LibraProgramming.Xamarin.Interactivity
{
    public class CallMethodTriggerAction : RequestTriggerAction
    {
        public static readonly BindableProperty MethodNameProperty;

        public string MethodName
        {
            get => (string)GetValue(MethodNameProperty);
            set => SetValue(MethodNameProperty, value);
        }

        public CallMethodTriggerAction()
        {
        }

        static CallMethodTriggerAction()
        {
            MethodNameProperty = BindableProperty.Create(
                nameof(MethodName),
                typeof(string),
                typeof(CallMethodTriggerAction),
                null,
                propertyChanged: OnMethodNamePropertyChanged
            );
        }

        public override void Invoke(IInteractionRequest request, InteractionRequestContext context, Action callback)
        {
            var target = AttachedObject;

            if (null != target)
            {
                var methodName = MethodName;

                if (String.IsNullOrEmpty(methodName))
                {
                    return;
                }


                var targetType = target.GetType();
                var method = targetType.GetMethod(MethodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                if (null != method)
                {
                    var parameters = method.GetParameters();
                    var args = new List<object>();

                    for (var index = 0; index < parameters.Length; index++)
                    {
                        var temp = parameters[index];

                        if (temp.ParameterType.IsAssignableFrom(typeof(InteractionRequestContext)))
                        {
                            args.Add(context);
                            continue;
                        }

                        if (temp.ParameterType.IsAssignableFrom(typeof(Action)))
                        {
                            args.Add(callback);
                            continue;
                        }

                        throw new Exception();
                    }

                    method.Invoke(target, args.ToArray());
                }
            }
        }

        private void OnMethodNameChanged()
        {
            ;
        }

        private static void OnMethodNamePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((CallMethodTriggerAction)bindable).OnMethodNameChanged();
        }
    }

    public sealed class CallMethodTriggerAction<TContext> : CallMethodTriggerAction
        where TContext : InteractionRequestContext
    {
        public CallMethodTriggerAction()
        {
        }

        public override void Invoke(IInteractionRequest request, InteractionRequestContext context, Action callback)
        {
            var target = AttachedObject;

            if (null != target)
            {
                var methodName = MethodName;

                if (String.IsNullOrEmpty(methodName))
                {
                    return;
                }

                var targetType = target.GetType();
                var method = targetType.GetMethod(MethodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                if (null != method)
                {
                    var parameters = method.GetParameters();
                    var args = new List<object>();

                    for (var index = 0; index < parameters.Length; index++)
                    {
                        var temp = parameters[index];

                        if (temp.ParameterType.IsAssignableFrom(typeof(TContext)))
                        {
                            args.Add(context);
                            continue;
                        }

                        if (temp.ParameterType.IsAssignableFrom(typeof(Action)))
                        {
                            args.Add(callback);
                            continue;
                        }

                        throw new Exception();
                    }

                    method.Invoke(target, args.ToArray());
                }
            }
        }
    }
}
