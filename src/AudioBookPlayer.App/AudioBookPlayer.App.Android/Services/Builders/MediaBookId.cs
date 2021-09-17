using AudioBookPlayer.App.Domain.Models;
using System;
using System.Diagnostics.CodeAnalysis;

namespace AudioBookPlayer.App.Android.Services.Builders
{
    internal readonly struct MediaBookId
    {
        private const string Prefix = "audiobook:";

        public BookId Id { get; }

        public MediaBookId(BookId bookId)
        {
            Id = bookId;
        }

        [return: NotNull]
        public static MediaBookId Parse([NotNull] string s)
        {
            if (TryParse(s, out var mediaBookId))
            {
                ;
            }

            return mediaBookId;
        }

        public static bool TryParse([NotNull] string s, out MediaBookId mediaBookId)
        {
            if (false == String.IsNullOrEmpty(s))
            {
                var position = s.IndexOf(Prefix[^1]);

                if (Prefix.Length == position)
                {
                    var value = s.Substring(position);

                    if (BookId.TryParse(value, out var audioBookId))
                    {
                        mediaBookId = new MediaBookId(audioBookId);

                        return true;
                    }
                }
            }

            mediaBookId = new MediaBookId(BookId.Empty);
            
            return false;
        }

        public override string ToString() => Prefix + Id;
    }
}