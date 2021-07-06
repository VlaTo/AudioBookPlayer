using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AudioBookPlayer.App.Models.Collections;

namespace AudioBookPlayer.App.Models
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
        }

        public ICollection<AudioBookAuthor> Authors
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

        public AudioBook(string title, long? id = null, string synopsis = null)
        {
            Id = id;
            Title = title;
            Synopsis = synopsis;
            Authors = new Collection<AudioBookAuthor>();
            Chapters = new OwnedCollection<AudioBookChapter>(OnChaptersCollectionChanged);
            Images = new OwnedCollection<AudioBookImage>(OnImagesCollectionChanged);
            SourceFiles = new OwnedCollection<AudioBookSourceFile>(OnSourceFilesCollectionChanged);
        }

        private void OnChaptersCollectionChanged(ChangeAction action, int index)
        {
            duration = null;
        }

        private void OnSourceFilesCollectionChanged(ChangeAction action, int index)
        {
            ;
        }

        private void OnImagesCollectionChanged(ChangeAction action, int index)
        {
            throw new NotImplementedException();
        }
    }
}
