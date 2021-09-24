using System;
using AudioBookPlayer.App.Domain.Core;
using LiteDB;

namespace AudioBookPlayer.App.Persistence.LiteDb.Models
{
    [Serializable]
    public sealed class Section : IEntity, IHasContentUri
    {
        [BsonField("source")]
        public string ContentUri
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

        [BsonField("chapters")]
        public Chapter[] Chapters
        {
            get;
            set;
        }
    }
}