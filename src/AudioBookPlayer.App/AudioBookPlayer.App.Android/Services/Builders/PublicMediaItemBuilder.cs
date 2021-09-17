using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Android.Support.V4.Media;
using AudioBookPlayer.App.Domain.Extensions;
using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Persistence.LiteDb.Models;

namespace AudioBookPlayer.App.Android.Services.Builders
{
    internal sealed class PublicMediaItemBuilder : MediaItemBuilder
    {
        [return: NotNull]
        public override MediaBrowserCompat.MediaItem BuildBookPreviewMediaItem([NotNull] Book source, int flags)
        {
            var mediaItemId = new MediaBookId(source.Id);
            var iconUri = GetBookImageUri(source, 0);
            var duration = source.GetDuration();
            var description = new MediaDescriptionCompat.Builder()
                .SetMediaId(mediaItemId.ToString())
                .SetTitle(source.Title)
                .SetSubtitle(duration.ToString("g", CultureInfo.CurrentUICulture))
                .SetDescription(source.GetAuthors());

            if (null != iconUri)
            {
                description.SetIconUri(iconUri);
            }

            return new MediaBrowserCompat.MediaItem(description.Build(), flags);
        }

        public override MediaBrowserCompat.MediaItem BuildMediaItemFrom(AudioBookChapter source, int flags)
        {
            ;
        }
    }
}