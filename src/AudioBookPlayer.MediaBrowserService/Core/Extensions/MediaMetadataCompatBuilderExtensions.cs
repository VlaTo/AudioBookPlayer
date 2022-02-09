using Android.Support.V4.Media;
using AudioBookPlayer.Domain.Models;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AudioBookPlayer.MediaBrowserService.Core.Extensions
{
    internal static class MediaMetadataCompatBuilderExtensions
    {
        public static void PutAuthors(this MediaMetadataCompat.Builder builder, IReadOnlyList<AudioBookAuthor> authors)
        {
            var separator = CultureInfo.CurrentUICulture.TextInfo.ListSeparator;
            var str = new StringBuilder();

            for (var index = 0; index < authors.Count; index++)
            {
                if (0 < index)
                {
                    str.Append(separator);
                }

                str.Append(authors[index].Name);
            }

            builder.PutString(MediaMetadataCompat.MetadataKeyArtist, builder.ToString());
        }
    }
}