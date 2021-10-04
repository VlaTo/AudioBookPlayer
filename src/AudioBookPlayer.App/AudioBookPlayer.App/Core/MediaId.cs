using System;
using System.Text;
using AudioBookPlayer.App.Domain.Models;

namespace AudioBookPlayer.App.Core
{
    public sealed class MediaId
    {
        private const string AudioBookPrefix = "audiobook:";
        private const string SectionPrefix = "section:";
        private const string ChapterPrefix = "chapter:";
        private const string RootStr = @"/";
        private const char Delimiter = '/';

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
                return "@empty";
            }

            var builder = new StringBuilder();
            
            builder.Append(AudioBookPrefix);
            builder.AppendFormat("{0:D}", (long)BookId);

            if (null != SectionIndex)
            {
                builder.Append(Delimiter);
                builder.Append(SectionPrefix);
                builder.AppendFormat("{0:D}", SectionIndex.Value);
            }

            if (null != ChapterIndex)
            {
                if (null == SectionIndex)
                {
                    throw new Exception();
                }

                builder.Append(Delimiter);
                builder.Append(ChapterPrefix);
                builder.AppendFormat("{0:D}", ChapterIndex.Value);
            }

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
            if (false != String.IsNullOrEmpty(s))
            {
                value = MediaId.Empty;

                return false;
            }

            // var str = s.Substring(RootStr.Length);
            var parts = s.Split(Delimiter, StringSplitOptions.RemoveEmptyEntries);
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
                        if (TryParseInt(part, SectionPrefix, out var index))
                        {
                            builder.SetSectionIndex(index);
                        }

                        break;
                    }

                    case 2:
                    {
                        if (TryParseInt(part, ChapterPrefix, out var index))
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

        private static bool TryParseInt(string s, string prefix, out int value)
        {
            if (false == String.IsNullOrEmpty(s) && s.StartsWith(prefix))
            {
                var str = s.Substring(prefix.Length);

                if (int.TryParse(str, out value))
                {
                    return true;
                }
            }

            value = default;

            return false;
        }
    }
}