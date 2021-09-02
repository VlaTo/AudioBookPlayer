using System;
using LiteDB;

namespace AudioBookPlayer.App.Persistence.LiteDb.Models
{
    [Serializable]
    public sealed class Section
    {
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