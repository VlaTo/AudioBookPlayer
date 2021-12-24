using System;
using LiteDB;

namespace AudioBookPlayer.Data.Persistence.Entities
{
    [Serializable]
    public sealed class Book : IEntity
    {
        [BsonId(true), BsonField("_id")]
        public long Id
        {
            get;
            set;
        }

        [BsonField("title")]
        public string Title
        {
            get;
            set;
        }
    }
}