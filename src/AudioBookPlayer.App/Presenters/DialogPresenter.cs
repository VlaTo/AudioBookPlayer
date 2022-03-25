using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using AudioBookPlayer.Core;
using AudioBookPlayer.MediaBrowserConnector;

#nullable enable

namespace AudioBookPlayer.App.Presenters
{
    internal class DialogPresenter
    {
        private readonly DialogAccessor dialogAccessor;
        private Dialog? dialog;

        public delegate Dialog DialogAccessor();

        public MediaBrowserServiceConnector Connector
        {
            get;
        }

        public Dialog Dialog => dialog ??= dialogAccessor.Invoke();

        protected DialogPresenter(DialogAccessor dialogAccessor)
        {
            this.dialogAccessor = dialogAccessor;
            Connector = MediaBrowserServiceConnector.GetInstance();
        }

        public virtual void AttachView(View? view)
        {

        }

        public virtual void DetachView()
        {

        }

        #region OnItemTouchListener

        private protected sealed class ItemTouchListener : Java.Lang.Object, RecyclerView.IOnItemTouchListener
        {
            private readonly IOnItemClickListener clickListener;
            private readonly GestureDetector gestureDetector;

            public interface IOnItemClickListener
            {
                void OnItemClick(View? view, int position);

                void OnLongItemClick(View? view, int position);
            }

            public ItemTouchListener(Context? context, RecyclerView recyclerView, IOnItemClickListener clickListener)
            {
                this.clickListener = clickListener;
                gestureDetector = new GestureDetector(context, new GestureListener(recyclerView, clickListener));
            }

            public bool OnInterceptTouchEvent(RecyclerView recyclerView, MotionEvent @event)
            {
                var childView = recyclerView.FindChildViewUnder(@event.GetX(), @event.GetY());

                if (null != childView && gestureDetector.OnTouchEvent(@event))
                {
                    var position = recyclerView.GetChildAdapterPosition(childView);

                    clickListener.OnItemClick(childView, position);

                    return true;
                }

                return false;
            }

            public void OnRequestDisallowInterceptTouchEvent(bool disallow)
            {
                ;
            }

            public void OnTouchEvent(RecyclerView recyclerView, MotionEvent @event)
            {
                ;
            }

            private sealed class GestureListener : GestureDetector.SimpleOnGestureListener
            {
                private readonly RecyclerView recyclerView;
                private readonly IOnItemClickListener clickListener;

                public GestureListener(RecyclerView recyclerView, IOnItemClickListener clickListener)
                {
                    this.recyclerView = recyclerView;
                    this.clickListener = clickListener;
                }

                public override bool OnSingleTapUp(MotionEvent? e) => true;

                public override void OnLongPress(MotionEvent? e)
                {
                    var child = null != e ? recyclerView.FindChildViewUnder(e.GetX(), e.GetY()) : null;

                    if (null == child)
                    {
                        return;
                    }

                    var position = recyclerView.GetChildAdapterPosition(child);

                    clickListener.OnLongItemClick(child, position);
                }
            }
        }

        #endregion

        #region ItemClickListener

        private protected sealed class ItemClickListener : ItemTouchListener.IOnItemClickListener
        {
            public Action<View?, int> OnItemClickImpl
            {
                get;
                set;
            }

            public Action<View?, int> OnLongItemClickImpl
            {
                get;
                set;
            }

            public ItemClickListener()
            {
                OnItemClickImpl = Stub.Nop<View?, int>();
                OnLongItemClickImpl = Stub.Nop<View?, int>();
            }

            public void OnItemClick(View? view, int position) => OnItemClickImpl.Invoke(view, position);

            public void OnLongItemClick(View? view, int position) => OnLongItemClickImpl.Invoke(view, position);
        }

        #endregion

        #region StartOffsetItemDecorator

        private protected sealed class StartOffsetItemDecoration : RecyclerView.ItemDecoration
        {
            private readonly int offset;

            public StartOffsetItemDecoration(int offset)
            {
                this.offset = offset;
            }

            public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
            {
                base.GetItemOffsets(outRect, view, parent, state);

                if (0 == parent.GetChildAdapterPosition(view) && parent.GetLayoutManager() is LinearLayoutManager layoutManager)
                {
                    switch (layoutManager.Orientation)
                    {
                        case LinearLayoutManager.Horizontal:
                            {
                                outRect.Left = offset;
                                break;
                            }

                        case LinearLayoutManager.Vertical:
                            {
                                outRect.Top = offset;
                                break;
                            }
                    }
                }
            }
        }

        #endregion

        #region EndOffsetItemDecorator

        private protected sealed class EndOffsetItemDecoration : RecyclerView.ItemDecoration
        {
            private readonly int offset;

            public EndOffsetItemDecoration(int offset)
            {
                this.offset = offset;
            }

            public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
            {
                base.GetItemOffsets(outRect, view, parent, state);

                var last = state.ItemCount - 1;

                if (last == parent.GetChildAdapterPosition(view) && parent.GetLayoutManager() is LinearLayoutManager layoutManager)
                {
                    switch (layoutManager.Orientation)
                    {
                        case LinearLayoutManager.Horizontal:
                            {
                                outRect.Right = offset;
                                break;
                            }

                        case LinearLayoutManager.Vertical:
                            {
                                outRect.Bottom = offset;
                                break;
                            }
                    }
                }
            }
        }

        #endregion
    }
}

#nullable restore