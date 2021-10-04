using Android.Support.V4.Media;
using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Domain.Models;

namespace AudioBookPlayer.App.Android.Extensions
{
    internal static class AudioBookSectionExtensions
    {
        public static MediaBrowserCompat.MediaItem ToMediaItem(this AudioBookSection audioBookSection)
        {
            const int flags = MediaBrowserCompat.MediaItem.FlagBrowsable | MediaBrowserCompat.MediaItem.FlagPlayable;

            /*Bundle BuildExtendedInfo()
            {
                var info = new Bundle();

                info.PutString("ContentUri", audioBookSection.);
                //info.PutDouble("Position", TimeSpan.Zero.TotalMilliseconds);
                //info.PutBoolean("IsCompleted", false);
                //info.PutStringArray("Authors", audioBookSection.Authors);

                return info;
            }*/

            var audioBook = audioBookSection.AudioBook;
            var mediaItemId = new MediaId(audioBook.Id, audioBookSection.Index);
            var mediaUri = global::Android.Net.Uri.Parse(audioBookSection.ContentUri);
            var description = new MediaDescriptionCompat.Builder()
                .SetMediaId(mediaItemId.ToString())
                .SetTitle(audioBookSection.Name)
                .SetMediaUri(mediaUri)
                // .SetExtras(BuildExtendedInfo())
                ;

            return new MediaBrowserCompat.MediaItem(description.Build(), flags);
        }
    }
}