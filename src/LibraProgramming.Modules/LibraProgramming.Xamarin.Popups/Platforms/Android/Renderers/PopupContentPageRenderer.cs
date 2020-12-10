using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using LibraProgramming.Xamarin.Popups.Platforms.Android.Renderers;
using LibraProgramming.Xamarin.Popups.Platforms.Xamarin;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Point = Xamarin.Forms.Point;

[assembly: ExportRenderer(typeof(PopupContentPage), typeof(PopupContentPageRenderer))]

namespace LibraProgramming.Xamarin.Popups.Platforms.Android.Renderers
{
    [Preserve(AllMembers = true)]
    public sealed class PopupContentPageRenderer : PageRenderer
    {
        //private readonly RgGestureDetectorListener _gestureDetectorListener;
        //private readonly GestureDetector _gestureDetector;
        private DateTime _downTime;
        private Point _downPosition;
        private bool disposed;

        private PopupContentPage CurrentElement => (PopupContentPage)Element;

        public PopupContentPageRenderer(Context context)
            : base(context)
        {
            //_gestureDetectorListener = new RgGestureDetectorListener();

            //_gestureDetectorListener.Clicked += OnBackgroundClick;

            //_gestureDetector = new GestureDetector(Context, _gestureDetectorListener);
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                disposed = true;

                //_gestureDetectorListener.Clicked -= OnBackgroundClick;
                //_gestureDetectorListener.Dispose();
                //_gestureDetector.Dispose();
            }

            base.Dispose(disposing);
        }

        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            Thickness systemPadding;
            var keyboardOffset = 0.0d;

            var view = Popup.GetContentView();
            var visibleRect = new global::Android.Graphics.Rect();

            if (null != view)
            {
                view.GetWindowVisibleDisplayFrame(visibleRect);
            }

            if (Build.VERSION.SdkInt >= BuildVersionCodes.M && null != RootWindowInsets)
            {
                var screenRealSize = new global::Android.Graphics.Point();

                TryGetDisplayRealSize(ref screenRealSize);

                var windowInsets = RootWindowInsets;
                var bottomPadding = Math.Min(windowInsets.StableInsetBottom, windowInsets.SystemWindowInsetBottom);

                if (screenRealSize.Y - visibleRect.Bottom > windowInsets.StableInsetBottom)
                {
                    keyboardOffset = Context.FromPixels(screenRealSize.Y - visibleRect.Bottom);
                }

                systemPadding = new Thickness
                {
                    Left = Context.FromPixels(windowInsets.SystemWindowInsetLeft),
                    Top = Context.FromPixels(windowInsets.SystemWindowInsetTop),
                    Right = Context.FromPixels(windowInsets.SystemWindowInsetRight),
                    Bottom = Context.FromPixels(bottomPadding)
                };
            }
            else if (Build.VERSION.SdkInt < BuildVersionCodes.M && null != view)
            {
                var screenSize = new global::Android.Graphics.Point();

                TryGetDisplaySize(ref screenSize);

                var keyboardHeight = 0d;

                var decoreHeight = view.Height;
                var decoreWidht = view.Width;

                if (visibleRect.Bottom < screenSize.Y)
                {
                    keyboardHeight = screenSize.Y - visibleRect.Bottom;
                    keyboardOffset = Context.FromPixels(decoreHeight - visibleRect.Bottom);
                }

                systemPadding = new Thickness
                {
                    Left = Context.FromPixels(visibleRect.Left),
                    Top = Context.FromPixels(visibleRect.Top),
                    Right = Context.FromPixels(decoreWidht - visibleRect.Right),
                    Bottom = Context.FromPixels(decoreHeight - visibleRect.Bottom - keyboardHeight)
                };
            }
            else
            {
                systemPadding = new Thickness();
            }

            CurrentElement.SetValue(PopupContentPage.SystemPaddingProperty, systemPadding);
            CurrentElement.SetValue(PopupContentPage.KeyboardOffsetProperty, keyboardOffset);

            if (changed)
            {
                var rect = new Rectangle(
                    Context.FromPixels(left),
                    Context.FromPixels(top),
                    Context.FromPixels(right),
                    Context.FromPixels(bottom)
                );

                CurrentElement.Layout(rect);
            }
            else
            {
                CurrentElement.ForceLayout();
            }

            base.OnLayout(changed, left, top, right, bottom);
        }

        protected override void OnAttachedToWindow()
        {
            var view = Popup.GetContentView();

            //Context.HideKeyboard(((Activity)Context)?.Window?.DecorView);
            Context.HideKeyboard(view);

            base.OnAttachedToWindow();
        }

        protected override void OnDetachedFromWindow()
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(0), () =>
            {
                var view = Popup.GetContentView();
                
                //Popup.Context.HideKeyboard(((Activity)Popup.Context)?.Window?.DecorView);
                Popup.Context.HideKeyboard(view);

                return false;
            });

            base.OnDetachedFromWindow();
        }
        
        protected override void OnWindowVisibilityChanged(ViewStates visibility)
        {
            base.OnWindowVisibilityChanged(visibility);

            // It is needed because a size of popup has not updated on Android 7+. See #209
            if (ViewStates.Visible == visibility)
            {
                RequestLayout();
            }
        }

        private bool TryGetDisplayRealSize(ref global::Android.Graphics.Point size)
        {
            var activity = (Activity)Context;

            if (null != activity && null != activity.WindowManager)
            {
                var display = activity.WindowManager.DefaultDisplay;

                if (null != display)
                {
                    display.GetRealSize(size);
                    return true;
                }
            }

            return false;

        }

        private bool TryGetDisplaySize(ref global::Android.Graphics.Point size)
        {
            var activity = (Activity)Context;

            if (null != activity && null != activity.WindowManager)
            {
                var display = activity.WindowManager.DefaultDisplay;

                if (null != display)
                {
                    display.GetSize(size);
                    return true;
                }
            }

            return false;

        }
    }
}
