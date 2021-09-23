using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using AudioBookPlayer.App.Domain.Core;

namespace AudioBookPlayer.App.Domain.Models
{
    public sealed partial class BookItem
    {
        /// <summary>
        /// 
        /// </summary>
        public sealed class Builder : IBuilder<BookItem>
        {
            private EntityId id;
            private string title;
            private TimeSpan duration;
            private TimeSpan position;
            private bool isCompleted;
            private readonly List<string> authors;
            private readonly List<string> covers;

            public Builder()
            {
                id = EntityId.Empty;
                duration = TimeSpan.Zero;
                position = TimeSpan.Zero;
                authors = new List<string>();
                covers = new List<string>();
            }

            [return: NotNull]
            public Builder SetId(EntityId value)
            {
                id = value;
                return this;
            }

            [return: NotNull]
            public Builder SetTitle([NotNull] string value)
            {
                if (null == value)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                title = value;

                return this;
            }

            [return: NotNull]
            public Builder SetAuthors(string[] value)
            {
                authors.Clear();
                authors.AddRange(value);

                return this;
            }

            [return: NotNull]
            public Builder AddAuthor([NotNull] string value)
            {
                authors.Add(value);
                return this;
            }

            [return: NotNull]
            public Builder SetDuration(TimeSpan value)
            {
                duration = value;
                return this;
            }

            [return: NotNull]
            public Builder SetPosition(TimeSpan value)
            {
                position = value;
                return this;
            }

            [return: NotNull]
            public Builder SetIsCompleted(bool value)
            {
                isCompleted = value;
                return this;
            }

            [return: NotNull]
            public Builder SetCovers(string[] value)
            {
                covers.Clear();
                covers.AddRange(value);

                return this;
            }

            [return: NotNull]
            public Builder AddCover([NotNull] string value)
            {
                covers.Add(value);
                return this;
            }

            [return: NotNull]
            public BookItem Build()
            {
                return new BookItem(id, title ?? String.Empty)
                {
                    Authors = authors.ToArray(),
                    Duration = duration,
                    Position = position,
                    IsCompleted = isCompleted,
                    Covers = covers.ToArray()
                };
            }
        }
    }
}