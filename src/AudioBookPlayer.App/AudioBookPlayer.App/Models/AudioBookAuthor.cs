using System;

namespace AudioBookPlayer.App.Models
{
    public sealed class AudioBookAuthor : IEquatable<AudioBookAuthor>
    {
        public string Name
        {
            get;
        }

        public AudioBookAuthor(string name)
        {
            Name = name;
        }

        public bool Equals(AudioBookAuthor other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return String.Equals(Name, other.Name);
        }

        public override bool Equals(object obj) =>
            ReferenceEquals(this, obj) || obj is AudioBookAuthor other && Equals(other);

        public override int GetHashCode() =>
            (Name != null ? Name.GetHashCode() : 0);
    }
}