using LibraProgramming.Media.QuickTime.Components;
using LibraProgramming.Media.QuickTime.Extensions;
using System;
using System.Text;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    [Chunk(AtomTypes.Ftyp)]
    internal sealed class MediaTypeChunk : Chunk
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

        public MediaTypeChunk(string brand, uint version, string compatibleBrand, Span<byte> data)
            : base(AtomTypes.Ftyp)
        {
            Brand = brand;
            Version = version;
            CompatibleBrand = compatibleBrand;
        }

        public static MediaTypeChunk ReadFrom(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var brand = StreamHelper.ReadString(atom.Stream, 4);
            var version = StreamHelper.ReadUInt32(atom.Stream);
            var compatibleBrand = StreamHelper.ReadString(atom.Stream, 4);
            var data = StreamHelper.ReadBytes(atom.Stream, 4);

            return new MediaTypeChunk(brand, version, compatibleBrand, data);
        }

        public override void Debug(int level)
        {
            var tabs = new String(' ', level);
            var bytes = BitConverter.GetBytes(Type).ToBigEndian();
            var type = Encoding.ASCII.GetString(bytes);

            Console.WriteLine($"{tabs}{type} brand: '{Brand}'");

            //Console.WriteLine($"[Ftyp] Brand: '{Brand}', Version: '{Version}', Compatible: '{CompatibleBrand}'");
        }
    }
}