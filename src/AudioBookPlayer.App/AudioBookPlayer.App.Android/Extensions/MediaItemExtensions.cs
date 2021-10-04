using System;
using Android.Support.V4.Media;
using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Domain.Models;

namespace AudioBookPlayer.App.Android.Extensions
{
    internal static class MediaItemExtensions
    {
        public static BookItem ToBookItem(this MediaBrowserCompat.MediaItem mediaItem)
        {
            TimeSpan GetTimeSpan(string key)
            {
                var value = mediaItem.Description.Extras.GetDouble(key);
                return TimeSpan.FromMilliseconds(value);
            }

            var duration = GetTimeSpan("Duration");
            var position = GetTimeSpan("Position");
            var isCompleted = mediaItem.Description.Extras.GetBoolean("IsCompleted");
            var authors = mediaItem.Description.Extras.GetStringArray("Authors");
            var id = MediaId.TryParse(mediaItem.MediaId, out var mediaId) ? mediaId.BookId : EntityId.Empty;
            var iconUri = mediaItem.Description.IconUri?.ToString();
            var bookItem = new BookItem.Builder()
                .SetId(id)
                .SetTitle(mediaItem.Description.Title)
                .SetAuthors(authors)
                .SetDuration(duration)
                .SetPosition(position)
                .SetIsCompleted(isCompleted)
                .AddCover(iconUri)
                .Build();

            return bookItem;
        }

        public static SectionItem ToSectionItem(this MediaBrowserCompat.MediaItem mediaItem)
        {
            /*string GetString(string key)
            {
                return mediaItem.Description.Extras.GetString(key);
            }*/

            var id = MediaId.TryParse(mediaItem.MediaId, out var result) ? result : MediaId.Empty;
            var contentUri = mediaItem.Description.MediaUri.ToString();
            var sectionItem = new SectionItem.Builder()
                .SetBookId(id.BookId)
                .SetTitle(mediaItem.Description.Title)
                .SetContentUri(contentUri);

            if (id.SectionIndex.HasValue)
            {
                sectionItem.SetIndex(id.SectionIndex.Value);
            }

            //.SetDuration(GetDuration())
            //.SetPosition(GetPosition())
            //.SetIsCompleted(GetIsCompleted())
            //.AddCover(mediaItem.Description.IconUri.ToString())

            return sectionItem.Build();
        }
    }
}