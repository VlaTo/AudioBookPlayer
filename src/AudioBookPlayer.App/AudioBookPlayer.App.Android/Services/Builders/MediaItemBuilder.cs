using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Android.Support.V4.Media;
using AudioBookPlayer.App.Domain.Data;
using AudioBookPlayer.App.Domain.Extensions;
using AudioBookPlayer.App.Domain.Models;

namespace AudioBookPlayer.App.Android.Services.Builders
{
    internal static class MediaItemBuilder
    {
        [return: NotNull]
        public static MediaBrowserCompat.MediaItem From([NotNull] AudioBook source, int flags)
        {
            var iconUri = GetBookImageUri(source, 0);
            var duration = source.Duration.ToString("g", CultureInfo.CurrentUICulture);
            var description = new MediaDescriptionCompat.Builder()
                .SetMediaId(GetBookMediaId(source))
                .SetTitle(source.Title)
                .SetSubtitle(duration)
                .SetDescription(source.GetAuthors());

            if (null != iconUri)
            {
                description.SetIconUri(iconUri);
            }

            return new MediaBrowserCompat.MediaItem(description.Build(), flags);
        }

        [return: MaybeNull]
        public static string GetBookMediaId([NotNull] AudioBook audioBook)
        {
            if (false == audioBook.Id.HasValue)
            {
                return null;
            }

            return $"audiobook:{audioBook.Id.Value:D}";
        }

        [return: MaybeNull]
        public static global::Android.Net.Uri GetBookImageUri([NotNull] AudioBook audioBook, int imageIndex)
        {
            if (audioBook.Images.Count > imageIndex)
            {
                if (audioBook.Images[imageIndex] is IHasContentUri has)
                {
                    return global::Android.Net.Uri.Parse(has.ContentUri);
                }
            }

            return null;
        }
    }
}