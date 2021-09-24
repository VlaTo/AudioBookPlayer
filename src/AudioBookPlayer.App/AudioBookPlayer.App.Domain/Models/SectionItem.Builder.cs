using AudioBookPlayer.App.Domain.Core;

namespace AudioBookPlayer.App.Domain.Models
{
    public sealed partial class SectionItem
    {
        public sealed class Builder : IBuilder<SectionItem>
        {
            private EntityId bookId;
            private int index;
            private string title;
            private string contentUri;

            public Builder SetBookId(EntityId value)
            {
                bookId = value;
                return this;
            }

            public Builder SetIndex(int value)
            {
                index = value;
                return this;
            }

            public Builder SetTitle(string value)
            {
                title = value;
                return this;
            }

            public Builder SetContentUri(string value)
            {
                contentUri = value;
                return this;
            }

            public SectionItem Build()
            {
                var sectionItem = new SectionItem
                {
                    BookId = bookId,
                    Index = index,
                    Title = title,
                    ContentUri = contentUri
                };

                return sectionItem;
            }
        }
    }
}