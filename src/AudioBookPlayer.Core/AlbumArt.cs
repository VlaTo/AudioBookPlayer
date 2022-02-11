using System;
using Android.Graphics;
using Android.Widget;
using Java.Lang;
using Uri = Android.Net.Uri;

#nullable enable

namespace AudioBookPlayer.Core
{
    public sealed class AlbumArt
    {
        private static AlbumArt? instance;
        private LoadImage<ImageView>? imageLoader;

        public static AlbumArt GetInstance()
        {
            if (null == instance)
            {
                instance = new AlbumArt();
            }

            return instance;
        }

        private AlbumArt()
        {
        }

        public void Initialize()
        {
            if (null == imageLoader)
            {
                imageLoader = LoadImage.For<ImageView>();
            }

            var state = imageLoader.GetState();

            if (state == Thread.State.New)
            {
                imageLoader.Start();
                var _ = imageLoader.Looper;
            }
        }

        public void Shutdown()
        {
            if (null == imageLoader)
            {
                return;
            }

            imageLoader.ClearQueue();
            imageLoader.Quit();
            imageLoader = null;
        }

        public void Fetch(Uri imageUri, ImageView view, Action<ImageView, Uri, Bitmap?, Bitmap?> callback)
        {
            imageLoader?.QueueImage(view, imageUri, callback);
        }
    }
}

#nullable restore