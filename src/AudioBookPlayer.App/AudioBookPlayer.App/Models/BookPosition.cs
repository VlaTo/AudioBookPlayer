using System;

namespace AudioBookPlayer.App.Models
{
    public sealed class BookPosition
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

        public BookPosition(long bookId, int chapterIndex, TimeSpan chapterPosition)
        {
            BookId = bookId;
            ChapterIndex = chapterIndex;
            ChapterPosition = chapterPosition;
        }

        public BookPosition(long bookId, int chapterIndex)
            : this(bookId, chapterIndex, TimeSpan.Zero)
        {
        }
    }
}