using System;
using System.Collections.Generic;

namespace AudioBookPlayer.App.Models
{
    public sealed class AudioBook
    {
        public long? Id
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        public TimeSpan Duration
        {
            get; 
            set;
        }

        public string Synopsis
        {
            get;
            set;
        }

        public IList<string> Authors
        {
            get;
        }

        public IList<Chapter> Chapters
        {
            get;
        }

        public IList<BookImage> Images
        {
            get;
        }

        public AudioBook()
        {
            Authors = new List<string>();
            Chapters = new List<Chapter>();
            Images = new List<BookImage>();
        }
    }
}
