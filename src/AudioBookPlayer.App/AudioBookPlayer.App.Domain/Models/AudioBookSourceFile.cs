using System;

namespace AudioBookPlayer.App.Domain.Models
{
    /// <summary>
    /// Immutable source file description class.
    /// </summary>
    public sealed class AudioBookSourceFile : IEquatable<AudioBookSourceFile>
    {
        public AudioBook AudioBook
        {
            get;
        }

        public string ContentUri
        {
            get;
        }

        public AudioBookSourceFile(AudioBook audioBook, string contentUri)
        {
            AudioBook = audioBook;
            ContentUri = contentUri;
        }

        public bool Equals(AudioBookSourceFile other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(AudioBook, other.AudioBook) && ContentUri == other.ContentUri;
        }

        public override bool Equals(object obj) =>
            ReferenceEquals(this, obj) || obj is AudioBookSourceFile other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(AudioBook, ContentUri);

        /*public static bool operator ==(AudioBookSourceFile first, AudioBookSourceFile second) =>
            !ReferenceEquals(first, null) && first.Equals(second);

        public static bool operator !=(AudioBookSourceFile first, AudioBookSourceFile second) =>
            ReferenceEquals(first, null) || !first.Equals(second);*/
    }
}