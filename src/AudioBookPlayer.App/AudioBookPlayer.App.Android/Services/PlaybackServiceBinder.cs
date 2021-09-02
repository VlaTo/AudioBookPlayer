﻿using Android.OS;

namespace AudioBookPlayer.App.Android.Services
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
        {
            Service = service;
        }
    }
}