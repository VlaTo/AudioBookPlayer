using LibraProgramming.Media.QuickTime.Components;
using LibraProgramming.Media.QuickTime.Extensions;
using System;
using System.Text;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    internal sealed class FrameDescription
    {
        public uint FrameCount
        {
            get;
        }

        public uint Duration
        {
            get;
        }

        public FrameDescription(uint frameCount, uint duration)
        {
            FrameCount = frameCount;
            Duration = duration;
        }
    }

    /// <summary>
    /// The time-to-sample atom.
    /// </summary>
    [Chunk(AtomTypes.Stts)]
    internal sealed class SttsChunk : Chunk
    {
        public FrameDescription[] FrameDescriptions
        {
            get;
        }

        public SttsChunk(FrameDescription[] frameDescriptions)
            : base(AtomTypes.Stts)
        {
            FrameDescriptions = frameDescriptions ?? Array.Empty<FrameDescription>();
        }

        public static SttsChunk ReadFrom(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var (version, flags) = ReadFlagAndVersion(atom.Stream);
            var numberOfTimes = StreamHelper.ReadUInt32(atom.Stream);
            var frameDescriptions = new FrameDescription[numberOfTimes];
            var position = atom.Stream.Position;

            using (var stream = new ReadOnlyAtomStream(atom.Stream, 0, atom.Stream.Length - position))
            {
                for (var index = 0; index < numberOfTimes; index++)
                {
                    var frameCount = StreamHelper.ReadUInt32(atom.Stream);
                    var duration = StreamHelper.ReadUInt32(atom.Stream);

                    frameDescriptions[index] = new FrameDescription(frameCount, duration);
                }
            }

            return new SttsChunk(frameDescriptions);
        }

        /*public override void Debug(int level)
        {
            var tabs = new String(' ', level);
            var bytes = BitConverter.GetBytes(Type).ToBigEndian();
            var type = Encoding.ASCII.GetString(bytes);

            Console.WriteLine($"{tabs}{type} (descriptions: {FrameDescriptions.Length})");

            var count = FrameDescriptions.Length;

            Console.WriteLine($"{tabs} index   frames  duration");

            for (var index = 0; index < Math.Min(3, count); index++)
            {
                var description = FrameDescriptions[index];
                Console.WriteLine($"{tabs}[{index:d6}] {description.FrameCount:d6} {description.Duration:d8}");
            }

            if (3 < count)
            {
                var description = FrameDescriptions[count - 1];
                Console.WriteLine($"{tabs}...");
                Console.WriteLine($"{tabs}[{(count-1):d6}] {description.FrameCount:d6} {description.Duration:d8}");
            }

            Console.WriteLine();
        }*/
    }
}
