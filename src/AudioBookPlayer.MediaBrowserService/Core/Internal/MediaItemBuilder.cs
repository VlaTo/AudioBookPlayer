using Android.Media.Browse;
using Android.Net;
using Android.OS;
using Android.Support.V4.Media;
using AudioBookPlayer.Domain;
using AudioBookPlayer.Domain.Models;
using AudioBookPlayer.MediaBrowserService.Core.Extensions;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace AudioBookPlayer.MediaBrowserService.Core.Internal
{
    internal sealed class MediaItemBuilder
    {
        public MediaBrowserCompat.MediaItem CreateItem(AudioBook audioBook)
        {
            var builder = new MediaDescriptionCompat.Builder();

            var mediaId = new MediaID(audioBook.Id.GetValueOrDefault());
            builder.SetMediaId(mediaId);
            builder.SetMediaUri(mediaId.ToUri());
            builder.SetTitle(audioBook.Title);
            builder.SetSubtitle(audioBook.Duration.ToString("g", CultureInfo.CurrentUICulture));
            builder.SetDescription(audioBook.Description);

            if (TryGetIconUri(audioBook.Images, out var iconUri))
            {
                builder.SetIconUri(iconUri);
            }

            var extra = new Bundle();

            extra.PutStringArray("Book.Authors", GetAuthors(audioBook.Authors));
            extra.PutDouble("Book.Duration", audioBook.Duration.TotalMilliseconds);
            extra.PutLong("Book.Created", audioBook.Created.ToBinary());

            builder.SetExtras(extra);

            return new MediaBrowserCompat.MediaItem(
                builder.Build(),
                (int)MediaItemFlags.Browsable | (int)MediaItemFlags.Playable
            );
        }

        private static string[] GetAuthors(IReadOnlyList<AudioBookAuthor> authors) =>
            authors.Select(x => x.Name).ToArray();

        private static bool TryGetIconUri(IReadOnlyList<IAudioBookImage> images, out Uri iconUri)
        {
            if (0 < images.Count)
            {
                var audioBookImage = images[0];

                if (audioBookImage is IHasContentUri holder)
                {
                    iconUri = Uri.Parse(holder.ContentUri);
                    return true;
                }
            }

            iconUri = null;

            return false;
        }
    }
}