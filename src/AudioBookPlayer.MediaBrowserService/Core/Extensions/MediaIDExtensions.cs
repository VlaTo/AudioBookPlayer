using Android.Net;
using AudioBookPlayer.Domain;

namespace AudioBookPlayer.MediaBrowserService.Core.Extensions
{
    internal static class MediaIDExtensions
    {
        public static Uri ToUri(this MediaID mediaId)
        {
            return Uri.FromParts("abp", mediaId, null);
        }
    }
}