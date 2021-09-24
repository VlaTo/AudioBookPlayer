using AudioBookPlayer.App.Domain.Core;
using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Domain.Services;
using AudioBookPlayer.App.Persistence.LiteDb.Models;

namespace AudioBookPlayer.App.Persistence.LiteDb.Extensions
{
    public static class AudioBookExtensions
    {
        /*public static Book ToBook(this AudioBook audioBook, ICoverService coverService)
        {
            var book = new Book
            {
                Id = (long)audioBook.Id,
                Title = audioBook.Title,
                Synopsis = audioBook.Synopsis,
                Duration = audioBook.Duration,
                Created = audioBook.Created,
                Images = new string[audioBook.Images.Count]
            };

            for (var index = 0; index < audioBook.Images.Count; index++)
            {
                var image = audioBook.Images[index];
                string contentUri;

                if (image is IHasContentUri hcu)
                {
                    contentUri = hcu.ContentUri;
                }
                else
                {
                    contentUri = coverService.AddImage(image.GetStream());
                }

                book.Images[index] = contentUri;
            }

            return book;
        }*/
    }
}