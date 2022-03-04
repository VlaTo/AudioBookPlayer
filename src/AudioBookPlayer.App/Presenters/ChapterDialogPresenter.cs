using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using AudioBookPlayer.Core;
using AudioBookPlayer.MediaBrowserConnector;
using System;
using System.Collections.Generic;
using Android.Util;
using Xamarin.Essentials;
using Object = Java.Lang.Object;

#nullable enable

namespace AudioBookPlayer.App.Presenters
{
    internal class ChapterDialogPresenter : MediaBrowserServiceConnector.IConnectCallback, MediaService.IMediaServiceListener
    {
        private readonly MediaBrowserServiceConnector connector;
        private readonly Func<Dialog> dialogProvider;
        private GroupedAdapter? adapter;
        private MediaService? mediaService;
        private RecyclerView? recyclerView;

        public ChapterDialogPresenter(Func<Dialog> dialogProvider)
        {
            this.dialogProvider = dialogProvider;
            connector = MediaBrowserServiceConnector.GetInstance();
        }

        public void AttachView(View? view)
        {
            if (null == view)
            {
                return;
            }

            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.list_view_recycler);

            if (null != recyclerView)
            {
                var context = Application.Context;
                var clickListener = new ItemClickListener
                {
                    OnItemClickImpl = OnItemClick,
                    OnLongItemClickImpl = OnLongItemClick
                };
                var offset = TypedValue.ApplyDimension(ComplexUnitType.Dip, 16.0f, view.Resources?.DisplayMetrics);

                adapter = new GroupedAdapter();
                recyclerView.SetLayoutManager(new LinearLayoutManager(context));
                recyclerView.SetAdapter(adapter);
                recyclerView.AddOnItemTouchListener(new ItemTouchListener(context, recyclerView, clickListener));
                recyclerView.AddItemDecoration(new StartOffsetItemDecoration((int)offset));
                recyclerView.AddItemDecoration(new EndOffsetItemDecoration((int)offset));
            }

            var dialog = dialogProvider.Invoke();

            dialog.SetCanceledOnTouchOutside(true);

            var window = dialog.Window;

            if (null != window)
            {
                window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
                window.RequestFeature(WindowFeatures.NoTitle);
                window.SetGravity(GravityFlags.CenterHorizontal | GravityFlags.CenterVertical);
                window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            }

            connector.Connect(this);
        }

        public void DetachView()
        {
            if (null != mediaService)
            {
                mediaService.RemoveListener(this);
            }

            connector.Disconnect(this);
        }

        private void SetMediaQueue()
        {
            if (null != adapter && null != mediaService)
            {
                adapter.SetQueue(mediaService.MediaQueue);
            }
        }

        private void SetActiveQueueItem()
        {
            if (null != adapter && null != mediaService)
            {
                adapter.SetActiveQueueItem(mediaService.ActiveQueueItemId);
            }
        }

        private void OnItemClick(View? view, int position)
        {
            if (null != adapter && null != mediaService)
            {
                var queueItem = adapter.GetItem(position);

                if (queueItem is ChapterItem chapterItem)
                {
                    var shouldClose = Preferences.Get("Chapters.CloseDialogAfterTap", false);

                    mediaService.ActiveQueueItemId = chapterItem.QueueId;

                    if (shouldClose)
                    {
                        var dialog = dialogProvider.Invoke();
                        dialog.Cancel();
                    }
                }
            }
        }

        private void OnLongItemClick(View? view, int position)
        {
            ;
        }

        #region MediaBrowserServiceConnector.IConnectCallback

        void MediaBrowserServiceConnector.IConnectCallback.OnConnected(MediaService service)
        {
            mediaService = service;

            if (null != mediaService)
            {
                SetMediaQueue();
                SetActiveQueueItem();
                mediaService.AddListener(this);
            }
        }

        void MediaBrowserServiceConnector.IConnectCallback.OnSuspended()
        {
            throw new NotImplementedException();
        }

        void MediaBrowserServiceConnector.IConnectCallback.OnFailed()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region MediaService.IMediaServiceListener

        void MediaService.IMediaServiceListener.OnMetadataChanged(MediaMetadataCompat metadata)
        {
            ;
        }

        void MediaService.IMediaServiceListener.OnQueueTitleChanged(string title)
        {
            ;
        }

        void MediaService.IMediaServiceListener.OnQueueChanged()
        {
            SetMediaQueue();
        }

        void MediaService.IMediaServiceListener.OnPlaybackStateChanged()
        {
            SetActiveQueueItem();
        }

        #endregion

        #region ChapterViewHolder

        private sealed class ChapterViewHolder : RecyclerView.ViewHolder
        {
            private readonly TextView? textView;
            private readonly ImageView? indicatorView;

            public ChapterViewHolder(View? itemView)
                : base(itemView)
            {
                textView = itemView?.FindViewById<TextView>(Resource.Id.chapter_item_text);
                indicatorView = itemView?.FindViewById<ImageView>(Resource.Id.chapter_item_active_indicator);
            }

            public void SetTitle(string value)
            {
                if (null != textView)
                {
                    textView.Text = value;
                }
            }

            public void SetActive(bool value)
            {
                if (null != indicatorView)
                {
                    indicatorView.Visibility = value ? ViewStates.Visible : ViewStates.Gone;
                }
            }
        }

