using System;
using AudioBookPlayer.App.Domain.Models;

namespace AudioBookPlayer.App.Core
{
    public sealed class MediaIdBuilder
    {
        private EntityId? bookId;
        private int? sectionIndex;
        private int? chapterIndex;

        public MediaIdBuilder(EntityId bookId, int sectionIndex, int chapterIndex)
            : this()
        {
            this.bookId = bookId;
            this.sectionIndex = sectionIndex;
            this.chapterIndex = chapterIndex;
        }

        public MediaIdBuilder(EntityId bookId, int sectionIndex)
            : this()
        {
            this.bookId = bookId;
            this.sectionIndex = sectionIndex;
        }

        public MediaIdBuilder(EntityId bookId)
            : this()
        {
            this.bookId = bookId;
        }

        public MediaIdBuilder()
        {
        }

        public MediaIdBuilder SetBookId(EntityId bookId)
        {
            this.bookId = bookId;
            return this;
        }

        public MediaIdBuilder SetSectionIndex(int index)
        {
            sectionIndex = index;
            return this;
        }

        public MediaIdBuilder SetChapterIndex(int index)
        {
            chapterIndex = index;
            return this;
        }

        public MediaId Build()
        {
            if (null == bookId)
            {
                throw new Exception();
            }

            if (null == chapterIndex)
            {
                if (null == sectionIndex)
                {
                    return new MediaId(bookId.Value);
                }

                return new MediaId(bookId.Value, sectionIndex.Value);
            }

            if (null == sectionIndex)
            {
                throw new Exception();
            }

            return new MediaId(bookId.Value, sectionIndex.Value, chapterIndex.Value);
        }
    }
}