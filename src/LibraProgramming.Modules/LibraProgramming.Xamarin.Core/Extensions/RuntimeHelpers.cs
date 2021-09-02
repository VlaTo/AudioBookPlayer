using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LibraProgramming.Xamarin.Core.Extensions
{
    internal static class RuntimeHelpers
    {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntPtr GetArrayDataByteOffset<T>()
        {
            return TypeInfo<T>.ArrayDataByteOffset;
        }

        private static class TypeInfo<T>
        {
            public static readonly IntPtr ArrayDataByteOffset = MeasureArrayDataByteOffset();
            
            [Pure]
            private static IntPtr MeasureArrayDataByteOffset()
            {
                var array = new T[1];
                return ObjectMarshal.DangerousGetObjectDataByteOffset(array, ref array[0]);
            }
        }
    }
}