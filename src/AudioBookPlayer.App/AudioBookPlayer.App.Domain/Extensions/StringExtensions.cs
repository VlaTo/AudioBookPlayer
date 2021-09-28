using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace AudioBookPlayer.App.Domain.Extensions
{
    public static class StringExtensions
    {
        [return: NotNull]
        public static string AsString([NotNull] this string[] authors)
        {
            var separator = CultureInfo.CurrentUICulture.TextInfo.ListSeparator;
            return String.Join(separator, authors);
        }
    }
}