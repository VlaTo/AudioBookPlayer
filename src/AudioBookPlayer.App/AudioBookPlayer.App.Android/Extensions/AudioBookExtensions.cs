using System;
using System.Globalization;
using System.Linq;
using Android.OS;
using Android.Support.V4.Media;
using AudioBookPlayer.App.Android.Core;
using AudioBookPlayer.App.Android.Services.Builders;
using AudioBookPlayer.App.Domain.Core;
using AudioBookPlayer.App.Domain.Models;

namespace AudioBookPlayer.App.Android.Extensions
{
    internal static class AudioBookExtensions
    {
        public static MediaBrowserCompat.MediaItem ToMediaItem(this AudioBook audioBook)
        {
            const int flags = MediaBrowserCompat.MediaItem.FlagBrowsable | MediaBrowserCompat.MediaItem.FlagPlayable;

            string[] GetAuthors()
            {
                return audioBook.Authors
                    .Select(author => author.Name)
                    .ToArray();
            }

            Bundle BuildExtendedInfo()
            {
                var extra = new Bundle();

                extra.PutDouble("Duration", audioBook.Duration.TotalMilliseconds);
                extra.PutDouble("Position", TimeSpan.Zero.TotalMilliseconds);
                extra.PutBoolean("IsCompleted", false);
                extra.PutStringArray("Authors", GetAuthors());

                return extra;
            }

            global::Android.Net.Uri GetBookImageUri(int imageIndex)
            {
                if (audioBook.Images.Count > imageIndex && audioBook.Images[imageIndex] is IHasContentUri hcu)
                {
                    return global::Android.Net.Uri.Parse(hcu.ContentUri);
                }

                return null;
            }

            var mediaItemId = new MediaBookId(audioBook.Id);
            var iconUri = GetBookImageUri(0);
            var description = new MediaDescriptionCompat.Builder()
                .SetMediaId(mediaItemId.ToString())
                .SetTitle(audioBook.Title)
                .SetSubtitle(audioBook.Duration.ToString("g", CultureInfo.CurrentUICulture))
                .SetDescription(audioBook.Synopsis)
                .SetExtras(BuildExtendedInfo());

            if (null != iconUri)
            {
                description.SetIconUri(iconUri);
            }

            return new MediaBrowserCompat.MediaItem(description.Build(), flags);
        }
    }
}