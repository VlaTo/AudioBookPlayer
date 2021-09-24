namespace AudioBookPlayer.App.Android.Services
{
    public partial class AudioBookMediaBrowserService
    {
        public interface IAudioBookMediaBrowserService
        {
            public const string UpdateLibrary = "AUDIOBOOKPLAYER_UPDATE_LIBRARY";

            public const string NoRoot = "@empty@";
            public const string Recent = "__RECENT__";
        }
    }
}