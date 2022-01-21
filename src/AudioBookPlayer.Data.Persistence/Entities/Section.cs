using System;
using AudioBookPlayer.Domain;
using LiteDB;

namespace AudioBookPlayer.Data.Persistence.Entities
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

        public Section()
        {
            Chapters = Array.Empty<Chapter>();
        }
    }
}