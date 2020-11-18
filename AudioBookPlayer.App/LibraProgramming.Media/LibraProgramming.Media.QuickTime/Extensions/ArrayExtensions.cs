using System;

namespace LibraProgramming.Media.QuickTime.Extensions
{
    internal static class ArrayExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static byte[] Slice(this byte[] source, int startIndex) => Slice(source, startIndex, source.Length - startIndex);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static byte[] Slice(this byte[] source, int startIndex, int count)
        {
            var size = Math.Min(count, source.Length - startIndex);
            var array = new byte[size];

            //Array.Copy(source, 0, array, 0, size);
            Array.Copy(source, startIndex, array, 0, size);

            return array;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static byte[] ToBigEndian(this byte[] source)
        {
            if (false == BitConverter.IsLittleEndian)
            {
                return source;
            }

            var length = source.Length;
            var buffer = new byte[length];

            for (var index = 0; index < length; index++)
            {
                buffer[index] = source[length - index - 1];
            }

            return buffer;
        }
    }
}