using Android.Graphics;
using Android.Net;

namespace AudioBookPlayer.App.Core
{
    internal sealed class ImageCache : LruCache<Uri, ImageCache.ImageEntry>
    {
        public ImageCache(int maxSize)
            : base(maxSize)
        {
        }

        public bool TryGetImages(Uri contentUri, out Bitmap thumbnail, out Bitmap cover)
        {
            var entry = Get(contentUri);

            if (null != entry)
            {
                thumbnail = entry.Thumbnail;
                cover = entry.Cover;

                return true;
            }

            thumbnail = null;
            cover = null;

            return false;
        }

        public void PutImages(Uri contentUri, Bitmap thumbnail, Bitmap cover)
        {
            Put(contentUri, new ImageEntry(thumbnail, cover));
        }

        protected override void Release(ImageEntry entry)
        {
            entry.Thumbnail.Dispose();
            entry.Cover.Dispose();
        }

        public sealed  class ImageEntry
        {
            public Bitmap Thumbnail
            {
                get;
            }

            public Bitmap Cover
            {
                get;
            }

            public ImageEntry(Bitmap thumbnail, Bitmap cover)
            {
                Thumbnail = thumbnail;
                Cover = cover;
            }
        }
    }
}