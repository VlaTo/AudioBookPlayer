using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Input;
using Xamarin.Forms;

namespace LibraProgramming.Xamarin.Interactivity.Behaviors
{
    public sealed class EventToCommandBehavior : BehaviorBase<View>
    {
		public static readonly BindableProperty EventNameProperty;
		public static readonly BindableProperty CommandProperty;
		public static readonly BindableProperty CommandParameterProperty;
		public static readonly BindableProperty EventArgsConverterProperty;
		public static readonly BindableProperty EventArgsParameterPathProperty;

		private static readonly MethodInfo onEventMethod;
		private Delegate eventHandler;

		public string EventName
		{
			get => (string)GetValue(EventNameProperty);
			set => SetValue(EventNameProperty, value);
		}

		public ICommand Command
		{
			get => (ICommand)GetValue(CommandProperty);
			set => SetValue(CommandProperty, value);
		}

		public object CommandParameter
		{
			get => GetValue(CommandParameterProperty);
			set => SetValue(CommandParameterProperty, value);
		}

		public IValueConverter EventArgsConverter
		{
			get => (IValueConverter)GetValue(EventArgsConverterProperty);
			set => SetValue(EventArgsConverterProperty, value);
		}

		public string EventArgsParameterPath
		{
			get => (string)GetValue(EventArgsParameterPathProperty);
			set => SetValue(EventArgsParameterPathProperty, value);
		}

		static EventToCommandBehavior()
		{
			EventNameProperty = BindableProperty.Create(
				nameof(EventName),
				typeof(string),
				typeof(EventToCommandBehavior),
				propertyChanged: OnEventNamePropertyChanged,
				defaultValue: null
			);
			CommandProperty = BindableProperty.Create(
				nameof(Command),
				typeof(ICommand),
				typeof(EventToCommandBehavior),
				defaultValue: null
			);
			CommandParameterProperty = BindableProperty.Create(
				nameof(CommandParameter),
				typeof(object),
				typeof(EventToCommandBehavior),
				defaultValue: null
			);
			EventArgsConverterProperty = BindableProperty.Create(
				nameof(EventArgsConverter),
				typeof(IValueConverter),
				typeof(EventToCommandBehavior),
				defaultValue: null
			);
			EventArgsParameterPathProperty= BindableProperty.Create(
				nameof(EventArgsParameterPath),
				typeof(string),
				typeof(EventToCommandBehavior),
				defaultValue: null
			);

			onEventMethod = typeof(EventToCommandBehavior).GetMethod(nameof(OnEventInternal), BindingFlags.Instance | BindingFlags.NonPublic);

		}

		public EventToCommandBehavior()
        {
        }

        protected override void OnAttachedTo(View bindable)
        {
            base.OnAttachedTo(bindable);
			RegisterEvent(EventName);
        }

        protected override void OnDetachingFrom(View bindable)
        {
            base.OnDetachingFrom(bindable);
			ReleaseEvent(EventName);
        }

        private void RegisterEvent(string eventName)
        {
            if (String.IsNullOrEmpty(eventName))
            {
				return;
            }

			var ei = AttachedObject.GetType().GetRuntimeEvent(eventName);

            if (null == ei)
            {
				throw new ArgumentException("", nameof(eventName));
            }

			eventHandler = onEventMethod.CreateDelegate(ei.EventHandlerType, this);

			ei.AddEventHandler(AttachedObject, eventHandler);
        }

		private void ReleaseEvent(string eventName)
        {
			if (String.IsNullOrEmpty(eventName))
			{
				return;
			}

			if (null == eventHandler)
			{
				return;
			}

			var ei = AttachedObject.GetType().GetRuntimeEvent(eventName);

			if (null == ei)
			{
				throw new ArgumentException("", nameof(eventName));
			}

			ei.RemoveEventHandler(AttachedObject, eventHandler);

			eventHandler = null;
		}

		private void OnEventInternal(object sender, object ea)
        {
			var handler = Command;

			if (null == handler)
			{
				return;
			}

			object resolvedParameter = ea;

			if (null == CommandParameter)
			{
				if (false == String.IsNullOrEmpty(EventArgsParameterPath))
				{
					var propertyInfo = ea.GetType().GetRuntimeProperty(EventArgsParameterPath);

                    if (null != propertyInfo)
                    {
						resolvedParameter = propertyInfo.GetValue(ea);
                    }
				}
			}

			if (null != EventArgsConverter)
			{
				resolvedParameter = EventArgsConverter.Convert(resolvedParameter, typeof(object), null, null);
			}

			if (handler.CanExecute(resolvedParameter))
			{
				handler.Execute(resolvedParameter);
			}
		}

        private void OnEventNameChanged(string oldValue, string newValue)
		{
			if (null == AttachedObject)
			{
				return;
			}

			ReleaseEvent(oldValue);
			RegisterEvent(newValue);
		}

		private static void OnEventNamePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
			((EventToCommandBehavior)bindable).OnEventNameChanged((string)oldValue, (string)newValue);
        }
    }
}