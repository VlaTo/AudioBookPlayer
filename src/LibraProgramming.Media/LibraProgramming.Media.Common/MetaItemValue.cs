using System;
using System.IO;

namespace LibraProgramming.Media.Common
{
    public class MetaItemValue
    {
        public static TextItemValue FromText(string text)
        {
            return new TextItemValue(text);
        }

        public static StreamItemValue FromStream(Stream stream)
        {
            if (null == stream)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            return new StreamItemValue(stream);
        }

        public static BinaryItemValue FromBinary(byte[] bytes)
        {
            if (null == bytes)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            return new BinaryItemValue(bytes);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class TextItemValue : MetaItemValue
    {
        public string Text
        {
            get;
        }

        internal TextItemValue(string text)
        {
            Text = text;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class StreamItemValue : MetaItemValue
    {
        public Stream Stream
        {
            get;
        }

        internal StreamItemValue(Stream stream)
        {
            Stream = stream;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class BinaryItemValue : MetaItemValue
    {
        public byte[] Bytes
        {
            get;
        }

        internal BinaryItemValue(byte[] bytes)
        {
            Bytes = bytes;
        }
    }
}
