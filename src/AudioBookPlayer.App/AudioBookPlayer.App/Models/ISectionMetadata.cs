using System;

namespace AudioBookPlayer.App.Models
{
    public interface ISectionMetadata
    {
        int Index
        {
            get;
        }

        string Name
        {
            get;
        }

        TimeSpan Start
        {
            get;
        }

        TimeSpan Duration
        {
            get;
        }
    }
}