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
        public TimeToSample[] TimeToSamples
        {
            get;
        }

        public SttsChunk(TimeToSample[] timeToSamples)
            : base(AtomTypes.Stts)
        {
            TimeToSamples = timeToSamples ?? Array.Empty<TimeToSample>();
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
            var timeToSamples = new TimeToSample[numberOfTimes];

            var start = atom.Stream.Position;
            var length = atom.Stream.Length - start;

            using (var source = new ReadOnlyAtomStream(atom.Stream, start, length))
            {
                for (var index = 0; index < numberOfTimes; index++)
                {
                    var timeToSample = TimeToSample.ReadFromStream(source);
                    timeToSamples[index] = timeToSample;
                }
            }

            return new SttsChunk(timeToSamples);
        }
    }
}
