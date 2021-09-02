using System;
using LiteDB;

namespace AudioBookPlayer.App.Persistence.LiteDb.Models
{
    [Serializable]
    public sealed class Fragment
    {
        [BsonField("start")]
        public TimeSpan Start
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
        public TimeSpan End => Start + Duration;

        [BsonField("source")]
        public string ContentUri
        {
            get;
            set;
        }
    }
}