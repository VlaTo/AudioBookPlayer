using Android.OS;
using Android.Support.V4.Media;
using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Domain.Services;
using AudioBookPlayer.App.ViewModels;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AudioBookPlayer.App.Android.Services.Builders
{
    internal sealed class PreviewBookBuilder : BookBuilder<BookPreviewViewModel>
    {
        private readonly ICoverService coverService;

        public PreviewBookBuilder(ICoverService coverService)
        {
            this.coverService = coverService;
        }

        [return: NotNull]
        public override BookPreviewViewModel BuildBookFrom([NotNull] MediaBrowserCompat.MediaItem mediaItem)
        {
            var id = MediaBookId.TryParse(mediaItem.MediaId, out var mediaId) ? mediaId.Id : BookId.Empty;
            var duration = GetDuration(mediaItem.Description.Extras);
            var book = new BookPreviewViewModel(id)
            {
                Title = mediaItem.Description.Title,
                Authors = mediaItem.Description.Subtitle,
                Duration = duration,
                Position = GetCompletion(mediaItem.Description.Extras, duration),
                Completed = GetIsCompleted(mediaItem.Description.Extras),
                ImageSource = GetThumbnailImage(mediaItem.Description.IconUri)
            };

            return book;
        }

        private Func<CancellationToken, Task<Stream>> GetThumbnailImage(global::Android.Net.Uri iconUri)
        {
            return cancellationToken => coverService.GetImageAsync(iconUri.ToString(), cancellationToken);
        }

        private static TimeSpan GetDuration(Bundle bundle)
        {
            var value = bundle.GetDouble("Duration");
            return TimeSpan.FromMilliseconds(value);
        }

        private static double GetCompletion(Bundle bundle, TimeSpan duration)
        {
            var position = bundle.GetDouble("Position");
            return position / duration.TotalMilliseconds;
        }

        private static bool GetIsCompleted(Bundle bundle)
        {
            return bundle.GetBoolean("Completed");
        }
    }
}