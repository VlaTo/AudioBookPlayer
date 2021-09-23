using System;
using AudioBookPlayer.App.Persistence.LiteDb.Models;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace AudioBookPlayer.App.Android.Services.Builders
{
    internal abstract class MediaItemBuilder
    {
        [return: MaybeNull]
        public static global::Android.Net.Uri GetBookImageUri([NotNull] Book book, int imageIndex)
        {
            if (book.Images.Length > imageIndex)
            {
                return global::Android.Net.Uri.Parse(book.Images[imageIndex]);
            }

            return null;
        }
        
        [return: NotNull]
        public static string GetBookAuthors([NotNull] Book book)
        {
            return String.Join(CultureInfo.CurrentUICulture.TextInfo.ListSeparator, book.Authors);
        }
    }
}