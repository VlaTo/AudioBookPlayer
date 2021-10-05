using System;
using Android.Support.V4.Media;
using AudioBookPlayer.App.Core;

namespace AudioBookPlayer.App.Android.Core
{
    internal sealed class AudioBookMediaMetadataDescription : IMediaMetadataDescription
    {
        private readonly MediaMetadataCompat metadata;

        public string Title => metadata.Description.Title;

        public string Subtitle => metadata.Description.Subtitle;
        
        public string Description => metadata.Description.Description;
        
        public TimeSpan Duration => TimeSpan.FromMilliseconds(metadata.Bundle.GetLong("Duration"));

        public string AlbumArtUri => metadata.Description.IconUri.ToString();

        public AudioBookMediaMetadataDescription(MediaMetadataCompat metadata)
        {
            this.metadata = metadata;
        }
    }
}