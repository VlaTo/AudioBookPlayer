using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AudioBookPlayer.App.Domain.Models;
using LibraProgramming.Xamarin.Core.Extensions;

namespace AudioBookPlayer.App.Models
{
    public sealed class StreamAudioBookImage : AudioBookImage
    {
        private readonly ReadOnlyMemory<byte> memory;

        public StreamAudioBookImage(AudioBook audioBook, ReadOnlyMemory<byte> memory)
            : base(audioBook)
        {
            this.memory = memory;
        }

        public override Task<Stream> GetStreamSync(CancellationToken cancellationToken = default)
        {
            var stream = memory.AsStream();
            return Task.FromResult(stream);
        }
    }
}