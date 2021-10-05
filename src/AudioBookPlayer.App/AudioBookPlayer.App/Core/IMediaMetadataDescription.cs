using System;

namespace AudioBookPlayer.App.Core
{
    public interface IMediaMetadataDescription
    {
        string Title
        {
            get;
        }

        string Subtitle
        {
            get;
        }

        string Description
        {
            get;
        }

        TimeSpan Duration
        {
            get;
        }

        string AlbumArtUri
        {
            get;
        }
    }
}