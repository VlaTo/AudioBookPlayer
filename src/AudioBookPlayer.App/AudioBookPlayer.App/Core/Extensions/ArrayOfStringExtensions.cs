using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace AudioBookPlayer.App.Core.Extensions
{
    internal static class ArrayOfStringExtensions
    {
        [return: NotNull]
        public static string ToCommaString([NotNull] this string[] array)
        {
            var delimiter = CultureInfo.CurrentUICulture.TextInfo.ListSeparator;
            return String.Join(delimiter, array);
        }
    }
}