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

        public int Hash
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

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = MediaId.GetHashCode();

                hashCode = (hashCode * 397) ^ (Title?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ Duration.GetHashCode();
                hashCode = (hashCode * 397) ^ (Description?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ Created.GetHashCode();
                hashCode = (hashCode * 397) ^ HashAuthors();
                hashCode = (hashCode * 397) ^ HashChapters();

                return hashCode;
            }
        }

        private int HashAuthors()
        {
            unchecked
            {
                var hashCode = 0;

                for (var index = 0; index < Authors.Count; index++)
                {
                    hashCode = (hashCode * 397) ^ Authors[index].GetHashCode();
                }

                return hashCode;
            }
        }

        private int HashChapters()
        {
            unchecked
            {
                var hashCode = 0;

                for (var index = 0; index < Chapters.Count; index++)
                {
                    var chapter = Chapters[index];
                    hashCode = (hashCode * 397) ^ chapter.GetHashCode();
                }

                return hashCode;
            }
        }
    }
}