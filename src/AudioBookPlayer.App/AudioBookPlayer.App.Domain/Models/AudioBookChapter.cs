using System;
using System.Collections.Generic;
using AudioBookPlayer.App.Domain.Collections;

namespace AudioBookPlayer.App.Domain.Models
{
    public sealed class AudioBookChapter
    {
        private TimeSpan? duration;

        public AudioBook AudioBook
        {
            get;
        }

        public string Title
        {
            get; 
        }

        public TimeSpan Start
        {
            get;
        }

        public TimeSpan Duration
        {
            get
            {
                if (false == duration.HasValue)
                {
                    duration = TimeSpan.Zero;

                    foreach (var fragment in Fragments)
                    {
                        duration += fragment.Duration;
                    }
                }

                return duration.Value;
            }
        }

        public TimeSpan End => Start + Duration;

        public AudioBookPart Part
        {
            get;
        }

        public IList<AudioBookChapterFragment> Fragments
        {
            get;
        }

        public AudioBookChapter(AudioBook audioBook, string title, TimeSpan start, AudioBookPart part = null)
        {
            AudioBook = audioBook;
            Title = title;
            Start = start;
            Fragments = new OwnedCollection<AudioBookChapterFragment>(OnCollectionChanged);
            Part = part;
        }

        private void OnCollectionChanged(CollectionChange action, int index)
        {
            duration = null;
        }
    }
}