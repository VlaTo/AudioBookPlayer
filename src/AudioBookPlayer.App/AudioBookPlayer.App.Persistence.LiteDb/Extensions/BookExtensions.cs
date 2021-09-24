using System.Linq;
using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Domain.Providers;
using AudioBookPlayer.App.Persistence.LiteDb.Models;

namespace AudioBookPlayer.App.Persistence.LiteDb.Extensions
{
    public static class BookExtensions
    {
        /*public static AudioBook ToAudioBook(this Book book, ICoverProvider coverProvider)
        {
            var audioBook = new AudioBook(book.Id, book.Title)
            {
                Synopsis = book.Synopsis,
                Created = book.Created,
                Duration = book.Duration
            };

            audioBook.Authors.AddRange(
                book.Authors.Select(author => new AudioBookAuthor(author))
            );

            audioBook.Sections.AddRange(
                book.Sections.Select(section => new AudioBookSection(audioBook)
                {
                    Name = section.Title
                })
            );

            audioBook.Images.AddRange(
                book.Images.Select(contentUri => new ContentProvidedAudioBookImage(audioBook, contentUri, coverProvider))
            );

            return audioBook;
        }*/
    }
}