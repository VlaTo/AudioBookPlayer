using System;
using System.Globalization;
using System.Text;

namespace AudioBookPlayer.Domain
{
    public sealed class MediaID : IEquatable<MediaID>
    {
        private const string RootStr = "//";
        private const string EmptyStr = "@empty@";
        private const char ChapterDelimiter = '/';
        private const char BookDelimiter = ':';
        private const string BookPrefix = "book";

        public static readonly MediaID Empty;

        public static readonly MediaID Root;

        public long BookId
        {
            get;
        }

        public long? ChapterId
        {
            get;
        }

        public bool HasChapterId => ChapterId.HasValue;

        public MediaID(long bookId, long? chapterId = null)
        {
            BookId = bookId;
            ChapterId = chapterId;
        }

        private MediaID()
        {
        }

        static MediaID()
        {
            Empty = new MediaID();
            Root = new MediaID();
        }

        public bool Equals(MediaID other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (ReferenceEquals(Empty, other))
            {
                return ReferenceEquals(Empty, this);
            }

            if (ReferenceEquals(Root, other))
            {
                return ReferenceEquals(Root, this);
            }

            return BookId == other.BookId && ChapterId == other.ChapterId;
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is MediaID other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (BookId.GetHashCode() * 397) ^ ChapterId.GetHashCode();
            }
        }

        public static bool TryParse(string str, out MediaID mediaId)
        {
            if (String.Equals(EmptyStr, str))
            {
                mediaId = Empty;
                return true;
            }

            if (String.Equals(RootStr, str))
            {
                mediaId = Root;
                return true;
            }

            if (str.StartsWith(RootStr))
            {
                str = str.Substring(RootStr.Length);

                if (str.StartsWith(BookPrefix))
                {
                    if (BookDelimiter == str[BookPrefix.Length])
                    {
                        var number = str.Substring(BookPrefix.Length + 1);

                        if (long.TryParse(number, NumberStyles.Integer, CultureInfo.InvariantCulture, out var bookId))
                        {
                            mediaId = new MediaID(bookId);
                            return true;
                        }
                    }
                }
            }

            /*var delimiter = str.IndexOf(ChapterDelimiter);

            if (-1 < delimiter)
            {
                var chapterStr = str.Substring(delimiter);

                if (long.TryParse(chapterStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out var chapter))
                {
                    chapterId = chapter;
                }

                str = str.Substring(0, delimiter - 1);
            }*/

            mediaId = Empty;

            return false;
        }

        public static bool operator ==(MediaID left, MediaID right) =>
            left?.Equals(right) ?? ReferenceEquals(null, right);

        public static bool operator !=(MediaID left, MediaID right)
        {
            return true;
        }

        public static implicit operator string(MediaID mediaId)
        {
            if (mediaId == Empty)
            {
                return EmptyStr;
            }

            if (mediaId == Root)
            {
                return RootStr;
            }

            var builder = new StringBuilder()
                .Append(RootStr)
                .Append(BookPrefix)
                .Append(BookDelimiter)
                .Append(mediaId.BookId);

            if (mediaId.HasChapterId)
            {
                builder
                    .Append(ChapterDelimiter)
                    .Append(mediaId.ChapterId);
            }

            return builder.ToString();
        }
    }
}
