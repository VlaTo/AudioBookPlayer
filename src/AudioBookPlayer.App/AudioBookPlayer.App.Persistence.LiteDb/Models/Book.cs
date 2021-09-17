using AudioBookPlayer.App.Persistence.LiteDb.Core;
using LiteDB;
using System;
using System.Linq;

namespace AudioBookPlayer.App.Persistence.LiteDb.Models
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

        [BsonField("synopsis")]
        public string Synopsis
        {
            get;
            set;
        }
        
        [BsonField("created")]
        public DateTime Created
        {
            get;
            set;
        }

        [BsonField("authors")]
        public string[] Authors
        {
            get;
            set;
        }

        [BsonField("covers")]
        public string[] Images
        {
            get;
            set;
        }

        [BsonField("sections")]
        public Section[] Sections
        {
            get;
            set;
        }

        [BsonCtor]
        public Book()
        {
            Authors = Array.Empty<string>();
            Images = Array.Empty<string>();
            Sections = Array.Empty<Section>();
        }

        public TimeSpan GetDuration()
        {
            return Sections
                .SelectMany(section => section.Chapters)
                .Aggregate(TimeSpan.Zero, (duration, chapter) => duration + chapter.GetDuration());
        }
    }
}