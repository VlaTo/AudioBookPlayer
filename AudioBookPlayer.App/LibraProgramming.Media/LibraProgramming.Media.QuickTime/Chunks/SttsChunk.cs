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

        public static SttsChunk ReadFrom(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var (version, flags) = ReadFlagAndVersion(atom.Stream);
            var numberOfTimes = StreamHelper.ReadUInt32(atom.Stream);
            var entries = new TimeToSample[numberOfTimes];
            var position = atom.Stream.Position;

            using (var stream = new ReadOnlyAtomStream(atom.Stream, 0, atom.Stream.Length - position))
            {
                for (var index = 0; index < numberOfTimes; index++)
                {
                    var entry = TimeToSample.ReadFromStream(atom.Stream);
                    entries[index] = entry;
                }
            }

            return new SttsChunk(entries);
        }
    }
}
