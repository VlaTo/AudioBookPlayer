using Android.Content;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views.Accessibility;
using Android.Widget;
using LibraProgramming.Xamarin.Controls.Platforms.Shared;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using View = global::Android.Views.View;
using ViewGroup = global::Android.Views.ViewGroup;
using TouchEffectCore = LibraProgramming.Xamarin.Controls.TouchEffect;

[assembly: ResolutionGroupName("LibraProgramming.Xamarin.Controls.Platforms.Xamarin")]
[assembly: ExportEffect(typeof(LibraProgramming.Xamarin.Controls.Platforms.Android.TouchEffect), nameof(LibraProgramming.Xamarin.Controls.Platforms.Android.TouchEffect))]

namespace LibraProgramming.Xamarin.Controls.Platforms.Android
{
    public sealed class TouchEffect : PlatformEffect
    {
        static readonly Color defaultNativeAnimationColor = Color.FromRgba(128, 128, 128, 64);

        //private global::Android.Views.View view;
        //private FrameLayout viewOverlay;
        private View viewOverlay;
        private RippleDrawable ripple;
        private TouchEffectCore effect;
        private AccessibilityManager accessibilityManager;
        private AccessibilityListener accessibilityListener;

        internal View View
        {
            get;
            private set;
        }

        internal bool IsAccessibilityMode =>
            accessibilityManager != null &&
            accessibilityManager.IsEnabled &&
            accessibilityManager.IsTouchExplorationEnabled;

        protected override void OnAttached()
        {
            if (Control is global::Android.Widget.ListView || Control is global::Android.Widget.ScrollView)
            {
                return;
            }

            View = Control ?? Container;

            if (null == View)
            {
                return;
            }

            effect = TouchEffectCore.GetFrom(Element);
            accessibilityManager = (AccessibilityManager)View.Context.GetSystemService(Context.AccessibilityService);

            effect.Element = (VisualElement)Element;

            TouchController.Add(View, OnTouch);

            UpdateClickHandler();

            if (null != accessibilityManager)
            {
                accessibilityListener = new AccessibilityListener(this);
                accessibilityManager.AddAccessibilityStateChangeListener(accessibilityListener);
                accessibilityManager.AddTouchExplorationStateChangeListener(accessibilityListener);
            }

            if (BuildVersionCodes.Lollipop <= Build.VERSION.SdkInt)
            {
                var group = (Container ?? Control) as ViewGroup;

                View.Clickable = true;
                View.LongClickable = true;

                viewOverlay = new FrameLayout(group.Context)
                {
                    Clickable = false,
                    Focusable = false,
                    LayoutParameters = new ViewGroup.LayoutParams(-1, -1),
                    Background = GetOrCreateRipple()
                };

                View.LayoutChange += OnViewLayoutChange;

                group.AddView(viewOverlay);
                viewOverlay.BringToFront();
            }
            else
            {
                viewOverlay = View;
                viewOverlay.Foreground = ripple;
            }
        }

        protected override void OnDetached()
        {
            if (null == effect?.Element)
            {
                return;
            }

            try
            {
                if (null != accessibilityManager)
                {
                    accessibilityManager.RemoveAccessibilityStateChangeListener(accessibilityListener);
                    accessibilityManager.RemoveTouchExplorationStateChangeListener(accessibilityListener);
                    
                    accessibilityListener.Dispose();

                    accessibilityManager = null;
                    accessibilityListener = null;
                }

                if (null != View)
                {
                    View.LayoutChange -= OnViewLayoutChange;
                    TouchController.Delete(View, OnTouch);

                    View.Click -= OnClick;
                }

                effect.Element = null;
                effect = null;

                if (null != viewOverlay)
                {
                    var group = (Container ?? Control) as ViewGroup;

                    viewOverlay.Pressed = false;
                    viewOverlay.Foreground = null;
                    viewOverlay.Background = null;

                    if (viewOverlay != View)
                    {
                        group.RemoveView(viewOverlay);
                        viewOverlay.Dispose();
                    }

                    viewOverlay = null;
                    ripple?.Dispose();
                }
            }
            catch (ObjectDisposedException)
            {

            }
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);

            if (//args.PropertyName == TouchEffect.IsAvailableProperty.PropertyName ||
                String.Equals(args.PropertyName, VisualElement.IsEnabledProperty.PropertyName))
            {
                UpdateClickHandler();
            }
        }

        private void OnTouch(View.TouchEventArgs e)
        {
            ;
        }

        private void OnClick(object sender, EventArgs args)
        {
            if (null == effect)
            {
                return;
            }

            if (false == IsAccessibilityMode)
            {
                return;
            }

            //IsCanceled = false;
            //HandleEnd(TouchStatus.Completed);
        }

        private RippleDrawable GetOrCreateRipple()
        {
            var background = View?.Background;

            if (background is RippleDrawable)
            {
                ripple = (RippleDrawable)background.GetConstantState().NewDrawable();
            }
            else
            {
                var noBackground = Element is Layout || null == background;
                
                ripple = new RippleDrawable(
                    GetColorStateList(),
                    noBackground ? null : background,
                    noBackground ? new ColorDrawable(Color.White.ToAndroid()) : null
                );

                UpdateRipple();
            }

            return ripple;
        }

        private void UpdateClickHandler()
        {
            View.Click -= OnClick;

            if (IsAccessibilityMode || (/*effect.IsAvailable &&*/ effect.Element.IsEnabled))
            {
                View.Click += OnClick;
            }
        }

        private void UpdateRipple()
        {
            ripple.SetColor(GetColorStateList());

            if (BuildVersionCodes.M <= Build.VERSION.SdkInt)
            {
                ripple.Radius = (int)(View.Context.Resources.DisplayMetrics.Density * 4);
            }
        }

        /*void HandleEnd(TouchStatus status)
        {
            if (IsCanceled)
                return;

            IsCanceled = true;
            if (effect.DisallowTouchThreshold > 0)
                Group?.Parent?.RequestDisallowInterceptTouchEvent(false);

            effect?.HandleTouch(status);
            effect?.HandleUserInteraction(TouchInteractionStatus.Completed);
            EndRipple();
        }*/

        private ColorStateList GetColorStateList()
        {
            var color = Color.Default;

            return new ColorStateList(
                new[] { new int[0] },
                new[] { (int)color.ToAndroid() }
            );
        }

        private void OnViewLayoutChange(object sender, View.LayoutChangeEventArgs e)
        {
            var group = (ViewGroup)sender;
            var viewGroup = (Container ?? Control) as ViewGroup;

            if (null == group || null == (viewGroup as IVisualElementRenderer)?.Element)
            {
                return; 
            }

            viewOverlay.Right = group.Width;
            viewOverlay.Bottom = group.Height;
        }

        /// <summary>
        /// AccessibilityListener class.
        /// </summary>
        internal class AccessibilityListener : Java.Lang.Object, AccessibilityManager.IAccessibilityStateChangeListener, AccessibilityManager.ITouchExplorationStateChangeListener
        {
            private TouchEffect effect;

            public AccessibilityListener(TouchEffect effect)
            {
                this.effect = effect;
            }

            public void OnAccessibilityStateChanged(bool enabled) => UpdateClickHandler();

            public void OnTouchExplorationStateChanged(bool enabled) => UpdateClickHandler();

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    effect = null;
                }

                base.Dispose(disposing);
            }

            private void UpdateClickHandler()
            {
                if (null != effect)
                {
                    effect.UpdateClickHandler();
                }
            }
        }
    }
}