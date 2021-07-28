using System;
using System.Collections.Generic;
using AudioBookPlayer.App.Domain.Collections;

namespace AudioBookPlayer.App.Domain.Models
{
    public sealed class AudioBook
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
            Chapters = new OwnedCollection<AudioBookChapter>(OnChaptersCollectionChanged);
            Images = new OwnedCollection<AudioBookImage>(OnImagesCollectionChanged);
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

        private void OnImagesCollectionChanged(CollectionChange action, int index)
        {
            throw new NotImplementedException();
        }
    }
}
