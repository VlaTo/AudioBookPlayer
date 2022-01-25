using System;
using System.IO;

namespace AudioBookPlayer.Domain.Models
{
    public sealed class InMemoryImage : IAudioBookImage
    {
        private readonly ReadOnlyMemory<byte> memory;

        public AudioBook AudioBook
        {
            get;
        }

        public InMemoryImage(AudioBook audioBook, ReadOnlyMemory<byte> memory)
        {
            this.memory = memory;
            AudioBook = audioBook;
        }

        public Stream GetImageStream()
        {
            var bytes = memory.ToArray();
            return new MemoryStream(bytes);
        }
    }
}