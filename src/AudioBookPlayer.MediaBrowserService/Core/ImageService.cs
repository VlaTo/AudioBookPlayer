using AudioBookPlayer.Core;
using AudioBookPlayer.Domain.Services;
using System.IO;

namespace AudioBookPlayer.MediaBrowserService.Core
{
    internal sealed class ImageService : IImageService
    {
        private readonly ImageContentService contentService;

        public ImageService(ImageContentService contentService)
        {
            this.contentService = contentService;
        }

        public Stream GetImage(string contentUri)
        {
            return contentService.GetImageStream(contentUri);
        }

        public string SaveImage(Stream stream)
        {
            return contentService.SaveImageStream(stream);
        }
    }
}