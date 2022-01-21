using System;
using LiteDB;

namespace AudioBookPlayer.Data.Persistence.Entities
{
    [Serializable]
    public sealed class Chapter : IEntity
    {
        [BsonField("order")]
        public int Order
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

        [BsonField("offset")]
        public TimeSpan Offset
        {
            get;
            set;
        }

        [BsonField("duration")]
        public TimeSpan Duration
        {
            get;
            set;
        }

        [BsonIgnore]
        public TimeSpan End => Offset + Duration;
    }
}