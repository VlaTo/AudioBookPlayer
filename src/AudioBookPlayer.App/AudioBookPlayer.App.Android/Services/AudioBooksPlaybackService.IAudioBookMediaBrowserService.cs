namespace AudioBookPlayer.App.Android.Services
{
    public partial class AudioBooksPlaybackService
    {
        public interface IAudioBookMediaBrowserService
        {
            public const string UpdateLibrary = "AUDIOBOOKPLAYER_UPDATE_LIBRARY";
            public const string SubscribePlayback = "AUDIOBOOKPLAYER_PLAYBACK_SUBSCRIBE";

            public const string NoRoot = "@empty@";
            public const string Recent = "__RECENT__";
        }
    }
}