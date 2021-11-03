using System;
using System.Text;
using AudioBookPlayer.App.Domain.Models;

namespace AudioBookPlayer.App.Core
{
    public sealed class MediaId : IEquatable<MediaId>
    {
        private const string AudioBookPrefix = "abid:";
        private const string SectionPrefix = "s";
        private const string ChapterPrefix = "ch";
        private const string RootStr = @"@root";
        private const string EmptyStr = @"@empty";
        private const char Delimiter = '/';
        private const char IndexOpenBracket = '[';
        private const char IndexCloseBracket = ']';
        private const string OpenTag = "{";
        private const string CloseTag = "}";

        public static readonly MediaId Empty;

        public EntityId BookId
        {
            get;
        }

        public int? SectionIndex
        {
            get;
        }

        public int? ChapterIndex
        {
            get;
        }

        public MediaId(EntityId bookId, int sectionIndex, int chapterIndex)
            : this(bookId, sectionIndex)
        {
            ChapterIndex = chapterIndex;
        }

        public MediaId(EntityId bookId, int sectionIndex)
            : this(bookId)
        {
            SectionIndex = sectionIndex;
        }

        public MediaId(EntityId bookId)
        {
            BookId = bookId;
        }

        private MediaId()
        {
            BookId = EntityId.Empty;
            SectionIndex = null;
            ChapterIndex = null;
        }

        static MediaId()
        {
            Empty = new MediaId();
        }

        public override string ToString()
        {
            if (ReferenceEquals(Empty, this) || EntityId.Empty == BookId)
            {
                return EmptyStr;
            }

            var builder = new StringBuilder();
            
            builder.Append(OpenTag);
            builder.Append(AudioBookPrefix);
            builder.AppendFormat("{0:D}", (long)BookId);

            if (null != SectionIndex)
            {
                builder.Append(Delimiter);
                builder.Append(SectionPrefix);
                builder.Append(IndexOpenBracket);
                builder.AppendFormat("{0:D}", SectionIndex.Value);
                builder.Append(IndexCloseBracket);
            }

            if (null != ChapterIndex)
            {
                if (null == SectionIndex)
                {
                    throw new Exception();
                }

                builder.Append(Delimiter);
                builder.Append(ChapterPrefix);
                builder.Append(IndexOpenBracket);
                builder.AppendFormat("{0:D}", ChapterIndex.Value);
                builder.Append(IndexCloseBracket);
            }

            builder.Append(CloseTag);

            return builder.ToString();
        }

        public static MediaId Parse(string s)
        {
            if (TryParse(s, out var result))
            {
                return result;
            }

            throw new Exception();
        }

        public static bool TryParse(string s, out MediaId value)
        {
            if (String.IsNullOrEmpty(s))
            {
                value = Empty;
                return false;
            }

            if (false == s.StartsWith(OpenTag) || false == s.EndsWith(CloseTag))
            {
                value = Empty;
                return false;
            }

            var innerStr = s.Substring(OpenTag.Length, s.Length - (OpenTag.Length + CloseTag.Length));
            var parts = innerStr.Split(Delimiter, StringSplitOptions.RemoveEmptyEntries);
            var builder = new MediaIdBuilder();

            for (var partIndex = 0; partIndex < parts.Length; partIndex++)
            {
                var part = parts[partIndex];

                switch (partIndex)
                {
                    case 0:
                    {
                        if (TryParseEntityId(part, AudioBookPrefix, out var bookId))
                        {
                            builder.SetBookId(bookId);
                        }

                        break;
                    }

                    case 1:
                    {
                        if (TryParseIndex(part, SectionPrefix, out var index))
                        {
                            builder.SetSectionIndex(index);
                        }

                        break;
                    }

                    case 2:
                    {
                        if (TryParseIndex(part, ChapterPrefix, out var index))
                        {
                            builder.SetChapterIndex(index);
                        }

                        break;
                    }

                    default:
                    {
                        throw new Exception();
                    }
                }
            }

            value = builder.Build();

            return true;
        }

        private static bool TryParseEntityId(string s, string prefix, out EntityId value)
        {
            if (false == String.IsNullOrEmpty(s) && s.StartsWith(prefix))
            {
                var str = s.Substring(prefix.Length);

                if (EntityId.TryParse(str, out value))
                {
                    return true;
                }
            }

            value = EntityId.Empty;

            return false;
        }

        private static bool TryParseIndex(string s, string prefix, out int value)
        {
            if (false == String.IsNullOrEmpty(s) && s.StartsWith(prefix))
            {
                var str = s.Substring(prefix.Length);

                if (str.StartsWith(IndexOpenBracket) && s.EndsWith(IndexCloseBracket))
                {
                    str = str.Substring(1, str.Length - 2);

                    if (int.TryParse(str, out value))
                    {
                        return true;
                    }
                }
            }

            value = default;

            return false;
        }

        public bool Equals(MediaId other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return BookId.Equals(other.BookId) && SectionIndex == other.SectionIndex && ChapterIndex == other.ChapterIndex;
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || (obj is MediaId other && Equals(other));
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(BookId, SectionIndex, ChapterIndex);
        }
    }
}