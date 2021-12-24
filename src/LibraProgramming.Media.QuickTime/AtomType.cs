using LibraProgramming.Media.QuickTime.Extensions;
using System;
using System.Diagnostics;
using System.Text;

namespace LibraProgramming.Media.QuickTime
{
    [DebuggerDisplay("Type: {ToString()}")]
    [DebuggerTypeProxy(typeof(BoxTypeDebug))]
    public sealed class AtomType : IEquatable<AtomType>
    {
        private readonly uint type;

        public static readonly AtomType Empty;

        private AtomType(uint type)
        {
            this.type = type;
        }

        static AtomType()
        {
            Empty = new AtomType(AtomTypes.Empty);
        }

        public static AtomType From(uint type)
        {
            if (AtomTypes.Empty == type)
            {
                return Empty;
            }

            return new AtomType(type);
        }

        public static AtomType From(string alias)
        {
            if (String.IsNullOrEmpty(alias))
            {
                throw new ArgumentException("", nameof(alias));
            }

            if (alias.Length != 4)
            {
                throw new ArgumentException("", nameof(alias));
            }

            var bytes = Encoding.ASCII.GetBytes(alias);

            return new AtomType(BitConverter.ToUInt32(bytes, 0));
        }

        public bool Equals(AtomType other)
        {
            if (null == other)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return type == other.type;
        }

        public override bool Equals(object obj)
        {
            if (null == obj)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj is AtomType other && Equals(other);
        }

        public override int GetHashCode() => type.GetHashCode();

        public override string ToString() => "0x" + type.ToString("X08");

        /// <summary>
        /// 
        /// </summary>
        private class BoxTypeDebug
        {
            private readonly AtomType instance;

            public uint Type => instance.type;

            public string Sign
            {
                get
                {
                    var bytes = BitConverter.GetBytes(instance.type);
                    return Encoding.ASCII.GetString(bytes.ToBigEndian());
                }
            }

            public BoxTypeDebug(AtomType instance)
            {
                this.instance = instance;
            }
        }
    }
}