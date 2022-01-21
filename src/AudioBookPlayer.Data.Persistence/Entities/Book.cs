﻿using System;
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

        [BsonField("description")]
        public string Description
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

        [BsonField("duration")]
        public TimeSpan Duration
        {
            get;
            set;
        }

        [BsonField("authors")]
        public Author[] Authors
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

        [BsonField("sections")]
        public string[] Images
        {
            get;
            set;
        }

        public Book()
        {
            Authors = Array.Empty<Author>();
            Sections = Array.Empty<Section>();
            Images = Array.Empty<string>();
        }
    }
}