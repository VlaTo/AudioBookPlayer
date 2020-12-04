using LibraProgramming.Media.QuickTime.Components;
using System;

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
        public static FtypChunk ReadFrom(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var brand = StreamHelper.ReadString(atom.Stream, 4);
            var version = StreamHelper.ReadUInt32(atom.Stream);
            var compatibleBrand = StreamHelper.ReadString(atom.Stream, 4);
            var data = StreamHelper.ReadBytes(atom.Stream, 4);

            return new FtypChunk(brand, version, compatibleBrand, data);
        }

        /*public override void Debug(int level)
        {
            var tabs = new String(' ', level);
            var bytes = BitConverter.GetBytes(Type).ToBigEndian();
            var type = Encoding.ASCII.GetString(bytes);

            Console.WriteLine($"{tabs}{type} brand: '{Brand}'");

            //Console.WriteLine($"[Ftyp] Brand: '{Brand}', Version: '{Version}', Compatible: '{CompatibleBrand}'");
        }*/
    }
}