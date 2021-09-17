using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace AudioBookPlayer.App.Domain.Models
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct BookId : IEquatable<BookId>
    {
        private const string EmptyString = "@empty";

        private readonly long? id;

        public static readonly BookId Empty;

        private BookId(long? id)
        {
            this.id = id;
        }

        static BookId()
        {
            Empty = new BookId(null);
        }

        public override string ToString()
        {
            return id.HasValue ? id.Value.ToString("D", CultureInfo.CurrentUICulture) : EmptyString;
        }

        public static BookId From(long id) => new BookId(id);

        public static bool TryParse([MaybeNull] string s, out BookId value)
        {
            if (String.IsNullOrEmpty(s) || String.Equals(EmptyString, s) || false == long.TryParse(s, out var id))
            {
                value = Empty;
                return false;
            }

            value = From(id);

            return true;
        }

        public static bool Equals(BookId x, BookId y) => x.Equals(y);

        public bool Equals(BookId other) => id == other.id;

        public override bool Equals(object obj)
        {
            if (obj is BookId other)
            {
                return Equals(other);
            }

            if (obj is long value)
            {
                return id.HasValue && value == id.Value;
            }

            return false;
        }

        public override int GetHashCode() => id.GetHashCode();

        public static bool operator ==(BookId left, BookId right) => Equals(left, right);

        public static bool operator !=(BookId left, BookId right) => !(left == right);

        public static explicit operator long(BookId source) => source.id.GetValueOrDefault();

        public static implicit operator BookId(long value) => From(value);
    }
}