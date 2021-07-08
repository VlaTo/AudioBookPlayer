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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool StartsWith(this byte[] array, byte[] value)
        {
            if (null == array)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (null == value)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (0 == value.Length)
            {
                throw new ArgumentException("", nameof(value));
            }

            if (array.Length < value.Length)
            {
                throw new InvalidOperationException("");
            }

            for (var index = 0; index < value.Length; index++)
            {
                if (array[index] != value[index])
                {
                    return false;
                }
            }

            return true;
        }
    }
}