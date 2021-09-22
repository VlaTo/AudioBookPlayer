using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Android.Support.V4.Media;
using AudioBookPlayer.App.Domain.Models;

namespace AudioBookPlayer.App.Android.Services.Builders
{
    /// <summary>
    /// Audio book builder abstraction.
    /// </summary>
    internal abstract class BookBuilder
    {
        public AudioBookAuthor[] GetAuthors([NotNull] MediaBrowserCompat.MediaItem mediaItem)
        {
            var delimiter = CultureInfo.CurrentUICulture.TextInfo.ListSeparator;
            var temp = mediaItem.Description.Description.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            var authors = new AudioBookAuthor[temp.Length];

            for (var index = 0; index < temp.Length; index++)
            {
                authors[index] = new AudioBookAuthor(temp[index]);
            }

            return authors;
        }

        public void BuildAuthors([NotNull] AudioBook audioBook, [NotNull] MediaBrowserCompat.MediaItem mediaItem)
        {
            var authors = GetAuthors(mediaItem);

            foreach (var author in authors)
            {
                audioBook.Authors.Add(author);
            }
        }
    }

    internal abstract class BookBuilder<TModel> : BookBuilder
    {
        /// <summary>
        /// Constructs new instance of the <see cref="AudioBook" /> from <see cref="MediaBrowserCompat.MediaItem" />.
        /// </summary>
        /// <param name="mediaItem">Source media item.</param>
        /// <returns>The instance of <see cref="AudioBook" /> constructed.</returns>
        [return: NotNull]
        public abstract TModel BuildBookFrom([NotNull] MediaBrowserCompat.MediaItem mediaItem);
    }
}