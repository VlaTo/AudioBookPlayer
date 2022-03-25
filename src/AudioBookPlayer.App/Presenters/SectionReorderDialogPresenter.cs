using System;
using System.Collections.Generic;
using System.Diagnostics;
using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using AudioBookPlayer.App.Core.Extensions;
using AudioBookPlayer.App.Core.Internal;
using AudioBookPlayer.MediaBrowserConnector;

#nullable enable

namespace AudioBookPlayer.App.Presenters
{
    internal sealed class SectionReorderDialogPresenter : DialogPresenter, MediaBrowserServiceConnector.IConnectCallback, MediaService.IMediaServiceListener, SectionReorderDialogPresenter.IOnStartDragListener
    {
        private MediaService? mediaService;
        private RecyclerView? recyclerView;
        private ItemTouchHelper? touchHelper;
        private RecyclerViewAdapter? adapter;

        #region IOnStartDragListener

        internal interface IOnStartDragListener
        {
            void OnStartDrag(RecyclerView.ViewHolder viewHolder);
        }

        #endregion

        public SectionReorderDialogPresenter(DialogAccessor dialogAccessor)
            : base(dialogAccessor)
        {
        }

        public override void AttachView(View? view)
        {
            if (null == view)
            {
                return;
            }

            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.sections_view_recycler);

            if (null != recyclerView)
            {
                adapter = new RecyclerViewAdapter(this);

                //var context = Application.Context;
                var callback = new ReorderItemTouchHelperCallback(adapter);
                var offset = TypedValue.ApplyDimension(ComplexUnitType.Dip, 16.0f, view.Resources?.DisplayMetrics);

                touchHelper = new ItemTouchHelper(callback);
                /*var clickListener = new ItemClickListener
                {
                    OnItemClickImpl = OnItemClick,
                    OnLongItemClickImpl = OnLongItemClick
                };*/

                //recyclerView.SetLayoutManager(new LinearLayoutManager(context));
                recyclerView.SetAdapter(adapter);
                touchHelper.AttachToRecyclerView(recyclerView);
                //recyclerView.AddOnItemTouchListener(new ItemTouchListener(context, recyclerView, clickListener));
                recyclerView.AddItemDecoration(new StartOffsetItemDecoration((int)offset));
                recyclerView.AddItemDecoration(new EndOffsetItemDecoration((int)offset));
            }

            Dialog.SetCanceledOnTouchOutside(true);

            var window = Dialog.Window;

            if (null != window)
            {
                window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
                window.RequestFeature(WindowFeatures.NoTitle);
                window.SetGravity(GravityFlags.CenterHorizontal | GravityFlags.CenterVertical);
                window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            }

            Connector.Connect(this);
        }

        public override void DetachView()
        {
            if (null != mediaService)
            {
                mediaService.RemoveListener(this);
            }

            Connector.Disconnect(this);
        }

        private void SetMediaQueue()
        {
            if (null != adapter && null != mediaService)
            {
                adapter.SetQueue(mediaService.MediaQueue);
            }
        }

        #region MediaBrowserServiceConnector.IConnectCallback