        #endregion

        #region FragmentViewHolder

        private sealed class FragmentViewHolder : RecyclerView.ViewHolder
        {
            private readonly TextView? textView;

            public FragmentViewHolder(View? itemView)
                : base(itemView)
            {
                textView = itemView?.FindViewById<TextView>(Resource.Id.chapter_item_text);
            }

            public void SetTitle(string value)
            {
                if (null != textView)
                {
                    textView.Text = value;
                }
            }
        }

        #endregion


        #region ChaptersAdapter
        
        private abstract class ListItem
        {
            public abstract int GetItemType();
        }

        private sealed class ChapterItem : ListItem
        {
            public long QueueId
            {
                get;
            }

            public string Title
            {
                get;
            }

            public ChapterItem(long id, string title)
            {
                QueueId = id;
                Title = title;
            }

            public override int GetItemType() => 0;
        }

        private sealed class FragmentItem : ListItem
        {
            public string Title
            {
                get;
            }

            public FragmentItem(string title)
            {
                Title = title;
            }

            public override int GetItemType() => 1;
        }

        private class GroupedAdapter : RecyclerView.Adapter
        {
            private List<ListItem>? items;
            private long activeQueueItemId;

            public override int ItemCount => items?.Count ?? 0;

            public GroupedAdapter()
            {
                items = null;
                activeQueueItemId = -1;
            }

            public void SetQueue(IList<MediaSessionCompat.QueueItem> value)
            {
                var list = new List<ListItem>();
                var fragments = new List<KeyValuePair<FragmentItem, List<ChapterItem>>>();

                for (var index = 0; index < value.Count; index++)
                {
                    var queueItem = value[index];
                    var section = queueItem.Description.Extras.GetString("Chapter.Section", null);

                    if (null == section)
                    {
                        continue;
                    }

                    if (false == TryGetValue(fragments, section, out var chapters))
                    {
                        chapters = new List<ChapterItem>();
                        fragments.Add(new KeyValuePair<FragmentItem, List<ChapterItem>>(
                            new FragmentItem(section),
                            chapters
                        ));
                    }

                    var chapter = new ChapterItem(queueItem.QueueId, queueItem.Description.Title);

                    chapters.Add(chapter);
                }

                for (var fragmentIndex = 0; fragmentIndex < fragments.Count; fragmentIndex++)
                {
                    var (fragmentItem, chapterItems) = fragments[fragmentIndex];

                    list.Add(fragmentItem);
                    list.AddRange(chapterItems);
                }

                items = list;

                NotifyDataSetChanged();
            }

            public void SetActiveQueueItem(long value)
            {
                var oldQueueItemId = activeQueueItemId;

                activeQueueItemId = value;

                for (var index = 0; null != items && index < items.Count; index++)
                {
                    if (items[index] is ChapterItem chapterItem)
                    {
                        if (chapterItem.QueueId == activeQueueItemId || chapterItem.QueueId == oldQueueItemId)
                        {
                            NotifyItemChanged(index);
                        }
                    }
                }
            }

            public ListItem GetItem(int position)
            {
                if (null == items)
                {
                    throw new InvalidOperationException();
                }

                return items[position];
            }


            public override int GetItemViewType(int position)
            {
                return null != items ? items[position].GetItemType() : base.GetItemViewType(position);
            }

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                switch (holder.ItemViewType)
                {
                    case 0:
                    {
                        if (items?[position] is ChapterItem chapterItem)
                        {
                            var view = (ChapterViewHolder)holder;

                            view.SetTitle(chapterItem.Title);
                            view.SetActive(chapterItem.QueueId == activeQueueItemId);
                        }

                        break;
                    }

                    case 1:
                    {
                        if (items?[position] is FragmentItem fragmentItem)
                        {
                            var view = (FragmentViewHolder)holder;
                            view.SetTitle(fragmentItem.Title);
                        }

                        break;
                    }
                }
            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                var layoutInflater = LayoutInflater.From(parent.Context);

                switch (viewType)
                {
                    case 0:
                    {
                        var itemView = layoutInflater?.Inflate(Resource.Layout.content_chapter_selection_item, parent, false);
                        return new ChapterViewHolder(itemView);
                    }

                    case 1:
                    {
                        var itemView = layoutInflater?.Inflate(Resource.Layout.content_chapter_selection_header, parent, false);
                        return new FragmentViewHolder(itemView);
                    }

                    default:
                    {
                        throw new NotSupportedException();
                    }
                }
            }

            private static bool TryGetValue(IReadOnlyList<KeyValuePair<FragmentItem, List<ChapterItem>>> list, string key, out List<ChapterItem> collection)
            {
                for (var index = 0; index < list.Count; index++)
                {
                    if (false == String.Equals(list[index].Key.Title, key))
                    {
                        continue;
                    }

                    collection = list[index].Value;

                    return true;
                }

                collection = null!;

                return false;
            }
        }

        #endregion

        #region OnItemTouchListener

        private sealed class ItemTouchListener : Object, RecyclerView.IOnItemTouchListener
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

        private sealed class ItemClickListener : ItemTouchListener.IOnItemClickListener
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

        private sealed class StartOffsetItemDecoration : RecyclerView.ItemDecoration
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

        private sealed class EndOffsetItemDecoration : RecyclerView.ItemDecoration
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