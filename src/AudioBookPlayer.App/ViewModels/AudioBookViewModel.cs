using System;
using Android.OS;
using Android.Support.V4.Media;

namespace AudioBookPlayer.App.ViewModels
{
    internal sealed class AudioBookViewModel : Java.Lang.Object
    {
        public string Title
        {
            get;
        }

        public TimeSpan Duration
        {
            get;
        }

        public string MediaId
        {
            get;
        }

        public AudioBookViewModel(string mediaId, string title, TimeSpan duration)
        {
            MediaId = mediaId;
            Title = title;
            Duration = duration;
        }

        public static AudioBookViewModel From(MediaBrowserCompat.MediaItem mediaItem)
        {
            var mediaId = mediaItem.MediaId;
            var title = mediaItem.Description.Title;
            var duration = (mediaItem.Description?.Extras ?? Bundle.Empty).GetDouble("Book.Duration", 0.0d);
            return new AudioBookViewModel(mediaId, title, TimeSpan.FromMilliseconds(duration));
        }
    }
}