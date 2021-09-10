using Android.OS;

namespace AudioBookPlayer.App.Android.Services
{
    public class AudioBookPlaybackServiceBinder : Binder
    {
        /// <summary>
        /// 
        /// </summary>
        public AudioBookPlaybackService Service
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        public AudioBookPlaybackServiceBinder(AudioBookPlaybackService service)
        {
            Service = service;
        }
    }
}