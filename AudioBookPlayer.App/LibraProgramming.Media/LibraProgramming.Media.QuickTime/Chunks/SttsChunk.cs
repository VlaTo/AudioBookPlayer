using LibraProgramming.Media.QuickTime.Components;
using System;
using System.IO;
using System.Threading.Tasks;

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

        [ChunkCreator]
        public static async Task<TimeToSample> ReadFromStreamAsync(Stream stream)
        {
            var frameCount = await StreamHelper.ReadUInt32Async(stream);
            var duration = await StreamHelper.ReadUInt32Async(stream);

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
        public static async Task<Chunk> ReadFromAsync(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var (_, _) = await ReadFlagsAndVersionAsync(atom.Stream);
            var numberOfTimes = await StreamHelper.ReadUInt32Async(atom.Stream);
            var entries = new TimeToSample[numberOfTimes];
            var position = atom.Stream.Position;

            using (var stream = new ReadOnlyAtomStream(atom.Stream, 0, atom.Stream.Length - position))
            {
                for (var index = 0; index < numberOfTimes; index++)
                {
                    var entry = await TimeToSample.ReadFromStreamAsync(atom.Stream);
                    entries[index] = entry;
                }
            }

            return new SttsChunk(entries);
        }
    }
}
