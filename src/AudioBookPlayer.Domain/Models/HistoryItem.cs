using System;

namespace AudioBookPlayer.Domain.Models
{
    public enum MediaAction
    {

    }

    public sealed class HistoryItem
    {
        public long MediaId
        {
            get;
            set;
        }

        public MediaAction Action
        {
            get;
            set;
        }

        public DateTime Created
        {
            get;
            set;
        }
    }
}