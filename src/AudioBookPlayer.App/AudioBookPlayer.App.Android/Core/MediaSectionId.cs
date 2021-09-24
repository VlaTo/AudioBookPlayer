using System;
using System.Diagnostics.CodeAnalysis;
using AudioBookPlayer.App.Domain.Models;

namespace AudioBookPlayer.App.Android.Core
{
    internal readonly struct MediaSectionId
    {
        private const string Prefix = "section:";
        private const char Delimiter = '/';

        public static readonly MediaSectionId Empty;

        public EntityId EntityId
        {
            get;
        }

        public int Index
        {
            get;
        }

        public MediaSectionId(EntityId entityId, int index)
        {
            EntityId = entityId;
            Index = index;
        }

        static MediaSectionId()
        {
            Empty = new MediaSectionId(EntityId.Empty, -1);
        }

        [return: NotNull]
        public static MediaSectionId Parse([NotNull] string s)
        {
            if (TryParse(s, out var mediaSectionId))
            {
                ;
            }

            return mediaSectionId;
        }

        public static bool TryParse([NotNull] string s, out MediaSectionId value)
        {
            if (false == String.IsNullOrEmpty(s))
            {
                var position = s.IndexOf(Delimiter);

                if (-1 < position)
                {
                    var start = s.Substring(0, position);
                    var tail = s.Substring(position + 1);

                    if (MediaBookId.TryParse(start, out var mediaBookId) && tail.StartsWith(Prefix))
                    {
                        var number = tail.Substring(Prefix.Length);

                        if (int.TryParse(number, out var index))
                        {
                            value = new MediaSectionId(mediaBookId.EntityId, index);

                            return true;
                        }
                    }
                }
            }

            value = Empty;

            return false;
        }

        public override string ToString() => Prefix + EntityId;
    }
}