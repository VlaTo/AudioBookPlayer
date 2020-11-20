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

            var bits = StreamHelper.ReadUInt32(atom.Stream);
            var numberOfTimes = StreamHelper.ReadUInt32(atom.Stream);

            var version = (byte)((bits & 0xFF00_0000) >> 24);
            var flags = bits & 0x00FF_FFFF;

            var frameDescriptions = new FrameDescription[numberOfTimes];

            for (var index = 0; index < numberOfTimes; index++)
            {
                var frameCount = StreamHelper.ReadUInt32(atom.Stream);
                var duration = StreamHelper.ReadUInt32(atom.Stream);

                frameDescriptions[index] = new FrameDescription(frameCount, duration);
            }

            return new SttsChunk(frameDescriptions);
        }

        public override void Debug(int level)
        {
            var tabs = new String(' ', level);
            var bytes = BitConverter.GetBytes(Type).ToBigEndian();
            var type = Encoding.ASCII.GetString(bytes);

            Console.WriteLine($"{tabs}{type} frame descriptions: {FrameDescriptions.Length}");
        }
    }
}
