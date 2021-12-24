using LibraProgramming.Media.QuickTime.Components;
using System;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    /// <summary>
    /// The sound media info header atom.
    /// </summary>
    [Chunk(AtomTypes.Smhd)]
    internal sealed class SmhdChunk : Chunk
    {
        float Balance
        {
            get;
        }

        private SmhdChunk(float balance)
            : base(AtomTypes.Smhd)
        {
            Balance = balance;
        }

        [ChunkCreator]
        public static SmhdChunk ReadFrom(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var (version, flags) = ReadFlagsAndVersion(atom.Stream);
            //balnce scale is left = negatives ; normal = 0.0 ; right = positives
            var audioBalance = StreamHelper.ReadFixedPoint(atom.Stream, 2);
            var reserved0 = StreamHelper.ReadBytes(atom.Stream, 2);

            return new SmhdChunk(audioBalance);
        }
    }
}