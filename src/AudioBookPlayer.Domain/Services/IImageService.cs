using System.IO;

namespace AudioBookPlayer.Domain.Services
{
    public interface IImageService
    {
        Stream GetImage(string contentUri);

        string SaveImage(Stream stream);
    }
}