using System;
using Android.OS;
using Android.Support.V4.Media;
using AudioBookPlayer.App.Persistence.LiteDb.Models;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace AudioBookPlayer.App.Android.Services.Builders
{
    internal sealed class MediaBookItemBuilder : MediaItemBuilder
    {
        [return: NotNull]
        public MediaBrowserCompat.MediaItem Build([NotNull] Book source, int flags)
        {
            var mediaItemId = new MediaBookId(source.Id);
            var iconUri = GetBookImageUri(source, 0);
            var duration = source.GetDuration();
            var description = new MediaDescriptionCompat.Builder()
                .SetMediaId(mediaItemId.ToString())
                .SetTitle(source.Title)
                .SetSubtitle(duration.ToString("g", CultureInfo.CurrentUICulture))
                .SetDescription(GetBookAuthors(source))
                .SetExtras(BuildExtra(source));

            if (null != iconUri)
            {
                description.SetIconUri(iconUri);
            }

            return new MediaBrowserCompat.MediaItem(description.Build(), flags);
        }

        [return: NotNull]
        public MediaBrowserCompat.MediaItem Build([NotNull] Section source, int flags)
        {
            var mediaItemId = new MediaBookId(source.Id);
            var iconUri = GetBookImageUri(source, 0);
            var description = new MediaDescriptionCompat.Builder()
                .SetMediaId(mediaItemId.ToString())
                .SetTitle(source.Title)
                .SetSubtitle(GetBookAuthors(source))
                .SetDescription(source.Synopsis)
                .SetExtras(BuildExtra(source));

            if (null != iconUri)
            {
                description.SetIconUri(iconUri);
            }

            return new MediaBrowserCompat.MediaItem(description.Build(), flags);
        }

        private static Bundle BuildExtra([NotNull] Book source)
        {
            var extra = new Bundle();
            var duration = source.GetDuration();

            extra.PutDouble("Duration", duration.TotalMilliseconds);
            extra.PutDouble("Position", TimeSpan.Zero.TotalMilliseconds);
            extra.PutBoolean("IsCompleted", false);
            extra.PutStringArray("Authors", source.Authors);
            
            return extra;
        }
    }
}