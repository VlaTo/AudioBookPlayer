using System.Diagnostics.CodeAnalysis;
using Android.Support.V4.Media;
using AudioBookPlayer.App.Domain.Models;

namespace AudioBookPlayer.App.Android.Services.Builders
{
    internal sealed class PublicBookBuilder : BookBuilder
    {

        [return: NotNull]
        public override AudioBook BuildAudioBookFrom([NotNull] MediaBrowserCompat.MediaItem mediaItem)
        {
            var id = MediaBookId.TryParse(mediaItem.MediaId, out var mediaId) ? mediaId.Id : BookId.Empty;
            var audioBook = new AudioBook(id, mediaItem.Description.Title);

            BuildAuthors(audioBook, mediaItem);

            return audioBook;
        }
    }
}