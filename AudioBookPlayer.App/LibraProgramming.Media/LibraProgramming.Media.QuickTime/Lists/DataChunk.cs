﻿using LibraProgramming.Media.QuickTime.Components;
using LibraProgramming.Media.QuickTime.Extensions;
using System;
using System.Text;

namespace LibraProgramming.Media.QuickTime.Lists
{
    public enum DataType
    {
        Binary = 0,
        Text = 1
    }

    [Chunk(AtomTypes.Data)]
    public class DataChunk : Chunk
    {
        private readonly Memory<byte> memory;

        public DataType DataType
        {
            get;
        }

        public byte[] Data => memory.ToArray();

        public string Text
        {
            get
            {
                if (DataType.Text != DataType)
                {
                    throw new Exception();
                }

                return Encoding.UTF8.GetString(Data);
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

            var type = StreamHelper.ReadUInt32(atom.Stream);
            var reserved = StreamHelper.ReadUInt32(atom.Stream);

            var offset = atom.Stream.Position;
            var length = (int) (atom.Stream.Length - offset);
            Span<byte> bytes;

            using (var stream = new ReadOnlyAtomStream(atom.Stream, offset, length))
            {
                bytes = StreamHelper.ReadBytes(stream, (uint) stream.Length);
            }

            return new DataChunk(GetDataType(type), bytes.ToArray());
        }

        public override void Debug(int level)
        {
            var tabs = new String(' ', level);
            var bytes = BitConverter.GetBytes(Type);
            var type = Encoding.ASCII.GetString(bytes.ToBigEndian());

            Console.WriteLine($"{tabs}{type}");
        }

        private static DataType GetDataType(uint type)
        {
            switch (type)
            {
                case 0:
                case 0x15:
                {
                        return DataType.Binary;
                }

                case 1:
                case 0x12:
                {
                    return DataType.Text;
                }

                default:
                {
                    throw new NotSupportedException();
                }
            }
        }
    }
}