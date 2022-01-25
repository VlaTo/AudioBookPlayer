using AudioBookPlayer.Domain.Services;
using System.IO;

namespace AudioBookPlayer.Domain.Models
{
    public sealed class ContentProviderImage : IAudioBookImage, IHasContentUri
    {
        private readonly IImageService imageService;
        private readonly string sourceFile;

        public AudioBook AudioBook
        {
            get;
        }

        public string ContentUri => sourceFile;

        public ContentProviderImage(AudioBook audioBook, IImageService imageService, string sourceFile)
        {
            AudioBook = audioBook;
            this.imageService = imageService;
            this.sourceFile = sourceFile;
        }

        public Stream GetImageStream() => imageService.GetImage(sourceFile);
    }
}