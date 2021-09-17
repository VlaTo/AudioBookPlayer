using Android.Support.V4.Media;
using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Persistence.LiteDb.Models;
using System.Diagnostics.CodeAnalysis;

namespace AudioBookPlayer.App.Android.Services.Builders
{
    internal abstract class MediaItemBuilder
    {
        [return: NotNull]
        public abstract MediaBrowserCompat.MediaItem BuildBookPreviewMediaItem([NotNull] Book source, int flags);

        [return: NotNull]
        public abstract MediaBrowserCompat.MediaItem BuildMediaItemFrom([NotNull] AudioBookChapter source, int flags);

        [return: MaybeNull]
        public static global::Android.Net.Uri GetBookImageUri([NotNull] Book book, int imageIndex)
        {
            if (book.Images.Length > imageIndex)
            {
                return global::Android.Net.Uri.Parse(book.Images[imageIndex]);
            }

            return null;
        }
    }
}