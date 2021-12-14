﻿#nullable enable

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Core.Extensions;
using AudioBookPlayer.App.ViewModels;
using AudioBookPlayer.App.Views.Activities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Fragment = AndroidX.Fragment.App.Fragment;
using FragmentTransaction = AndroidX.Fragment.App.FragmentTransaction;

namespace AudioBookPlayer.App.Views.Fragments
{
    public class AllBooksFragment : Fragment
    {
        private ListView? listView;
        private IDisposable? subscription;

        private MainActivity MainActivity => (MainActivity)Activity;

        private BooksListViewAdapter? BooksAdapter => (BooksListViewAdapter?)listView?.Adapter;

        private AllBooksViewModel ViewModel => AllBooksViewModel.Instance();

        public static AllBooksFragment NewInstance()
        {
            var bundle = new Bundle();

            return new AllBooksFragment
            {
                Arguments = bundle
            };
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if (ViewModel is { HasBookItems: false } && null != MainActivity.ServiceConnector)
            {
                MainActivity.ServiceConnector.Connect(ViewModel);
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var _ = base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.fragment_books_list, container, false);

            if (null != view)
            {
                listView = view.FindViewById<ListView>(Resource.Id.books_list);

                if (null != listView)
                {
                    var adapter = new BooksListViewAdapter(Application.Context, ViewModel.BookItems);
                    listView.OnItemClickListener = new ItemClickListener(Activity.SupportFragmentManager);
                    listView.Adapter = adapter;
                    subscription = ViewModel.BookItems.Subscribe(adapter);
                }
            }

            return view;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            subscription?.Dispose();
        }

        //
        private sealed class BooksListViewAdapter : BaseAdapter<AudioBookViewModel>, IListObserver<AudioBookViewModel>
        {
            private readonly Context context;
            private readonly LayoutInflater? inflater;
            private List<AudioBookViewModel> list;

            public override int Count => list.Count;

            public override AudioBookViewModel this[int position] => list[position];

            public BooksListViewAdapter(Context context, IList<AudioBookViewModel> list)
            {
                this.context = context;
                this.list = new List<AudioBookViewModel>(list);

                inflater = LayoutInflater.From(Application.Context);
            }

            public override long GetItemId(int position) => position;

            public override View? GetView(int position, View? convertView, ViewGroup? parent)
            {
                var view = convertView ?? inflater?.Inflate(Resource.Layout.content_book_list_item, null);
                var coverImage = view?.FindViewById<ImageView>(Resource.Id.book_item_cover);
                var titleView = view?.FindViewById<TextView>(Resource.Id.book_item_title);
                var subtitleView = view?.FindViewById<TextView>(Resource.Id.book_item_subtitle);
                var descriptionView = view?.FindViewById<TextView>(Resource.Id.book_item_description);

                var item = list[position];

                if (null != titleView)
                {
                    titleView.Text = item.Title;
                }

                if (null != subtitleView)
                {
                    subtitleView.Text = "Author Some";
                }

                if (null != descriptionView)
                {
                    descriptionView.Text = item.Duration.ToString("t", CultureInfo.CurrentUICulture);
                }

                return view;
            }

            /*public void SetItems(IReadOnlyList<AudioBookViewModel> items)
            {
                if (ReferenceEquals(list, items))
                {
                    return;
                }

                list = items.ToArray();

                NotifyDataSetChanged();
            }*/

            #region IListObserver<AudioBookViewModel>

            void IListObserver<AudioBookViewModel>.OnAdded(int position, AudioBookViewModel item)
            {
                list.Insert(position, item);
                NotifyDataSetChanged();
            }

            void IListObserver<AudioBookViewModel>.OnReplace(int position, AudioBookViewModel item)
            {
                list[position] = item;
                NotifyDataSetChanged();
            }

            void IListObserver<AudioBookViewModel>.OnRemove(int position)
            {
                list.RemoveAt(position);
                NotifyDataSetChanged();
            }

            void IListObserver<AudioBookViewModel>.OnClear()
            {
                list.Clear();
                NotifyDataSetChanged();
            }

            #endregion
        }

        //
        private sealed class ItemClickListener : Java.Lang.Object, AdapterView.IOnItemClickListener
        {
            private readonly AndroidX.Fragment.App.FragmentManager fragmentManager;

            public ItemClickListener(AndroidX.Fragment.App.FragmentManager fragmentManager)
            {
                this.fragmentManager = fragmentManager;
            }

            public void OnItemClick(AdapterView? parent, View? view, int position, long id)
            {
                var item = (AudioBookViewModel?)parent?.GetItemAtPosition(position);

                if (null != item)
                {
                    var fragment = NowPlayingFragment.NewInstance(item.MediaId);

                    fragmentManager
                        .BeginTransaction()
                        .Replace(Resource.Id.nav_host_frame, fragment)
                        .AddToBackStack(null)
                        .SetTransition(FragmentTransaction.TransitFragmentFade)
                        .Commit();
                }
            }
        }
    }
}