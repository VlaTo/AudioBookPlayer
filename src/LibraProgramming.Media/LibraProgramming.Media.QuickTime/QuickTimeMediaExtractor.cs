using LibraProgramming.Media.Common;
using LibraProgramming.Media.QuickTime.Chunks;
using LibraProgramming.Media.QuickTime.Visitors;
using System;
using System.Collections.Generic;
using System.IO;

namespace LibraProgramming.Media.QuickTime
{
    public sealed class QuickTimeMediaExtractor : MediaExtractor
    {
        private Stream stream;
        private RootChunk root;
        private bool disposed;

        private QuickTimeMediaExtractor(Stream stream, RootChunk root)
        {
            this.stream = stream;
            this.root = root;
        }

        public static QuickTimeMediaExtractor CreateFrom(Stream stream)
        {
            if (null == stream)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (false == stream.CanRead)
            {
                throw new InvalidOperationException("");
            }

            if (0L != stream.Seek(0L, SeekOrigin.Begin))
            {
                throw new Exception();
            }

            var chunks = new List<Chunk>();

            using (var temp = new ReadOnlySharedStream(stream))
            {
                var extractor = new AtomExtractor(temp);

                foreach (var atom in extractor)
                {
                    var chunk = ChunkFactory.Instance.CreateFrom(atom);

                    chunks.Add(chunk);
                }
            }

            var root = new RootChunk(chunks.ToArray());

            return new QuickTimeMediaExtractor(stream, root);
        }

        public void Debug()
        {
            EnsureNotDisposed();

            root.Debug(0);
        }

        public override IReadOnlyCollection<IMediaTrack> GetTracks()
        {
            EnsureNotDisposed();

            var tracks = new List<IMediaTrack>();
            var visitor = new MediaTrackVisitor(this, stream, tracks);

            visitor.Visit(root);

            return tracks.AsReadOnly();
        }

        public override MetaInformation GetMeta()
        {
            EnsureNotDisposed();

            var information = new MetaInformation();
            var visitor = new MetaInformationVisitor(information);

            visitor.Visit(root);

            return information;
        }

        internal Stream GetStream() => stream;
    }
}
