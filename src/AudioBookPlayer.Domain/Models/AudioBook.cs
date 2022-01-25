using System;
using System.Collections.Generic;

namespace AudioBookPlayer.Domain.Models
{
    public sealed class AudioBook
    {
        public long? Id
        {
            get;
            set;
        }
        public long MediaId
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

        public string Description
        {
            get;
            set;
        }

        public DateTime Created
        {
            get;
            set;
        }

        public int Version
        {
            get;
            set;
        }

        public IReadOnlyList<AudioBookAuthor> Authors
        {
            get;
            set;
        }

        public IReadOnlyList<AudioBookSection> Sections
        {
            get;
            set;
        }

        public IReadOnlyList<AudioBookChapter> Chapters
        {
            get;
            set;
        }

        public IReadOnlyList<AudioBookSourceFile> SourceFiles
        {
            get;
            set;
        }

        public IReadOnlyList<IAudioBookImage> Images
        {
            get;
            set;
        }

        public AudioBook()
        {
            Authors = new ArrayList<AudioBookAuthor>();
            Sections = new ArrayList<AudioBookSection>();
            Chapters = new ArrayList<AudioBookChapter>();
            SourceFiles = new ArrayList<AudioBookSourceFile>();
            Images = new ArrayList<IAudioBookImage>();
        }
    }
}