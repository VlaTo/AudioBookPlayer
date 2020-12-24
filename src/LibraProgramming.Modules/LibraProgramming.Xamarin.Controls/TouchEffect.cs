using System.Linq;
using Xamarin.Forms;

namespace LibraProgramming.Xamarin.Controls
{
    // https://github.com/xamarin/XamarinCommunityToolkit/tree/main/src/CommunityToolkit/Xamarin.CommunityToolkit/Effects/Touch
    // https://github.com/mrxten/XamEffects/tree/master/src/XamEffects
    // https://docs.microsoft.com/en-us/xamarin/xamarin-forms/app-fundamentals/effects/
    public sealed class TouchEffect : RoutingEffect
    {
        public static readonly BindableProperty ShouldMakeChildrenInputTransparentProperty;

        //private readonly GestureManager gestures;
        private VisualElement element;

        public bool ShouldMakeChildrenInputTransparent => GetShouldMakeChildrenInputTransparent(Element);

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
            : base($"LibraProgramming.Xamarin.Controls.Platforms.Xamarin.{nameof(TouchEffect)}")
        {
        }

        static TouchEffect()
        {
            ShouldMakeChildrenInputTransparentProperty = BindableProperty.CreateAttached(
                nameof(ShouldMakeChildrenInputTransparent),
                typeof(bool),
                typeof(TouchEffect),
                true,
                propertyChanged: OnShouldMakeChildrenInputTransparentPropertyChanged
            );
        }

        public static bool GetShouldMakeChildrenInputTransparent(BindableObject bindable)
        {
            return (bool)bindable.GetValue(ShouldMakeChildrenInputTransparentProperty);
        }

        public static void SetShouldMakeChildrenInputTransparent(BindableObject bindable, bool value)
        {
            bindable.SetValue(ShouldMakeChildrenInputTransparentProperty, value);
        }

        internal static TouchEffect GetFrom(BindableObject bindable)
        {
            if (bindable is VisualElement visual && null != visual)
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

        private void OnLayoutChildAdded(object sender, ElementEventArgs e)
        {
            if (e.Element is View view && null != view)
            {
                view.InputTransparent = ShouldMakeChildrenInputTransparent /*&& !(GetFrom(view)?.IsAvailable ?? false)*/;
            }
        }

        private void SetChildrenInputTransparent(bool value)
        {
            if (Element is Layout layout && null != layout)
            {
                layout.ChildAdded -= OnLayoutChildAdded;

                if (false == value)
                {
                    return;
                }

                layout.InputTransparent = false;

                foreach (var element in layout.Children)
                {
                    OnLayoutChildAdded(layout, new ElementEventArgs(element));
                }

                layout.ChildAdded += OnLayoutChildAdded;
            }
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
    }
}
