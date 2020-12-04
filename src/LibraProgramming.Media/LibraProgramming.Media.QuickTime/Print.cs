using System;
using System.Diagnostics;
using System.Text;

namespace LibraProgramming.QuickTime.Container
{
    public static class Print
    {
        public static void WriteDump(Span<byte> span, string title = null)
        {
            WriteDump(span.ToArray(), title);
        }

        public static void WriteDump(byte[] bytes, string title = null)
        {
            const int l = 16;
            var length = bytes.Length;

            if (false == String.IsNullOrEmpty(title))
            {
                Debug.WriteLine(title);
            }

            var offset = 0;

            while (0 < length)
            {
                var count = Min.For(l, length);
                var line = new StringBuilder($"{offset:X08}");

                for (var position = 0; position < count; position++)
                {
                    if (8 == position)
                    {
                        line.Append(' ', 1);
                    }

                    line.Append(' ', 1).Append($"{bytes[offset + position]:X02}");
                }

                if (l > count)
                {
                    line.Append(' ', (l - count) * 3);

                    if (8 > count)
                    {
                        line.Append(' ', 1);
                    }
                }

                line.Append(' ', 2);

                for (var position = 0; position < count; position++)
                {
                    var sym = (char) bytes[offset + position];
                    line.Append(Char.IsLetterOrDigit(sym) ? sym : '.');
                }

                Debug.WriteLine(line.ToString());

                length -= count;
                offset += count;
            }
        }

    }
}