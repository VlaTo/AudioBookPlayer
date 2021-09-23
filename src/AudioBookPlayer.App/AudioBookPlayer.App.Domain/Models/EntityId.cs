using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace AudioBookPlayer.App.Domain.Models
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct EntityId : IEquatable<EntityId>
    {
        private const string EmptyString = "@empty";

        private readonly long? id;

        public static readonly EntityId Empty;

        private EntityId(long? id)
        {
            this.id = id;
        }

        static EntityId()
        {
            Empty = new EntityId(null);
        }

        public override string ToString()
        {
            return id.HasValue ? id.Value.ToString("D", CultureInfo.CurrentUICulture) : EmptyString;
        }

        public static EntityId From(long id) => new EntityId(id);

        public static bool TryParse([MaybeNull] string s, out EntityId value)
        {
            if (String.IsNullOrEmpty(s) || String.Equals(EmptyString, s) || false == long.TryParse(s, out var id))
            {
                value = Empty;
                return false;
            }

            value = From(id);

            return true;
        }

        public static bool Equals(EntityId x, EntityId y) => x.Equals(y);

        public bool Equals(EntityId other) => id == other.id;

        public override bool Equals(object obj)
        {
            if (obj is EntityId other)
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

        public static bool operator ==(EntityId left, EntityId right) => Equals(left, right);

        public static bool operator !=(EntityId left, EntityId right) => !(left == right);

        public static explicit operator long(EntityId source) => source.id.GetValueOrDefault();

        public static implicit operator EntityId(long value) => From(value);
    }
}