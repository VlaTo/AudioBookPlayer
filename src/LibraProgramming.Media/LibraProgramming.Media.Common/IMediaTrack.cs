using System;
using System.IO;

namespace LibraProgramming.Media.Common
{
    public interface IMediaTrack
    {
        string Title
        {
            get;
        }

        TimeSpan Duration
        {
            get;
        }

        Stream GetMediaStream();
    }
}
