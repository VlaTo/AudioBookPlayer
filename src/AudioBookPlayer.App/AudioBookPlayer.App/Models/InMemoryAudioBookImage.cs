using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AudioBookPlayer.App.Models
{
    public sealed class InMemoryAudioBookImage : AudioBookImage
    {
        private readonly byte[] bytes;

        public InMemoryAudioBookImage(AudioBook audioBook, string key, byte[] bytes)
            : base(audioBook, key)
        {
            this.bytes = bytes;
        }

        public override Task<Stream> GetStreamSync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<Stream>(new MemoryStream(bytes));
        }

        public override Task<byte[]> GetBytesAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(bytes);
    }
}