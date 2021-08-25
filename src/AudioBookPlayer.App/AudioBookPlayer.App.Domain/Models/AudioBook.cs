using System;
using System.Collections.Generic;
using AudioBookPlayer.App.Domain.Collections;
using AudioBookPlayer.App.Domain.Data;

namespace AudioBookPlayer.App.Domain.Models
{
    public sealed class AudioBook : IEntity
    {
        private TimeSpan? duration;

        public long? Id
        {
            get;
            set;
        }

        public string Title
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

                    foreach (var chapter in Chapters)
                    {
                        duration += chapter.Duration;
                    }
                }

                return duration.Value;
            }
        }

        public string Synopsis
        {
            get;
            set;
        }

        public DateTime? AddedToLibrary
        {
            get;
            set;
        }

        public IList<AudioBookAuthor> Authors
        {
            get;
        }

        public IList<AudioBookPart> Parts
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

        public AudioBook(string title, long? id = null)
        {
            Id = id;
            Title = title;
            Authors = new List<AudioBookAuthor>();
            Parts = new List<AudioBookPart>();
            Images = new List<AudioBookImage>();
            Chapters = new OwnedCollection<AudioBookChapter>(OnChaptersCollectionChanged);
            SourceFiles = new OwnedCollection<AudioBookSourceFile>(OnSourceFilesCollectionChanged);
        }

        private void OnChaptersCollectionChanged(CollectionChange action, int index)
        {
            duration = null;
        }

        private void OnSourceFilesCollectionChanged(CollectionChange action, int index)
        {
            ;
        }
    }
}
