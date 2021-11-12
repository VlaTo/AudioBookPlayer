using AudioBookPlayer.App.Domain.Core;
using LiteDB;
using System;

namespace AudioBookPlayer.App.Persistence.LiteDb.Models
{
    [Serializable]
    public sealed class Activity : IEntity
    {
        [BsonId(true), BsonField("_id")]
        public long Id
        {
            get;
            set;
        }

        [BsonField("mid")]
        public string MediaId
        {
            get;
            set;
        }

        [BsonField("qid")]
        public long QueueId
        {
            get;
            set;
        }

        [BsonField("mpos")]
        public long MediaPosition
        {
            get;
            set;
        }

        [BsonField("time")]
        public DateTime Time
        {
            get;
            set;
        }
    }
}