using System;
using AudioBookPlayer.App.Domain.Models;

namespace AudioBookPlayer.App.Domain.Extensions
{
    public static class AudioBookExtensions
    {
        public static AudioBookPart GetOrCreatePart(this AudioBook audioBook, string title)
        {
            if (null == title)
            {
                return null;
            }

            for (var index = 0; index < audioBook.Parts.Count; index++)
            {
                var part = audioBook.Parts[index];

                if (String.Equals(part.Title, title, StringComparison.InvariantCulture))
                {
                    return part;
                }
            }

            var target = new AudioBookPart(audioBook, title);

            audioBook.Parts.Add(target);

            return target;
        }
    }
}