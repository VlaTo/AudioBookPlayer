using LibraProgramming.Media.QuickTime.Components;
using LibraProgramming.Media.QuickTime.Extensions;
using System;
using System.Text;

namespace LibraProgramming.Media.QuickTime.Lists
{
    public enum DataType
    {
        Unknown = -1,
        Binary = 0,
        Text = 1,
    }

    [Chunk(AtomTypes.Data)]
    public class DataChunk : Chunk
    {
        private readonly Memory<byte> memory;

        public DataType DataType
        {
            get;
        }

        public ReadOnlyMemory<byte> Data => memory;

        public string Text
        {
            get
            {
                if (DataType.Text != DataType)
                {
                    throw new Exception();
                }

                return Encoding.UTF8.GetString(memory.Span);
            }
        }

        public DataChunk(DataType dataType, byte[] data)
            : base(AtomTypes.Data)
        {
            memory = new Memory<byte>(data);
            DataType = dataType;
        }
        
        [ChunkCreator]
        public static DataChunk ReadFrom(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var prefix = StreamHelper.ReadBytes(atom.Stream, 4);
            var type = Lists.DataType.Unknown;

            if (0 == prefix[0])
            {
                StreamHelper.ToLittleEndian(prefix, 0, prefix.Length);
                var valueType = BitConverter.ToUInt32(prefix, 0);
                type = GetWellKnownType(valueType);
            }
            /*else
            {
                var type = StreamHelper.ReadUInt32(atom.Stream);
            }*/

            var locale = StreamHelper.ReadUInt32(atom.Stream);
            var offset = atom.Stream.Position;
            var length = (int) (atom.Stream.Length - offset);
            Span<byte> bytes;

            using (var stream = new ReadOnlyAtomStream(atom.Stream, offset, length))
            {
                bytes = StreamHelper.ReadBytes(stream, (uint) stream.Length);
            }

            return new DataChunk(type, bytes.ToArray());
        }

        public override void Debug(int level)
        {
            var tabs = new String(' ', level);
            var bytes = BitConverter.GetBytes(Type);
            var type = Encoding.ASCII.GetString(bytes.ToBigEndian());

            Console.WriteLine($"{tabs}{type}");
        }

        private static DataType GetWellKnownType(uint type)
        {
            switch (type)
            {
                case 0x00:
                case 0x15:
                {
                    return DataType.Binary;
                }

                case 0x01: // 1 - UTF8 w\o any count or NULL terminator
                case 0x12:
                {
                    return DataType.Text;
                }

                case 0x0D: // 13 - JPEG data
                {
                    return DataType.Binary;
                }

                case 0x0E: // 14 - PNG data
                {
                    return DataType.Binary;
                }

                default:
                {
                    return DataType.Unknown;
                }
            }
        }
    }
}