using System;
using LiteDB;

namespace AudioBookPlayer.App.Persistence.LiteDb.Models
{
    [Serializable]
    public sealed class Chapter
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

        [BsonField("fragments")]
        public Fragment[] Fragments
        {
            get;
            set;
        }
    }
}