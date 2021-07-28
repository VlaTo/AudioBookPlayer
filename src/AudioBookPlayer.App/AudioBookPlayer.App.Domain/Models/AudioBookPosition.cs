using System;

namespace AudioBookPlayer.App.Domain.Models
{
    public sealed class AudioBookPosition
    {
        public long BookId
        {
            get;
        }

        public int ChapterIndex
        {
            get;
        }

        public TimeSpan ChapterPosition
        {
            get;
        }

        public AudioBookPosition(long bookId, int chapterIndex, TimeSpan chapterPosition)
        {
            BookId = bookId;
            ChapterIndex = chapterIndex;
            ChapterPosition = chapterPosition;
        }

        public AudioBookPosition(long bookId, int chapterIndex)
            : this(bookId, chapterIndex, TimeSpan.Zero)
        {
        }
    }
}