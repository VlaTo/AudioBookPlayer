using System;
using System.Collections.Generic;

namespace AudioBookPlayer.Domain.Models
{
    public sealed class AudioBookChapter
    {
        public AudioBook AudioBook
        {
            get;
        }

        public string Title
        {
            get;
            set;
        }

        public TimeSpan Offset
        {
            get;
            set;
        }

        public TimeSpan Duration
        {
            get;
            set;
        }

        public TimeSpan End => Offset + Duration;

        public AudioBookSection Section
        {
            get;
        }

        public IReadOnlyList<AudioBookFragment> Fragments
        {
            get;
            set;
        }

        public AudioBookChapter(AudioBook audioBook, AudioBookSection section)
        {
            AudioBook = audioBook;
            Section = section;
            Fragments = new ArrayList<AudioBookFragment>();
        }
    }
}