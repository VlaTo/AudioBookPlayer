using Android.Content;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views.Accessibility;
using Android.Widget;
using System;
using System.ComponentModel;
using System.Drawing;
using Android.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Color = Xamarin.Forms.Color;
using ListView = global::Android.Widget.ListView;
using Point = Xamarin.Forms.Point;
using Rectangle = Xamarin.Forms.Rectangle;
using ScrollView = global::Android.Widget.ScrollView;
using View = global::Android.Views.View;
using ViewGroup = global::Android.Views.ViewGroup;

[assembly: ResolutionGroupName("LibraProgramming.Xamarin.Controls.Effects.Ripple")]
[assembly: ExportEffect(typeof(LibraProgramming.Xamarin.Controls.Effects.Ripple.PlatformTouchEffect), nameof(LibraProgramming.Xamarin.Controls.Effects.Ripple.PlatformTouchEffect))]

namespace LibraProgramming.Xamarin.Controls.Effects.Ripple
{
    public sealed class PlatformTouchEffect : PlatformEffect
    {
        static readonly Color defaultNativeAnimationColor = Color.FromRgba(128, 128, 128, 64);

        //private global::Android.Views.View view;
        //private FrameLayout viewOverlay;
        private View viewOverlay;
        private RippleDrawable ripple;
        private TouchEffect effect;
        private AccessibilityManager accessibilityManager;
        private AccessibilityListener accessibilityListener;
        private PointF origin;

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
            if (Control is ListView || Control is ScrollView)
            {
                return;
            }

            View = Control ?? Container;

            if (null == View)
            {
                return;
            }

            effect = TouchEffect.GetFrom(Element);
            accessibilityManager = (AccessibilityManager) View.Context.GetSystemService(Context.AccessibilityService);

            effect.Element = (VisualElement) Element;

            View.Touch += OnTouch;

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
                    View.Touch -= OnTouch;
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

        private void OnTouch(object sender, View.TouchEventArgs e)
        {
            switch (e.Event.ActionMasked)
            {
                case MotionEventActions.Down:
                {
                    OnTouchDown(e);
                    //_activeView = view;
                    View.PlaySoundEffect(SoundEffects.Click);

                    break;
                }

                case MotionEventActions.Up:
                case MotionEventActions.Cancel:
                {
                    //_activeView = null;
                    e.Handled = true;

                    OnTouchUp(e);

                    break;
                }
            }
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
            HandleEnd(TouchStatus.Completed);
        }

        private void UpdateClickHandler()
        {
            View.Click -= OnClick;

            if (IsAccessibilityMode || (/*effect.IsAvailable &&*/ effect.Element.IsEnabled))
            {
                View.Click += OnClick;
            }
        }

        #region Ripple

        private RippleDrawable GetOrCreateRipple()
        {
            var background = (BuildVersionCodes.M <= Build.VERSION.SdkInt)
                ? View?.Background
                : View?.Foreground;

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

        private void StartRipple(PointF position)
        {
            /*if (effect?.IsDisabled ?? true)
                return;*/

            /*if (effect.CanExecute && effect.NativeAnimation)
            {
                UpdateRipple();

                if (viewOverlay != null)
                {
                    viewOverlay.Enabled = true;
                    viewOverlay.BringToFront();
                    ripple.SetHotspot(position.X, position.Y);
                    viewOverlay.Pressed = true;
                }
                else if (IsForegroundRippleWithTapGestureRecognizer)
                {
                    ripple.SetHotspot(position.X, position.Y);
                    View.Pressed = true;
                }
            }*/
        }

        private void UpdateRipple()
        {
            ripple.SetColor(GetColorStateList());

            if (BuildVersionCodes.M <= Build.VERSION.SdkInt)
            {
                ripple.Radius = (int)(View.Context.Resources.DisplayMetrics.Density * 4);
            }
        }

        private void EndRipple()
        {
            /*if (effect?.IsDisabled ?? true)
                return;*/

            if (null != View)
            {
                if (View.Pressed)
                {
                    View.Pressed = false;
                    View.Enabled = false;
                }
            }
            else /*if (IsForegroundRippleWithTapGestureRecognizer)*/
            {
                if (View.Pressed)
                {
                    View.Pressed = false;
                }
            }
        }

        #endregion

        private void HandleEnd(TouchStatus status)
        {
            /*if (IsCanceled)
                return;

            IsCanceled = true;
            if (effect.DisallowTouchThreshold > 0)
                Group?.Parent?.RequestDisallowInterceptTouchEvent(false);*/

            effect.HandleTouch(status);
            // effect?.HandleUserInteraction(TouchInteractionStatus.Completed);
            EndRipple();
        }

        /*private void OnClick(object sender, EventArgs e)
        {

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

        private void OnTouchDown(View.TouchEventArgs e)
        {
            origin = new PointF(e.Event.GetX(), e.Event.GetY());

            effect.HandleTouch(TouchStatus.Started);

            StartRipple(origin);

            /*if (effect.DisallowTouchThreshold > 0)
            {
                group.Parent?.RequestDisallowInterceptTouchEvent(true);
            }*/
        }

        private void OnTouchUp(View.TouchEventArgs e)
        {
            HandleEnd(effect.Status == TouchStatus.Started ? TouchStatus.Completed : TouchStatus.Canceled);
        }

        private void OnTouchCancel()
        {
            HandleEnd(TouchStatus.Canceled);
        }

        private void OnTouchMove(View.TouchEventArgs e)
        {
            if ( /*IsCanceled || */e.Event == null)
            {
                return;
            }

            var position = new PointF(e.Event.GetX(), e.Event.GetY());

            var diffX = Math.Abs(position.X - origin.X) / View.Context?.Resources?.DisplayMetrics?.Density ?? throw new NullReferenceException();
            var diffY = Math.Abs(position.Y - origin.Y) / View.Context?.Resources?.DisplayMetrics?.Density ?? throw new NullReferenceException();
            var maxDiff = Math.Max(diffX, diffY);

            /*var disallowTouchThreshold = effect?.DisallowTouchThreshold;
            if (disallowTouchThreshold > 0 && maxDiff > disallowTouchThreshold)
            {
                HandleEnd(TouchStatus.Canceled);
                return;
            }

            if (sender is not AView view)
                return;*/

            var screenPointerCoords = new Point(View.Left + position.X, View.Top + position.Y);
            var viewRect = new Rectangle(View.Left, View.Top, View.Right - View.Left, View.Bottom - View.Top);
            var status = viewRect.Contains(screenPointerCoords) ? TouchStatus.Started : TouchStatus.Canceled;

            /*if (isHoverSupported && ((status == TouchStatus.Canceled && effect?.HoverStatus == HoverStatus.Entered)
                                     || (status == TouchStatus.Started && effect?.HoverStatus == HoverStatus.Exited)))
                effect?.HandleHover(status == TouchStatus.Started ? HoverStatus.Entered : HoverStatus.Exited);*/

            if (status != effect.Status)
            {
                effect.HandleTouch(status);

                if (status == TouchStatus.Started)
                {
                    StartRipple(position);
                }

                if (status == TouchStatus.Canceled)
                {
                    EndRipple();
                }
            }
        }

        /// <summary>
        /// AccessibilityListener class.
        /// </summary>
        internal class AccessibilityListener : Java.Lang.Object, AccessibilityManager.IAccessibilityStateChangeListener, AccessibilityManager.ITouchExplorationStateChangeListener
        {
            private PlatformTouchEffect effect;

            public AccessibilityListener(PlatformTouchEffect effect)
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