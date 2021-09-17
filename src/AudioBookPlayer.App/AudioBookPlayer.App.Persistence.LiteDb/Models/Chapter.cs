using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;

namespace AudioBookPlayer.App.Persistence.LiteDb.Models
{
    [Serializable]
    public sealed class Chapter
    {
        [BsonIgnore]
        private TimeSpan? duration;

        [BsonIgnore]
        private IEnumerable<Fragment> fragments;

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
        public IEnumerable<Fragment> Fragments
        {
            get => fragments;
            set
            {
                if (ReferenceEquals(fragments, value))
                {
                    return;
                }

                fragments = value;
                duration = null;
            }
        }

        [BsonIgnore]
        public TimeSpan Duration
        {
            get
            {
                if (false == duration.HasValue)
                {
                    duration = Fragments.Aggregate(TimeSpan.Zero, (span, fragment) => span + fragment.Duration);
                }

                return duration.Value;
            }
        }
    }
}