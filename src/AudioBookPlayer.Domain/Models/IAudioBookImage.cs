using System.IO;

namespace AudioBookPlayer.Domain.Models
{
    public interface IAudioBookImage
    {
        AudioBook AudioBook
        {
            get;
        }

        Stream GetImageStream();
    }
}