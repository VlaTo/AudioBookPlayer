using System;
using LiteDB;

namespace AudioBookPlayer.Data.Persistence.Entities
{
    [Serializable]
    public class Author : IEntity
    {
        [BsonField("name")]
        public string Name
        {
            get;
            set;
        }
    }
}