using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using AudioBookPlayer.App.Domain.Models;

namespace AudioBookPlayer.App.Domain.Extensions
{
    public static class AudioBookAuthorExtensions
    {
        [return: NotNull]
        public static string AsString([NotNull] this IList<AudioBookAuthor> authors)
        {
            var delimiter = CultureInfo.CurrentUICulture.TextInfo.ListSeparator;
            var builder = new StringBuilder();

            for (var index = 0; index < authors.Count; index++)
            {
                var author = authors[index];

                if (String.IsNullOrEmpty(author.Name))
                {
                    continue;
                }

                if (0 < builder.Length)
                {
                    builder.Append(delimiter);
                }

                builder.Append(author.Name);
            }

            return builder.ToString();
        }
    }
}