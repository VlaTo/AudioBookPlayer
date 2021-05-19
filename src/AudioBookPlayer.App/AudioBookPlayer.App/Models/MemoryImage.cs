﻿using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AudioBookPlayer.App.Models
{
    public sealed class MemoryImage : BookImage
    {
        private readonly byte[] bytes;

        public MemoryImage(string tag, byte[] bytes)
            : base(tag)
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