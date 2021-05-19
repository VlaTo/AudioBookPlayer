using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AudioBookPlayer.App.Models
{
    public abstract class BookImage
    {
        public string Key
        {
            get;
        }

        protected BookImage(string key)
        {
            Key = key;
        }

        public abstract Task<Stream> GetStreamSync(CancellationToken cancellationToken = default);

        public abstract Task<byte[]> GetBytesAsync(CancellationToken cancellationToken = default);
    }
}