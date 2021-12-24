using System.Collections.Generic;

namespace AudioBookPlayer.Domain.Models
{
    public sealed class AudioBookPart
    {
        public IReadOnlyList<AudioBookChapter> Chapters
        {
            get;
        }
    }
}