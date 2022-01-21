using System;
using System.Globalization;
using Android.OS;
using Android.Support.V4.Media;
using AudioBookPlayer.App.ViewModels;

namespace AudioBookPlayer.App.Core
{
    internal sealed class AudioBookViewModelBuilder
    {
        public AudioBookViewModel Create(MediaBrowserCompat.MediaItem mediaItem)
        {
            var mediaId = mediaItem.MediaId;
            var title = mediaItem.Description.Title;
            var subtitle = mediaItem.Description.Subtitle;
            var description = mediaItem.Description.Description;
            var authors = GetAuthors(mediaItem.Description.Extras);
            var duration = GetDuration(mediaItem.Description.Extras);
            var created = GetCreated(mediaItem.Description.Extras);

            return new AudioBookViewModel(mediaId, title, subtitle, description, authors, duration, created);
        }

        private static string GetAuthors(Bundle extra)
        {
            if (null == extra)
            {
                return String.Empty;
            }

            var separator = CultureInfo.CurrentUICulture.TextInfo.ListSeparator;
            var authors = extra.GetStringArray("Book.Authors");

            if (null == authors)
            {
                return String.Empty;
            }

            return String.Join(separator, authors);
        }

        private static TimeSpan GetDuration(Bundle extra)
        {
            if (null == extra)
            {
                return TimeSpan.Zero;
            }

            var value = extra.GetDouble("Book.Duration", Double.NaN);

            return Double.IsNaN(value) ? TimeSpan.Zero : TimeSpan.FromMilliseconds(value);
        }

        private static DateTime GetCreated(Bundle extra)
        {
            if (null == extra)
            {
                return DateTime.UtcNow;
            }

            var value = extra.GetLong("Book.Created", 0L);

            return 0L == value ? DateTime.UtcNow : DateTime.FromBinary(value);
        }
    }
}