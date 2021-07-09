using LibraProgramming.Media.Common;
using System;

namespace LibraProgramming.Media.QuickTime
{
    public sealed class QuickTimeMediaTrack : IMediaTrack
    {
        // private readonly QuickTimeMediaExtractor extractor;

        public string Title
        {
            get;
            internal set;
        }

        public TimeSpan Duration
        {
            get;
            internal set;
        }

        internal uint[] Samples
        {
            get;
            set;
        }

        internal QuickTimeMediaTrack(/*QuickTimeMediaExtractor extractor*/)
        {
            // this.extractor = extractor;
        }

        /*public Stream GetMediaStream()
        {
            var source = extractor.GetStream();
            var memory = new MemoryStream();

            source.Seek(168L, SeekOrigin.Begin);

            for (var index = 0; index < Samples.Length; index++)
            {
                var count = Samples[index];
                var buffer = new byte[count];
                var readed = source.Read(buffer, 0, (int)count);
                memory.Write(buffer, 0, readed);
            }

            return new ReadOnlyAtomStream(memory, 0, memory.Length);
        }*/
    }
}
