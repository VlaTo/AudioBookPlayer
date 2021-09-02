using System;
using AudioBookPlayer.App.Domain.Models;

namespace AudioBookPlayer.App.Domain.Extensions
{
    public static class AudioBookExtensions
    {
        public static AudioBookSection GetOrCreatePart(this AudioBook audioBook, string title)
        {
            if (null == title)
            {
                return null;
            }

            for (var index = 0; index < audioBook.Sections.Count; index++)
            {
                var part = audioBook.Sections[index];

                if (String.Equals(part.Title, title, StringComparison.InvariantCulture))
                {
                    return part;
                }
            }

            var target = new AudioBookSection(audioBook, title);

            audioBook.Sections.Add(target);

            return target;
        }
    }
}