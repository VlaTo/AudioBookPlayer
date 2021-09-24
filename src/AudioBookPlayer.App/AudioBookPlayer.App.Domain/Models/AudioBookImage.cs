using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AudioBookPlayer.App.Domain.Models
{
    public abstract class AudioBookImage
    {
        public AudioBook AudioBook
        {
            get;
        }

        protected AudioBookImage(AudioBook audioBook)
        {
            AudioBook = audioBook;
        }

        public abstract Task<Stream> GetStreamAsync(CancellationToken cancellationToken = default);

        public abstract Stream GetStream();
    }
}