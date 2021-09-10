using Android.OS;
using AudioBookPlayer.App.Services;

namespace AudioBookPlayer.App.Android.Services
{
    public class AudioBookPlaybackServiceBinder : Binder
    {
        /// <summary>
        /// 
        /// </summary>
        public IAudioBookPlaybackService Service
        {
            get;
        }

        public AudioBookPlaybackServiceBinder(AudioBookPlaybackService service)
        {
            Service = service;
        }

        public void Unbind()
        {
            ;
        }
    }
}