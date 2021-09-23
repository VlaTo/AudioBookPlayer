using AudioBookPlayer.App.Domain.Models;
using System;
using System.Diagnostics.CodeAnalysis;

namespace AudioBookPlayer.App.Android.Services.Builders
{
    internal readonly struct MediaBookId
    {
        private const string Prefix = "audiobook:";

        public EntityId EntityId { get; }

        public MediaBookId(EntityId entityId)
        {
            EntityId = entityId;
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
            if (false == String.IsNullOrEmpty(s) && s.StartsWith(Prefix))
            {
                var value = s.Substring(Prefix.Length);

                if (EntityId.TryParse(value, out var audioBookId))
                {
                    mediaBookId = new MediaBookId(audioBookId);

                    return true;
                }
            }

            mediaBookId = new MediaBookId(EntityId.Empty);
            
            return false;
        }

        public override string ToString() => Prefix + EntityId;
    }
}