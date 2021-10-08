using System;
using Android.Support.V4.Media;
using AudioBookPlayer.App.Models;

namespace AudioBookPlayer.App.Android.Models
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class AudioBookMetadata : IAudioBookMetadata
    {
        private readonly MediaMetadataCompat metadata;

        public string Title => metadata.Description.Title;

        public string Subtitle => metadata.Description.Subtitle;
        
        public string Description => metadata.Description.Description;
        
        public TimeSpan Duration => TimeSpan.FromMilliseconds(metadata.Bundle.GetLong("Duration"));

        public string AlbumArtUri => metadata.Description.IconUri.ToString();

        public AudioBookMetadata(MediaMetadataCompat metadata)
        {
            this.metadata = metadata;
        }
    }
}