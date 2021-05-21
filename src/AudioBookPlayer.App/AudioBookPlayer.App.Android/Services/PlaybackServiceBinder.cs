using Android.OS;

namespace AudioBookPlayer.App.Droid.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class PlaybackServiceBinder : Binder
    {
        /// <summary>
        /// 
        /// </summary>
        public PlaybackService Service
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        public PlaybackServiceBinder(PlaybackService service)
            : base()
        {
            Service = service;
        }
    }
}