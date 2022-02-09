using System;
using LiteDB;

namespace AudioBookPlayer.Data.Persistence.Entities
{
    [Serializable]
    public enum ActionType : byte
    {
        Paused,
        Play
    }

    [Serializable]
    public sealed class HistoryEntry : IEntity
    {
        [BsonField("act")]
        public ActionType Action
        {
            get;
            set;
        }

        [BsonField("date")]
        public DateTime DateTime
        {
            get;
            set;
        }
    }
}