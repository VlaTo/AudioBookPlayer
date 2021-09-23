using Android.Support.V4.Media;
using AudioBookPlayer.App.Domain.Models;
using System;
using System.Diagnostics.CodeAnalysis;

namespace AudioBookPlayer.App.Android.Services.Builders
{
    internal sealed class PreviewBookBuilder : BookBuilder<BookItem>
    {
        [return: NotNull]
        public override BookItem BuildBookFrom([NotNull] MediaBrowserCompat.MediaItem mediaItem)
        {
            TimeSpan GetDuration()
            {
                var value = mediaItem.Description.Extras.GetDouble("Duration");
                return TimeSpan.FromMilliseconds(value);
            }

            TimeSpan GetPosition()
            {
                var value = mediaItem.Description.Extras.GetDouble("Position");
                return TimeSpan.FromMilliseconds(value);
            }

            bool GetIsCompleted() => mediaItem.Description.Extras.GetBoolean("IsCompleted");

            string[] GetAuthors() => mediaItem.Description.Extras.GetStringArray("Authors");

            var id = MediaBookId.TryParse(mediaItem.MediaId, out var mediaId) ? mediaId.EntityId : EntityId.Empty;
            var bookItem = new BookItem.Builder()
                .SetId(id)
                .SetTitle(mediaItem.Description.Title)
                .SetAuthors(GetAuthors())
                .SetDuration(GetDuration())
                .SetPosition(GetPosition())
                .SetIsCompleted(GetIsCompleted())
                .AddCover(mediaItem.Description.IconUri.ToString())
                .Build();

            return bookItem;
        }
    }
}