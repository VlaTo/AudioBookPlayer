using System;
using System.IO;
using AudioBookPlayer.Domain.Models;
using MemoryStream = LibraProgramming.Memory.MemoryStream;

namespace AudioBookPlayer.MediaBrowserService.Core
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
            return MemoryStream.Create(memory, true);
        }
    }
}