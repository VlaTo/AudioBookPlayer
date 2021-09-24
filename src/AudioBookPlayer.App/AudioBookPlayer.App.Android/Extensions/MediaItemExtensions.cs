using System;
using Android.Support.V4.Media;
using AudioBookPlayer.App.Android.Core;
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
            var id = MediaBookId.TryParse(mediaItem.MediaId, out var mediaId) ? mediaId.EntityId : EntityId.Empty;
            var bookItem = new BookItem.Builder()
                .SetId(id)
                .SetTitle(mediaItem.Description.Title)
                .SetAuthors(authors)
                .SetDuration(duration)
                .SetPosition(position)
                .SetIsCompleted(isCompleted)
                .AddCover(mediaItem.Description.IconUri.ToString())
                .Build();

            return bookItem;
        }

        public static SectionItem ToSectionItem(this MediaBrowserCompat.MediaItem mediaItem)
        {
            string GetString(string key)
            {
                return mediaItem.Description.Extras.GetString(key);
            }

            var id = MediaSectionId.TryParse(mediaItem.MediaId, out var result) ? result : MediaSectionId.Empty;
            var sectionItem = new SectionItem.Builder()
                .SetBookId(id.EntityId)
                .SetIndex(id.Index)
                .SetTitle(mediaItem.Description.Title)
                .SetContentUri(GetString("ContentUri"))
                //.SetDuration(GetDuration())
                //.SetPosition(GetPosition())
                //.SetIsCompleted(GetIsCompleted())
                //.AddCover(mediaItem.Description.IconUri.ToString())
                .Build();

            return sectionItem;
        }
    }
}