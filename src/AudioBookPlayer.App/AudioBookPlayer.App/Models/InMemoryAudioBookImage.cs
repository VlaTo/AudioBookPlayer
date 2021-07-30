using AudioBookPlayer.App.Domain.Models;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LibraProgramming.Xamarin.Core.Extensions;

namespace AudioBookPlayer.App.Models
{
    public sealed class InMemoryAudioBookImage : AudioBookImage
    {
        private readonly ReadOnlyMemory<byte> memory;

        public InMemoryAudioBookImage(AudioBook audioBook, string key, ReadOnlyMemory<byte> memory)
            : base(audioBook, key)
        {
            this.memory = memory;
        }

        public override Task<Stream> GetStreamSync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(memory.AsStream());
        }
    }
}