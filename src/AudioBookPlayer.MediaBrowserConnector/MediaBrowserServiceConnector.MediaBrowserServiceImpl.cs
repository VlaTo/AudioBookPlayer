using System.Collections.Generic;
using Android.OS;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;

namespace AudioBookPlayer.MediaBrowserConnector
{
    public sealed partial class MediaBrowserServiceConnector
    {

        public interface IHistoryCallback
        {
            void OnHistoryReady(IList<MediaBrowserCompat.MediaItem> list, Bundle options);

            void OnHistoryError(Bundle options);
        }

    }
}