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

        public string Key
        {
            get;
        }

        protected AudioBookImage(AudioBook audioBook, string key)
        {
            AudioBook = audioBook;
            Key = key;
        }

        public abstract Task<Stream> GetStreamSync(CancellationToken cancellationToken = default);

        public abstract Task<byte[]> GetBytesAsync(CancellationToken cancellationToken = default);
    }
}