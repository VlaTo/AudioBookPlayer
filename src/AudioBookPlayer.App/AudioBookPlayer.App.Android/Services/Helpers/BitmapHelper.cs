using System;
using System.IO;
using System.Threading.Tasks;
using Android.Graphics;
using AudioBookPlayer.App.Domain.Providers;

namespace AudioBookPlayer.App.Android.Services.Helpers
{
    internal static class BitmapHelper
    {
        public static async Task<Bitmap> FetchAndRescaleBitmap(ICoverProvider coverProvider, string artUri, int width, int height)
        {
            var stream = await coverProvider.GetImageAsync(artUri);
            var scaleFactor = FindScaleFactor(stream, width, height);

            return Scale(stream, scaleFactor);
        }

        public static int FindScaleFactor(Stream stream, int targetWidth, int targetHeight)
        {
            var bmOptions = new BitmapFactory.Options();
            
            bmOptions.InJustDecodeBounds = true;

            BitmapFactory.DecodeStream(stream, null, bmOptions);
            
            var actualWidth = bmOptions.OutWidth;
            var actualHeight = bmOptions.OutHeight;
            var scaleFactor = Math.Min(actualWidth / targetWidth, actualHeight / targetHeight);

            // return Math.Max(1, scaleFactor);
            var position = stream.Seek(0L, SeekOrigin.Begin);

            return scaleFactor;
        }
        
        public static Bitmap Scale(Stream stream, int scaleFactor)
        {
            var bmOptions = new BitmapFactory.Options();

            bmOptions.InJustDecodeBounds = false;
            bmOptions.InSampleSize = scaleFactor;

            var bitmap = BitmapFactory.DecodeStream(stream, null, bmOptions);
            var position = stream.Seek(0L, SeekOrigin.Begin);

            return bitmap;
        }

        public static Bitmap Scale(Bitmap src, int maxWidth, int maxHeight)
        {
            var scaleFactor = Math.Min(((double)maxWidth) / src.Width, ((double)maxHeight) / src.Height);
            return Bitmap.CreateScaledBitmap(src, (int)(src.Width * scaleFactor), (int)(src.Height * scaleFactor), false);
        }
    }
}