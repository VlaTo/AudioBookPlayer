using System;
using Xamarin.Forms;

namespace LibraProgramming.Xamarin.Popups.Views
{
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum PaddingSides
    {
        Left = 1,
        Top = 2,
        Right = 4,
        Bottom = 8,

        All = Left | Top | Right | Bottom
    }

    /// <summary>
    /// 
    /// </summary>
    public class PopupContentPage : ContentPage
    {
        public static readonly BindableProperty HasSystemPaddingProperty;
        public static readonly BindableProperty SystemPaddingProperty;
        public static readonly BindableProperty SystemPaddingSidesProperty;
        public static readonly BindableProperty HasKeyboardOffsetProperty;
        public static readonly BindableProperty KeyboardOffsetProperty;

        public bool HasSystemPadding
        {
            get => (bool)GetValue(HasSystemPaddingProperty);
            set => SetValue(HasSystemPaddingProperty, value);
        }

        public Thickness SystemPadding
        {
            get => (Thickness)GetValue(SystemPaddingProperty);
            set => SetValue(SystemPaddingProperty, value);
        }

        public PaddingSides SystemPaddingSides
        {
            get => (PaddingSides)GetValue(SystemPaddingSidesProperty);
            set => SetValue(SystemPaddingSidesProperty, value);
        }

        public bool HasKeyboardOffset
        {
            get => (bool)GetValue(HasKeyboardOffsetProperty);
            set => SetValue(HasKeyboardOffsetProperty, value);
        }

        public double KeyboardOffset
        {
            get => (double)GetValue(KeyboardOffsetProperty);
            set => SetValue(KeyboardOffsetProperty, value);
        }

        public PopupContentPage()
        {
        }

        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            if (HasSystemPadding)
            {
                var padding = SystemPadding;
                var sides = SystemPaddingSides;
                
                if (sides.HasFlag(PaddingSides.Left))
                {
                    x += padding.Left;
                    width -= padding.Left;
                }

                if (sides.HasFlag(PaddingSides.Top))
                {
                    y += padding.Top;
                    height -= padding.Top;
                }

                if (sides.HasFlag(PaddingSides.Right))
                {
                    width -= padding.Right;
                }

                if (sides.HasFlag(PaddingSides.Bottom))
                {
                    height -= padding.Bottom;
                }
            }

            base.LayoutChildren(x, y, width, height);
        }

        static PopupContentPage()
        {
            HasSystemPaddingProperty = BindableProperty.Create(
                nameof(HasSystemPadding),
                typeof(bool),
                typeof(PopupContentPage),
                propertyChanged: OnHasSystemPaddingPropertyChanged,
                defaultValue: true
            );
            SystemPaddingProperty = BindableProperty.Create(
                nameof(SystemPadding),
                typeof(Thickness),
                typeof(PopupContentPage),
                propertyChanged: OnSystemPaddingPropertyChanged,
                defaultValue: new Thickness()
            );
            SystemPaddingSidesProperty = BindableProperty.Create(
                nameof(SystemPaddingSides),
                typeof(PaddingSides),
                typeof(PopupContentPage),
                propertyChanged: OnSystemPaddingSidesPropertyChanged,
                defaultValue: PaddingSides.All
            );
            HasKeyboardOffsetProperty = BindableProperty.Create(
                nameof(HasKeyboardOffset),
                typeof(bool),
                typeof(PopupContentPage),
                propertyChanged: OnHasKeyboardOffsetPropertyChanged,
                defaultValue: true
            );
            KeyboardOffsetProperty = BindableProperty.Create(
                nameof(KeyboardOffset),
                typeof(double),
                typeof(PopupContentPage),
                propertyChanged: OnKeyboardOffsetPropertyChanged,
                defaultBindingMode: BindingMode.OneWayToSource,
                defaultValue: 0.0d
            );
        }

        private void OnHasSystemPaddingChanged()
        {
            ;
        }

        private void OnSystemPaddingChanged()
        {
            ;
        }

        private void OnSystemPaddingSidesChanged()
        {
            ;
        }

        private void OnHasKeyboardOffsetChanged()
        {
            ;
        }

        private void OnKeyboardOffsetChanged()
        {
            ;
        }

        private static void OnHasSystemPaddingPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((PopupContentPage)bindable).OnHasSystemPaddingChanged();
        }

        private static void OnSystemPaddingPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((PopupContentPage)bindable).OnSystemPaddingChanged();
        }

        private static void OnSystemPaddingSidesPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((PopupContentPage)bindable).OnSystemPaddingSidesChanged();
        }

        private static void OnHasKeyboardOffsetPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((PopupContentPage)bindable).OnHasKeyboardOffsetChanged();
        }

        private static void OnKeyboardOffsetPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((PopupContentPage)bindable).OnKeyboardOffsetChanged();
        }
    }
}