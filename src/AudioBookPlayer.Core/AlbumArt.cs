using System;
using Android.Graphics;
using Android.Widget;
using Uri = Android.Net.Uri;

#nullable enable

namespace AudioBookPlayer.Core
{
    public sealed class AlbumArt
    {
        private static AlbumArt? instance;
        private readonly LoadImage<ImageView>? imageLoader;

        public static AlbumArt GetInstance()
        {
            if (null == instance)
            {
                var task = LoadImage.For<ImageView>();

                instance = new AlbumArt(task);
            }

            return instance;
        }

        private AlbumArt(LoadImage<ImageView> imageLoader)
        {
            this.imageLoader = imageLoader;
        }

        public void Initialize()
        {
            if (null == imageLoader)
            {
                return;
            }

            imageLoader.Start();
            
            var _ = imageLoader.Looper;
        }

        public void Fetch(Uri imageUri, ImageView view, Action<ImageView, Android.Net.Uri, Bitmap?, Bitmap?> callback)
        {
            imageLoader?.QueueImage(view, imageUri, callback);
        }
    }
}

#nullable restore