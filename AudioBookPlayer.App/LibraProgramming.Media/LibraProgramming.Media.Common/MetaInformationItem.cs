using System;
using System.IO;

namespace LibraProgramming.Media.Common
{
    public class MetaInformationItem
    {
        public string Key
        {
            get;
        }

        internal MetaInformationItem(string key)
        {
            Key = key;
        }

        public static MetaInformationTextItem FromText(string key, string text)
        {
            if (null == key)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (0 == key.Length)
            {
                throw new ArgumentException(nameof(key));
            }

            return new MetaInformationTextItem(key, text);
        }

        public static MetaInformationStreamItem FromStream(string key, Stream stream)
        {
            if (null == key)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (0 == key.Length)
            {
                throw new ArgumentException(nameof(key));
            }

            if (null == stream)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            return new MetaInformationStreamItem(key, stream);
        }

        public static MetaInformationBinaryItem FromBinary(string key, byte[] bytes)
        {
            if (null == key)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (0 == key.Length)
            {
                throw new ArgumentException(nameof(key));
            }

            if (null == bytes)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            return new MetaInformationBinaryItem(key, bytes);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class MetaInformationTextItem : MetaInformationItem
    {
        public string Text
        {
            get;
        }

        internal MetaInformationTextItem(string key, string text)
            : base(key)
        {
            Text = text;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class MetaInformationStreamItem : MetaInformationItem
    {
        public Stream Stream
        {
            get;
        }

        internal MetaInformationStreamItem(string key, Stream stream)
            : base(key)
        {
            Stream = stream;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class MetaInformationBinaryItem : MetaInformationItem
    {
        public byte[] Bytes
        {
            get;
        }

        internal MetaInformationBinaryItem(string key, byte[] bytes)
            : base(key)
        {
            Bytes = bytes;
        }
    }
}
