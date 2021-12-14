using System;
using Android.Support.V4.Media;
using AndroidX.MediaRouter.Media;
using AudioBookPlayer.Domain;

namespace AudioBookPlayer.App.Core
{
    internal static class AudioBookDescriptionBuilder
    {
        public static AudioBookDescription From(MediaBrowserCompat.MediaItem mediaItem)
        {
            var title = mediaItem.Description.Title;
            var duration = mediaItem.Description.Extras?.GetDouble(MediaItemMetadata.KeyDuration, 0.0d) ?? 0.0d;
            var description = new AudioBookDescription(mediaItem.MediaId, title, TimeSpan.FromMilliseconds(duration));

            return description;
        }
    }
}