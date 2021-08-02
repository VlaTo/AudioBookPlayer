using LibraProgramming.Media.QuickTime.Components;
using System;
using System.IO;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    internal sealed class TimeToSample
    {
        public uint SampleCount
        {
            get;
        }

        public uint Duration
        {
            get;
        }

        public TimeToSample(uint sampleCount, uint duration)
        {
            SampleCount = sampleCount;
            Duration = duration;
        }

        public static TimeToSample ReadFromStream(Stream stream)
        {
            var frameCount = StreamHelper.ReadUInt32(stream);
            var duration = StreamHelper.ReadUInt32(stream);

            return new TimeToSample(frameCount, duration);
        }
    }

    /// <summary>
    /// The time-to-sample atom.
    /// </summary>
    [Chunk(AtomTypes.Stts)]
    internal sealed class SttsChunk : Chunk
    {
        public TimeToSample[] Entries
        {
            get;
        }

        public SttsChunk(TimeToSample[] entries)
            : base(AtomTypes.Stts)
        {
            Entries = entries ?? Array.Empty<TimeToSample>();
        }

        [ChunkCreator]
        public static SttsChunk ReadFrom(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var (_, _) = ReadFlagsAndVersion(atom.Stream);
            var numberOfTimes = StreamHelper.ReadUInt32(atom.Stream);
            
            var position = atom.Stream.Position;
            var entries = new TimeToSample[numberOfTimes];

            using (var source = new ReadOnlyAtomStream(atom.Stream, 0, atom.Stream.Length - position))
            {
                using (var stream = new BufferedStream(source, 10240))
                {
                    for (var index = 0; index < numberOfTimes; index++)
                    {
                        var entry = TimeToSample.ReadFromStream(stream);
                        entries[index] = entry;
                    }
                }
            }

            return new SttsChunk(entries);
        }
    }
}
