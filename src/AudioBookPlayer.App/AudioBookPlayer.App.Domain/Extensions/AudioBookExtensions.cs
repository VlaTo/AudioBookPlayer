﻿using AudioBookPlayer.App.Domain.Models;
using System;
using System.Diagnostics.CodeAnalysis;

namespace AudioBookPlayer.App.Domain.Extensions
{
    public static class AudioBookExtensions
    {
        [return: NotNull]
        public static AudioBookSection GetOrCreatePart(this AudioBook audioBook, [NotNull] string name, [NotNull] string contentUri)
        {
            if (null == name)
            {
                return null;
            }

            for (var index = 0; index < audioBook.Sections.Count; index++)
            {
                var section = audioBook.Sections[index];

                if (String.Equals(section.Name, name, StringComparison.InvariantCulture))
                {
                    return section;
                }
            }

            var target = new AudioBookSection(audioBook)
            {
                Name = name,
                ContentUri = contentUri
            };

            audioBook.Sections.Add(target);

            return target;
        }
    }
}