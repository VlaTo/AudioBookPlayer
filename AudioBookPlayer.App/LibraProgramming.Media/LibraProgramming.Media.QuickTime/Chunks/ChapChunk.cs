using LibraProgramming.Media.QuickTime.Components;
using System;
using System.Threading.Tasks;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    [Chunk(AtomTypes.Chap)]
    internal sealed class ChapChunk : Chunk
    {
        public uint[] Scenes
        {
            get;
        }

        public ChapChunk(uint[] scenes)
            : base(AtomTypes.Chap)
        {
            Scenes = scenes ?? Array.Empty<uint>();
        }

        [ChunkCreator]
        public static async Task<ChapChunk> ReadFromAsync(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var numberOfEntries = atom.Stream.Length / 4;
            var scenes = new uint[numberOfEntries];

            using (var source = new ReadOnlyAtomStream(atom.Stream, 0, atom.Stream.Length))
            {
                for (var index = 0; index < numberOfEntries; index++)
                {
                    var blockSize = await StreamHelper.ReadUInt32Async(atom.Stream);
                    scenes[index] = blockSize;
                }
            }

            return new ChapChunk(scenes);
        }
    }
}
