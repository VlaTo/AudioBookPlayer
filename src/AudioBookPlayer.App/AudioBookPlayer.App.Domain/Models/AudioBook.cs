using System;
using System.Collections.Generic;
using System.Linq;

namespace AudioBookPlayer.App.Domain.Models
{
    public sealed class AudioBook
    {
        public EntityId Id
        {
            get;
            set;
        }

        public string Title
        {
            get;
        }

        public string Synopsis
        {
            get;
            set;
        }

        public DateTime? Created
        {
            get;
            set;
        }

        public IList<AudioBookAuthor> Authors
        {
            get;
        }

        public IList<AudioBookSection> Sections
        {
            get;
        }

        public IList<AudioBookChapter> Chapters
        {
            get;
        }

        public IList<AudioBookImage> Images
        {
            get;
        }

        public IList<AudioBookSourceFile> SourceFiles
        {
            get;
        }

        public AudioBook(EntityId id, string title)
        {
            Id = id;
            Title = title;
            Authors = new List<AudioBookAuthor>();
            Sections = new List<AudioBookSection>();
            Images = new List<AudioBookImage>();
            Chapters = new List<AudioBookChapter>();
            SourceFiles = new List<AudioBookSourceFile>();
        }

        public TimeSpan GetDuration()
        {
            return Chapters.Aggregate(TimeSpan.Zero, (current, chapter) => current + chapter.Duration);
        }
    }
}
