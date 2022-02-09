using System;
using LiteDB;

namespace AudioBookPlayer.Data.Persistence.Entities
{
    [Serializable]
    public sealed class History : IEntity
    {
        [BsonId(true), BsonField("_id")]
        public long BookId
        {
            get;
            set;
        }

        [BsonField("items")]
        public HistoryEntry[] Entries
        {
            get;
            set;
        }
    }
}