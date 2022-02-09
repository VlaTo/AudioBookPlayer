using Android.Content;
using Android.Content.Res;
using Android.Views;
using Android.Widget;
using AudioBookPlayer.App.Models;
using System;
using System.Collections.Generic;

#nullable enable

namespace AudioBookPlayer.App.Views
{
    internal sealed class RecentBookListAdapter : BooksListAdapter
    {
        public const int ITEM_TYPE_HEADER = 1;
        public const int ITEM_TYPE_ITEM = 2;

        private const int ViewsCount = 2;
        
        private int todayIndex;
        private int recentIndex;

        public override int Count => InternalList.Count;

        public override int ViewTypeCount { get; } = ViewsCount;

        public override BaseItem this[int position] => InternalList[position];

        public RecentBookListAdapter(Context context, Resources? resources)
            : base(new Sorter(), context, resources)
        {
            todayIndex = -1;
            recentIndex = -1;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View? GetView(int position, View? convertView, ViewGroup? parent)
        {
            if (position == todayIndex || position == recentIndex)
            {
                var view = convertView ?? Inflater?.Inflate(Resource.Layout.content_book_list_header, null);

                if (InternalList[position] is HeaderItem headerItem)
                {
                    return BindSection(view, headerItem);
                }
            }
            else
            {
                var view = convertView ?? Inflater?.Inflate(Resource.Layout.content_book_list_item, null);
                
                if (InternalList[position] is BookItem bookItem)
                {
                    return BindBookItemView(view, bookItem);
                }
            }

            return convertView;
        }

        public override int GetItemViewType(int position)
        {
            return position == todayIndex || position == recentIndex ? ITEM_TYPE_HEADER : ITEM_TYPE_ITEM;
        }

        public override void Clear()
        {
            todayIndex = -1;
            recentIndex = -1;
            
            InternalList.Clear();
            
            NotifyDataSetChanged();
        }

        public override void AddRange(IEnumerable<BaseItem> source)
        {
            var today = DateTime.UtcNow.Date;

            foreach (var item in source)
            {
                var bookItem = (BookItem)item;
                var index = FindInsertIndex(item);

                InternalList.Insert(index, item);

                if (bookItem.RecentActionTime.HasValue)
                {
                    var date = bookItem.RecentActionTime.Value.Date;

                    if (today.Equals(date))
                    {
                        if (0 > todayIndex)
                        {
                            var headerItem = new HeaderItem(Resources.GetString(Resource.String.book_header_today));

                            todayIndex = 0;
                            InternalList.Insert(todayIndex, headerItem);
                        }

                        continue;
                    }

                    if (0 > recentIndex)
                    {
                        var headerItem = new HeaderItem(Resources.GetString(Resource.String.book_header_recent));

                        recentIndex = index;
                        InternalList.Insert(recentIndex, headerItem);
                    }
                }
            }

            NotifyDataSetChanged();
        }

        public override void Detach()
        {
            base.Detach();
        }

        private static View? BindSection(View? view, HeaderItem headerItem)
        {
            var titleView = view?.FindViewById<TextView>(Resource.Id.book_item_header);
            
            if (null != titleView)
            {
                titleView.Text = headerItem.Title;
            }

            return view;
        }

        private sealed class Sorter : IComparer<BaseItem>
        {
            public int Compare(BaseItem x, BaseItem y)
            {
                if (x is BookItem one)
                {
                    if (one.RecentActionTime.HasValue)
                    {
                        if (y is BookItem two)
                        {
                            if (two.RecentActionTime.HasValue)
                            {
                                return DateTime.Compare(one.RecentActionTime.Value, two.RecentActionTime.Value);
                            }
                        }
                    }
                    else
                    {
                        return -1;
                    }
                }

                return 1;
            }
        }

        private sealed class HeaderItem : BaseItem
        {
            public HeaderItem(string title)
                : base(title)
            {
            }
        }
    }
}

#nullable restore