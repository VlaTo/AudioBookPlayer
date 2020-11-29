using LibraProgramming.Media.QuickTime.Components;
using System;
using System.Threading.Tasks;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    [Chunk(AtomTypes.Ftyp)]
    internal sealed class FtypChunk : Chunk
    {
        public string Brand
        {
            get;
        }

        public uint Version
        {
            get;
        }

        public string CompatibleBrand
        {
            get;
        }

        public FtypChunk(string brand, uint version, string compatibleBrand, Span<byte> data)
            : base(AtomTypes.Ftyp)
        {
            Brand = brand;
            Version = version;
            CompatibleBrand = compatibleBrand;
        }

        [ChunkCreator]
        public static async Task<Chunk> ReadFromAsync(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var brand = await StreamHelper.ReadStringAsync(atom.Stream, 4);
            var version = await StreamHelper.ReadUInt32Async(atom.Stream);
            var compatibleBrand = await StreamHelper.ReadStringAsync(atom.Stream, 4);
            var data = await StreamHelper.ReadBytesAsync(atom.Stream, 4);

            return new FtypChunk(brand, version, compatibleBrand, data);
        }
    }
}