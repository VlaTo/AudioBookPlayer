using System;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace LibraProgramming.Xamarin.Controls.Effects.Ripple
{
    // https://github.com/xamarin/XamarinCommunityToolkit/tree/main/src/CommunityToolkit/Xamarin.CommunityToolkit/Effects/Touch
    // https://github.com/mrxten/XamEffects/tree/master/src/XamEffects
    // https://docs.microsoft.com/en-us/xamarin/xamarin-forms/app-fundamentals/effects/
    public sealed class TouchEffect : RoutingEffect
    {
        public static readonly BindableProperty ShouldMakeChildrenInputTransparentProperty;
        public static readonly BindableProperty CommandProperty;
        public static readonly BindableProperty StateProperty;
        public static readonly BindableProperty StatusProperty;

        //private readonly GestureManager gestures;
        private readonly WeakEventManager eventManager;
        private VisualElement element;

        public bool ShouldMakeChildrenInputTransparent => GetShouldMakeChildrenInputTransparent(Element);

        public event EventHandler<TouchStateChangedEventArgs> StateChanged
        {
            add => eventManager.AddEventHandler(value);
            remove => eventManager.RemoveEventHandler(value);
        }

        public event EventHandler<TouchStatusChangedEventArgs> StatusChanged
        {
            add => eventManager.AddEventHandler(value);
            remove => eventManager.RemoveEventHandler(value);
        }

        public ICommand Command
        {
            get => GetCommand(Element);
            set => SetCommand(Element, value);
        }
        
        public TouchState State
        {
            get => GetState(Element);
            internal set => SetState(Element, value);
        }
        
        public TouchStatus Status
        {
            get => GetStatus(Element);
            internal set => SetStatus(Element, value);
        }

        internal new VisualElement Element
        {
            get => element;
            set
            {
                if (null != element)
                {
                    //gestures.Reset();
                    SetChildrenInputTransparent(false);
                }

                //gestures.AbortAnimations(this);
                element = value;

                if (null != element)
                {
                    SetChildrenInputTransparent(ShouldMakeChildrenInputTransparent);
                    ForceUpdateState(false);
                }
            }
        }

        public TouchEffect()
            : base($"LibraProgramming.Xamarin.Controls.Effects.Ripple.{nameof(TouchEffect)}")
        {
            eventManager = new WeakEventManager();
        }

        static TouchEffect()
        {
            CommandProperty = BindableProperty.CreateAttached(
                nameof(Command),
                typeof(ICommand),
                typeof(TouchEffect),
                null,
                propertyChanged: OnCommandPropertyChanged
            );
            StateProperty = BindableProperty.CreateAttached(
                nameof(State),
                typeof(TouchState),
                typeof(TouchEffect),
                TouchState.Normal,
                BindingMode.OneWayToSource
            );
            StatusProperty = BindableProperty.CreateAttached(
                nameof(Status),
                typeof(TouchStatus),
                typeof(TouchEffect),
                TouchStatus.Completed,
                BindingMode.OneWayToSource
            );
            ShouldMakeChildrenInputTransparentProperty = BindableProperty.CreateAttached(
                nameof(ShouldMakeChildrenInputTransparent),
                typeof(bool),
                typeof(TouchEffect),
                true,
                propertyChanged: OnShouldMakeChildrenInputTransparentPropertyChanged
            );
        }

        public static bool GetShouldMakeChildrenInputTransparent(BindableObject bindable)
            => (bool) bindable.GetValue(ShouldMakeChildrenInputTransparentProperty);

        public static void SetShouldMakeChildrenInputTransparent(BindableObject bindable, bool value)
            => bindable.SetValue(ShouldMakeChildrenInputTransparentProperty, value);

        public static ICommand GetCommand(BindableObject bindable)
            => (ICommand) bindable.GetValue(CommandProperty);

        public static void SetCommand(BindableObject bindable, ICommand value)
            => bindable.SetValue(CommandProperty, value);

        public static TouchState GetState(BindableObject bindable)
            => (TouchState) bindable.GetValue(StateProperty);

        public static void SetState(BindableObject bindable, TouchState value)
            => bindable.SetValue(StateProperty, value);

        public static TouchStatus GetStatus(BindableObject bindable)
            => (TouchStatus) bindable.GetValue(StatusProperty);

        public static void SetStatus(BindableObject bindable, TouchStatus value)
            => bindable.SetValue(StatusProperty, value);

        internal static TouchEffect GetFrom(BindableObject bindable)
        {
            if (bindable is VisualElement visual)
            {
                var effects = visual.Effects.OfType<TouchEffect>();
                return effects.FirstOrDefault();
            }

            return null;
        }
        
        internal void ForceUpdateState(bool animated = true)
        {
            if (null != Element)
            {
                //_ = gestureManager.ChangeStateAsync(this, animated);
            }
        }

        internal void HandleTouch(TouchStatus status)
        {
            //gestureManager.HandleTouch(this, status);
        }

        internal void RaiseStateChanged()
        {
            ForceUpdateState();
            eventManager.HandleEvent(Element, new TouchStateChangedEventArgs(State), nameof(StateChanged));
        }
        
        internal void RaiseStatusChanged()
        {
            eventManager.HandleEvent(Element, new TouchStatusChangedEventArgs(Status), nameof(StatusChanged));
        }

        private void OnLayoutChildAdded(object sender, ElementEventArgs e)
        {
            if (e.Element is View view)
            {
                view.InputTransparent = ShouldMakeChildrenInputTransparent /*&& !(GetFrom(view)?.IsAvailable ?? false)*/;
            }
        }

        private void SetChildrenInputTransparent(bool value)
        {
            if (Element is Layout layout)
            {
                layout.ChildAdded -= OnLayoutChildAdded;

                if (false == value)
                {
                    return;
                }

                layout.InputTransparent = false;

                foreach (var child in layout.Children)
                {
                    OnLayoutChildAdded(layout, new ElementEventArgs(child));
                }

                layout.ChildAdded += OnLayoutChildAdded;
            }
        }

        private void OnCommandChanged(ICommand oldvalue, ICommand newvalue)
        {
            ;
        }

        private static void OnShouldMakeChildrenInputTransparentPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            //((TouchEffect)bindable).OnShouldMakeChildrenInputTransparentChanged((bool)oldValue, (bool)newValue);
            var effect = GetFrom(bindable);

            if (null != effect)
            {
                effect.SetChildrenInputTransparent((bool)newValue);
            }

            //TryGenerateEffect(bindable, oldValue, newValue);
        }

        private static void OnCommandPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var effect = GetFrom(bindable);

            if (null != effect)
            {
                effect.OnCommandChanged((ICommand) oldvalue, (ICommand) newvalue);
            }
        }
    }
}