        void MediaBrowserServiceConnector.IConnectCallback.OnConnected(MediaService service)
        {
            mediaService = service;

            if (null != mediaService)
            {
                SetMediaQueue();
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
            ;
        }

        #endregion

        #region IOnStartDragListener implementation

        void IOnStartDragListener.OnStartDrag(RecyclerView.ViewHolder viewHolder)
        {
            touchHelper?.StartDrag(viewHolder);
        }

        #endregion

        #region RecyclerViewAdapter

        private sealed class SectionItem
        {
            public string Title
            {
                get;
            }

            public SectionItem(string title)
            {
                Title = title;
            }
        }

        private sealed class SectionItemViewHolder : RecyclerView.ViewHolder
        {
            private readonly IOnStartDragListener? listener;
            private readonly TextView? textView;
            private readonly ImageView? actionView;

            public SectionItemViewHolder(View? itemView, IOnStartDragListener? listener)
                : base(itemView)
            {
                this.listener = listener;

                textView = itemView?.FindViewById<TextView>(Resource.Id.section_item_text);
                actionView = itemView?.FindViewById<ImageView>(Resource.Id.section_reorder_view);

                if (null != actionView)
                {
                    actionView.SetOnClickListener(ClickListener.Create(OnClick));
                }
            }

            public void SetTitle(string value)
            {
                if (null != textView)
                {
                    textView.Text = value;
                }
            }

            private void OnClick(View? view)
            {
                listener?.OnStartDrag(this);
            }
        }

        private sealed class RecyclerViewAdapter : RecyclerView.Adapter
        {
            private readonly IOnStartDragListener listener;
            private List<KeyValuePair<SectionItem, int>>? items;

            public override int ItemCount => items?.Count ?? 0;

            public RecyclerViewAdapter(IOnStartDragListener listener)
            {
                this.listener = listener;
            }

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                var view = (SectionItemViewHolder)holder;
                var item = GetItem(position);
                view.SetTitle(item.Title);
            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                var layoutInflater = LayoutInflater.From(parent.Context);
                var view = layoutInflater?.Inflate(Resource.Layout.content_section_reorder_item, parent, false);
                return new SectionItemViewHolder(view, listener);
            }

            public void SetQueue(IList<MediaSessionCompat.QueueItem> value)
            {
                /*var sections = new List<KeyValuePair<SectionItem, int>>();

                bool Exists(string title)
                {
                    return sections.Exists(item => String.Equals(item.Key.Title, title));
                }

                for (var index = 0; index < value.Count; index++)
                {
                    var queueItem = value[index];
                    var section = queueItem.Description.Extras.GetString("Chapter.Section", null);

                    if (null == section || Exists(section))
                    {
                        continue;
                    }
                    
                    var item = new SectionItem(section);

                    sections.Add(new KeyValuePair<SectionItem, int>(item, sections.Count));
                }

                items = sections;*/

                items = new List<KeyValuePair<SectionItem, int>>
                {
                    new KeyValuePair<SectionItem, int>(new SectionItem("Sample book section 1"), 0),
                    new KeyValuePair<SectionItem, int>(new SectionItem("Sample book section 2"), 1),
                    new KeyValuePair<SectionItem, int>(new SectionItem("Sample book section 3"), 2),
                    new KeyValuePair<SectionItem, int>(new SectionItem("Sample book section 4"), 3),
                    new KeyValuePair<SectionItem, int>(new SectionItem("Sample book section 5"), 4)
                };

                NotifyDataSetChanged();
            }

            public SectionItem GetItem(int position)
            {
                if (null == items)
                {
                    throw new InvalidOperationException();
                }

                return items[position].Key;
            }

            public void MoveItem(int sourceIndex, int targetIndex)
            {
                items.Swap(sourceIndex, targetIndex);
                NotifyItemMoved(sourceIndex, targetIndex);
            }

            public void DismissItem(int position)
            {
                ;
            }
        }

        #endregion

        #region ReorderItemTouchHelperCallback

        private class ReorderItemTouchHelperCallback : ItemTouchHelper.Callback
        {
            private readonly RecyclerViewAdapter adapter;

            public ReorderItemTouchHelperCallback(RecyclerViewAdapter adapter)
            {
                this.adapter = adapter;
            }

            public override int GetMovementFlags(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder)
            {
                const int dragFlags = ItemTouchHelper.Up | ItemTouchHelper.Down;
                const int swipeFlags = ItemTouchHelper.ActionStateIdle;
                //const int swipeFlags = ItemTouchHelper.Start | ItemTouchHelper.End;
                return MakeMovementFlags(dragFlags, swipeFlags);
            }

            public override bool OnMove(RecyclerView recyclerView, RecyclerView.ViewHolder source, RecyclerView.ViewHolder target)
            {
                
                adapter.MoveItem(source.AbsoluteAdapterPosition, target.AbsoluteAdapterPosition);

                return true;
            }

            public override void OnSwiped(RecyclerView.ViewHolder viewHolder, int direction)
            {
                adapter.DismissItem(viewHolder.AbsoluteAdapterPosition);
            }

            public override void OnSelectedChanged(RecyclerView.ViewHolder viewHolder, int actionState)
            {
                if (ItemTouchHelper.ActionStateDrag == actionState)
                {
                    viewHolder.ItemView.Alpha = 0.5f;
                }
            }

            public override void ClearView(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder)
            {
                viewHolder.ItemView.Alpha = 1.0f;
            }
        }

        #endregion
    }
}

#nullable restore