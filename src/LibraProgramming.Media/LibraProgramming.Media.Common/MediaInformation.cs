using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LibraProgramming.Media.Common
{
    public sealed class MediaInformation
    {
        public string BookTitle
        {
            get; 
        }

        public string[] BookAuthors
        {
            get;
        }

        public TimeSpan? BookDuration
        {
            get;
        }

        public ushort? BookYear
        {
            get;
        }

        public IReadOnlyCollection<IMediaTrack> Tracks
        {
            get;
        }

        public  MetaInformation Meta
        {
            get;
        }

        public MediaInformation(
            IList<IMediaTrack> tracks,
            MetaInformation meta,
            string bookTitle = null,
            string[] bookAuthors = null,
            TimeSpan? bookDuration = null,
            ushort? bookYear = null)
        {
            Tracks = new ReadOnlyCollection<IMediaTrack>(tracks);
            Meta = meta;
            BookTitle = bookTitle;
            BookAuthors = bookAuthors ?? Array.Empty<string>();
            BookDuration = bookDuration;
            BookYear = bookYear;
        }
    }
}