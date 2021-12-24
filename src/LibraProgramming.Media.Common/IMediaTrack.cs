using System;

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
    }
}
