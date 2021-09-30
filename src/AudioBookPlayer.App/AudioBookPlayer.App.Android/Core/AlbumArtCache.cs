#nullable enable

using System;
using System.Threading.Tasks;
using Android.Graphics;
using AudioBookPlayer.App.Android.Services.Helpers;
using AudioBookPlayer.App.Core.Extensions;
using AudioBookPlayer.App.Domain.Providers;
using IOException = Java.IO.IOException;

namespace AudioBookPlayer.App.Android.Core
{
    public class FetchListener
    {
        public Action<string, Bitmap, Bitmap> OnFetched
        {
            get;
            set;
        }

        public void OnError(string artUrl, Exception e)
        {
            // LogHelper.Error(Tag, e, "AlbumArtFetchListener: error while downloading " + artUrl);
        }
    }

    internal sealed class AlbumArtCache
    {
        private const int MaxAlbumArtCacheSize = 12 * 1024 * 1024;
        private const int MaxArtWidth = 800;
        private const int MaxArtHeight = 480;

        private const int MaxArtWidthIcon = 128;
        private const int MaxArtHeightIcon = 128;

        private const int BigBitmapIndex = 0;
        private const int IconBitmapIndex = 1;

        private readonly BitmapsCache cache;

        private static AlbumArtCache instance;

        public static AlbumArtCache Instance => instance ??= new AlbumArtCache();

        private AlbumArtCache()
        {
            var maxMemory = Java.Lang.Runtime.GetRuntime()?.MaxMemory();
            var maxSize = Math.Min(
                MaxAlbumArtCacheSize,
                (int)Math.Min(int.MaxValue, maxMemory.GetValueOrDefault() / 4)
            );

            cache = new BitmapsCache(maxSize);
        }
        
        public Bitmap? GetBigImage(string artUrl)
        {
            var result = cache.Get(artUrl);
            return result?[BigBitmapIndex];
        }
        
        public Bitmap? GetIconImage(string artUrl)
        {
            var result = cache.Get(artUrl);
            return result?[IconBitmapIndex];
        }
        
        public void Fetch(ICoverProvider coverProvider, string artUrl, FetchListener listener)
        {
            // WARNING: for the sake of simplicity, simultaneous multi-thread fetch requests
            // are not handled properly: they may cause redundant costly operations, like HTTP
            // requests and bitmap rescales. For production-level apps, we recommend you use
            // a proper image loading library, like Glide.
            var entry = cache.Get(artUrl);

            if (null != entry)
            {
                //LogHelper.Debug(Tag, "getOrFetch: album art is in cache, using it", artUrl);
                listener.OnFetched(artUrl, entry[BigBitmapIndex], entry[IconBitmapIndex]);

                return;
            }

            // LogHelper.Debug(Tag, "getOrFetch: starting asynctask to fetch ", artUrl);

            Task
                .Run(async () =>
                {
                    Bitmap[] bitmaps;
                    try
                    {
                        Bitmap bitmap = await BitmapHelper.FetchAndRescaleBitmap(coverProvider, artUrl, MaxArtWidth, MaxArtHeight);
                        Bitmap icon = BitmapHelper.Scale(bitmap, MaxArtWidthIcon, MaxArtHeightIcon);

                        bitmaps = new[] { bitmap, icon };

                        cache.Put(artUrl, bitmaps);
                    }
                    catch (IOException)
                    {
                        return null;
                    }

                    // LogHelper.Debug(Tag, "doInBackground: putting bitmap in cache. cache size=" + cache.Size());
                    return bitmaps;

                })
                .ContinueWith((antecedent) =>
                    {
                        var bitmaps = antecedent.Result;
                        if (bitmaps == null)
                        {
                            listener.OnError(artUrl, new ArgumentException("got null bitmaps"));
                        }
                        else
                        {
                            listener.OnFetched(artUrl, bitmaps[BigBitmapIndex], bitmaps[IconBitmapIndex]);
                        }
                    },
                    TaskContinuationOptions.OnlyOnRanToCompletion
                )
                .RunAndForget();
        }

        // Bitmaps cache
        private sealed class BitmapsCache : LRUCache<string, Bitmap[]>
        {
            public BitmapsCache(int maxSize)
                : base(maxSize)
            {
            }

            public override int SizeOf(Bitmap[] value) => value[BigBitmapIndex].ByteCount + value[IconBitmapIndex].ByteCount;
        }
    }
}