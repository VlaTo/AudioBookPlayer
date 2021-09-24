using System;
using AudioBookPlayer.App.Domain.Core;
using LiteDB;

namespace AudioBookPlayer.App.Persistence.LiteDb.Models
{
    [Serializable]
    public sealed class Chapter : IEntity
    {
        [BsonField("title")]
        public string Title
        {
            get;
            set;
        }

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
    }
}