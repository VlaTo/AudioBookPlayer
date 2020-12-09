
using System;
using Xamarin.Forms;

namespace LibraProgramming.Xamarin.Popups.Platforms.Xamarin
{
    [Flags]
    public enum PaddingSides
    {
        Left = 1,
        Top = 2,
        Right = 4,
        Bottom = 8,

        All = Left | Top | Right | Bottom
    }

    public class PopupContentPage : ContentPage
    {
        public static readonly BindableProperty HasSystemPaddingProperty;
        public static readonly BindableProperty SystemPaddingProperty;
        public static readonly BindableProperty SystemPaddingSidesProperty;

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

        public PopupContentPage()
        {
            Content = new StackLayout
            {
                Children = {
                    new Label { Text = "Welcome to Xamarin.Forms!" }
                }
            };
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
    }
}