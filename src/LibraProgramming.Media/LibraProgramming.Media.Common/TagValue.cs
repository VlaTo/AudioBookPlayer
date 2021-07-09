using System;
using System.IO;

namespace LibraProgramming.Media.Common
{
    public class TagValue
    {
        public static TextValue FromText(string text)
        {
            return new TextValue(text);
        }

        public static StreamValue FromStream(Stream stream)
        {
            if (null == stream)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            return new StreamValue(stream);
        }

        public static BinaryValue FromBinary(byte[] bytes)
        {
            if (null == bytes)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            return new BinaryValue(bytes);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class TextValue : TagValue
    {
        public string Text
        {
            get;
        }

        internal TextValue(string text)
        {
            Text = text;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class StreamValue : TagValue
    {
        public Stream Stream
        {
            get;
        }

        internal StreamValue(Stream stream)
        {
            Stream = stream;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class BinaryValue : TagValue
    {
        public byte[] Bytes
        {
            get;
        }

        internal BinaryValue(byte[] bytes)
        {
            Bytes = bytes;
        }
    }
}
