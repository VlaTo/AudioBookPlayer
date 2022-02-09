#nullable enable

using System;
using System.Globalization;
using Android.OS;
using Android.Support.V4.Media;
using Uri = Android.Net.Uri;

namespace AudioBookPlayer.App.Models
{
    internal sealed class BookItem : BaseItem
    {
        public long Id
        {
            get;
        }

        public string MediaId
        {
            get;
        }

        public TimeSpan Duration
        {
            get;
        }

        public string Authors
        {
            get;
        }

        public Uri? ImageUri
        {
            get;
        }

        public DateTime? RecentActionTime
        {
            get;
        }

        public BookItem(
            long id,
            string mediaId,
            string title,
            string authors,
            TimeSpan duration,
            Uri? imageUri,
            DateTime? recentActionTime)
            : base(title)
        {
            Id = id;
            MediaId = mediaId;
            Duration = duration;
            Authors = authors;
            ImageUri = imageUri;
            RecentActionTime = recentActionTime;
        }

        public static BookItem From(MediaBrowserCompat.MediaItem mediaItem)
        {
            var id = GetId(mediaItem.Description.Extras);
            var mediaId = mediaItem.MediaId;
            var title = mediaItem.Description.Title;
            var authors = GetAuthors(mediaItem.Description.Extras);
            var duration = GetDuration(mediaItem.Description.Extras);
            var recentActionTime = GetRecentActionTime(mediaItem.Description.Extras);

            return new BookItem(id, mediaId, title, authors, duration, mediaItem.Description.IconUri, recentActionTime);
        }

        private static long GetId(Bundle? extra)
        {
            return extra?.GetLong("Book.Id", 0L) ?? 0L;
        }

        private static string GetAuthors(Bundle? extra)
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

        private static TimeSpan GetDuration(Bundle? extra)
        {
            if (null == extra)
            {
                return TimeSpan.Zero;
            }

            var value = extra.GetDouble("Book.Duration", Double.NaN);

            return Double.IsNaN(value) ? TimeSpan.Zero : TimeSpan.FromMilliseconds(value);
        }

        private static DateTime? GetRecentActionTime(Bundle? extra)
        {
            const string key = "Book.RecentActionTime";

            if (null == extra || false == extra.ContainsKey(key))
            {
                return null;
            }

            var value = extra.GetLong(key);

            return DateTime.FromBinary(value);
        }
    }
}