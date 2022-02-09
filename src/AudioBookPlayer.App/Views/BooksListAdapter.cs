#nullable enable

using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using AudioBookPlayer.App.Models;
using AudioBookPlayer.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using BookItem = AudioBookPlayer.App.Models.BookItem;
using Uri = Android.Net.Uri;

namespace AudioBookPlayer.App.Views
{
    internal class BooksListAdapter : BaseAdapter<BaseItem>
    {
        private readonly IComparer<BaseItem> comparer;

        public override int Count => InternalList.Count;

        public override BaseItem this[int position] => InternalList[position];

        protected LayoutInflater? Inflater
        {
            get;
        }

        protected List<BaseItem> InternalList
        {
            get;
        }

        protected Resources? Resources
        {
            get;
        }

        protected BooksListAdapter(IComparer<BaseItem> comparer, Context context, Resources? resources)
        {
            this.comparer = comparer;

            InternalList = new List<BaseItem>();
            Resources = resources;
            Inflater = LayoutInflater.From(context);
        }

        public override long GetItemId(int position)
        {
            //return ((BookItem)InternalList[position]).Id;
            return position;
        }

        public override View? GetView(int position, View? convertView, ViewGroup? parent)
        {
            var view = convertView ?? Inflater?.Inflate(Resource.Layout.content_book_list_item, null);

            if (InternalList[position] is BookItem bookItem)
            {
                return BindBookItemView(view, bookItem);
            }

            return view;
        }

        public virtual void Clear()
        {
            InternalList.Clear();
            NotifyDataSetChanged();
        }

        public virtual void AddRange(IEnumerable<BaseItem> source)
        {
            foreach (var item in source)
            {
                var index = FindInsertIndex(item);
                InternalList.Insert(index, item);
            }

            NotifyDataSetChanged();
        }

        public virtual void Detach()
        {
            ;
        }

        protected View? BindBookItemView(View? view, BookItem bookItem)
        {
            var coverImage = view?.FindViewById<ImageView>(Resource.Id.book_item_cover);
            var titleView = view?.FindViewById<TextView>(Resource.Id.book_item_title);
            var authorView = view?.FindViewById<TextView>(Resource.Id.book_item_author);
            var durationView = view?.FindViewById<TextView>(Resource.Id.book_item_duration_time);
            
            if (null != coverImage && null != bookItem.ImageUri)
            {
                AlbumArt.GetInstance().Fetch(bookItem.ImageUri, coverImage, OnImageLoaded);
            }

            if (null != titleView)
            {
                titleView.Text = bookItem.Title;
            }

            if (null != authorView)
            {
                authorView.Text = bookItem.Authors;
            }

            if (null != durationView)
            {
                var format = Resources?.GetString(Resource.String.book_item_duration_format);
                durationView.Text = String.Format(CultureInfo.CurrentUICulture, format, bookItem.Duration);
            }

            return view;
        }

        protected int FindInsertIndex(BaseItem item)
        {
            for (var index = 0; index < InternalList.Count; index++)
            {
                var actual = InternalList[index];
                var result = comparer.Compare(actual, item);

                if (0 >= result)
                {
                    continue;
                }

                return index;
            }

            return InternalList.Count;
        }

        protected virtual void OnImageLoaded(ImageView imageView, Uri imageUri, Bitmap? thumbnail, Bitmap? cover)
        {
            imageView.SetImageBitmap(thumbnail);
        }
    }
}