using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LibraProgramming.Xamarin.Core.Extensions
{
    public static class ArrayExtensions
    {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T DangerousGetReferenceAt<T>(this T[] array, int offset)
        {
            IntPtr ptr = RuntimeHelpers.GetArrayDataByteOffset<T>();
            ref T r0 = ref ObjectMarshal.DangerousGetObjectDataReferenceAt<T>(array, ptr);
            ref T ri = ref Unsafe.Add(ref r0, offset);

            return ref ri;
        }
    }
}