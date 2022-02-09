using Android.Graphics;
using Android.Net;

namespace AudioBookPlayer.Core
{
    public sealed class ImageCache : LruCache<Uri, ImageCache.ImageEntry>
    {
        private static ImageCache instance;

        public static ImageCache GetInstance()
        {
            if (null == instance)
            {
                instance = new ImageCache(8);
            }

            return instance;
        }

        private ImageCache(int maxSize)
            : base(maxSize)
        {
        }

        public bool Contains(Uri contentUri)
        {
            return null != Get(contentUri);
        }

        public bool TryGetImages(Uri contentUri, out ImageEntry entry)
        {
            entry = Get(contentUri);
            return null != entry;
        }

        public void PutImages(Uri contentUri, ImageEntry entry)
        {
            Put(contentUri, entry);
        }

        protected override void Release(ImageEntry entry)
        {
            entry.Thumbnail.Dispose();
            entry.Cover.Dispose();
        }

        public sealed class ImageEntry
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