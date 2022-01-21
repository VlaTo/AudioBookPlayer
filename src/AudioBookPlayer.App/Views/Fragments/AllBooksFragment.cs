#nullable enable

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

            // Plural test
            for (var count = 0; count < 22; count++)
            {
                var text = Resources.GetQuantityString(Resource.Plurals.plural_test, count, count);
                System.Diagnostics.Debug.WriteLine($"[PluralTest] {text}");
            }

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
                    var adapter = new BooksListViewAdapter(Application.Context)
                    {
                        SortOrderFunc = (x, y) =>
                            String.Compare(x.Title, y.Title, CultureInfo.CurrentUICulture, CompareOptions.StringSort)
                    };

                    listView.EmptyView = view.FindViewById<ViewStub>(Resource.Id.empty_books_list);
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
            private readonly List<AudioBookViewModel> list;
            private Func<AudioBookViewModel, AudioBookViewModel, int>? sortOrderFunc;

            public override int Count => list.Count;

            public Func<AudioBookViewModel, AudioBookViewModel, int>? SortOrderFunc
            {
                get => sortOrderFunc;
                set
                {
                    if (ReferenceEquals(sortOrderFunc, value))
                    {
                        return;
                    }

                    sortOrderFunc = value;

                    if (null != sortOrderFunc)
                    {
                        list.Sort(new Comparison<AudioBookViewModel>(sortOrderFunc));
                    }

                    NotifyDataSetChanged();
                }
            }

            public override AudioBookViewModel this[int position] => list[position];

            public BooksListViewAdapter(Context context)
            {
                this.context = context;

                list = new List<AudioBookViewModel>();
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

                var model = list[position];

                if (null != titleView)
                {
                    titleView.Text = model.Title;
                }

                if (null != subtitleView)
                {
                    subtitleView.Text = model.Authors;
                }

                if (null != descriptionView)
                {
                    descriptionView.Text = model.Duration.ToString("g", CultureInfo.CurrentUICulture);
                }

                return view;
            }

            #region IListObserver<AudioBookViewModel>

            void IListObserver<AudioBookViewModel>.OnAdded(int position, AudioBookViewModel value)
            {
                if (null != sortOrderFunc)
                {
                    position = list.FindIndex(value, sortOrderFunc);
                }

                list.Insert(position, value);

                NotifyDataSetChanged();
            }

            void IListObserver<AudioBookViewModel>.OnRangeAdded(int position, IEnumerable<AudioBookViewModel> values)
            {
                if (null != sortOrderFunc)
                {
                    foreach (var item in values)
                    {
                        var index = list.FindIndex(item, sortOrderFunc);
                        list.Insert(index, item);
                    }
                }
                else
                {
                    list.AddRange(values);
                }

                NotifyDataSetChanged();
            }

            void IListObserver<AudioBookViewModel>.OnReplace(int position, AudioBookViewModel oldValue, AudioBookViewModel newValue)
            {
                position = list.IndexOf(oldValue);

                if (0 > position)
                {
                    return;
                }

                list[position] = newValue;

                NotifyDataSetChanged();
            }

            void IListObserver<AudioBookViewModel>.OnRemove(int position, AudioBookViewModel value)
            {
                position = list.IndexOf(value);

                if (0 > position)
                {
                    return;
                }

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