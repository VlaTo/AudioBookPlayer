#nullable enable

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Views.Activities;
using AudioBookPlayer.Domain;
using System;
using System.Collections.Generic;
using System.Globalization;
using AudioBookPlayer.App.ViewModels;
using Fragment = AndroidX.Fragment.App.Fragment;
using FragmentTransaction = AndroidX.Fragment.App.FragmentTransaction;

namespace AudioBookPlayer.App.Views.Fragments
{
    public class AllBooksFragment : Fragment, MediaBrowserServiceConnector.IAudioBooksResultCallback
    {
        private ListView? listView;
        
        public MainActivity MainActivity => (MainActivity)Activity;
        
        private AudioBookDescriptionAdapter? BooksAdapter => (AudioBookDescriptionAdapter?)listView?.Adapter;

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

            if (ViewModel is { HasBookItems: false})
            {
                MainActivity.ServiceConnector?.GetAudioBooks(this);
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
                    var bookItems = ViewModel.BookItems ?? Array.Empty<AudioBookDescription>();

                    listView.OnItemClickListener = new ItemClickListener(Activity.SupportFragmentManager);
                    listView.Adapter = new AudioBookDescriptionAdapter(Application.Context, bookItems);
                }
            }

            return view;
        }

        /*public override void OnStart()
        {
            base.OnStart();

            MainActivity.ServiceConnector?.Connect(this);
        }*/

        /*#region MediaBrowserServiceConnector.IConnectCallback

        void MediaBrowserServiceConnector.IConnectCallback.OnConnected()
        {
            MainActivity.ServiceConnector?.GetAudioBooks(this);
        }

        #endregion*/

        #region MediaBrowserServiceConnector.IAudioBooksResultCallback

        void MediaBrowserServiceConnector.IAudioBooksResultCallback.OnAudioBooksResult(IReadOnlyList<AudioBookDescription> list)
        {
            if (null != BooksAdapter)
            {
                ViewModel.SetBookItems(list);
                BooksAdapter.SetItems(list);
            }
        }

        void MediaBrowserServiceConnector.IAudioBooksResultCallback.OnAudioBooksError()
        {
            ;
        }

        #endregion

        //
        private sealed class AudioBookDescriptionAdapter : BaseAdapter<AudioBookDescription>
        {
            private readonly Context context;
            private readonly LayoutInflater? inflater;
            private IList<AudioBookDescription> list;

            public override int Count => list.Count;

            public override AudioBookDescription this[int position] => list[position];

            public AudioBookDescriptionAdapter(Context context, IReadOnlyList<AudioBookDescription> list)
            {
                this.context = context;
                this.list = new List<AudioBookDescription>(list);
                inflater = LayoutInflater.From(Application.Context);
            }

            public override long GetItemId(int position) => position;

            public override View? GetView(int position, View? convertView, ViewGroup? parent)
            {
                // var view = convertView ?? inflater?.Inflate(Android.Resource.Layout.conSimpleListItem1, null);
                // var textView = view?.FindViewById<TextView>(Android.Resource.Id.Text1);
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

            public void SetItems(IReadOnlyList<AudioBookDescription> items)
            {
                if (ReferenceEquals(list, items))
                {
                    return;
                }

                list = new List<AudioBookDescription>(items);

                NotifyDataSetChanged();
            }
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
                var mediaId = "book:0/0/1";
                var fragment = NowPlayingFragment.NewInstance(mediaId);

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