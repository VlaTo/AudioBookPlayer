using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LibraProgramming.Memory.Core
{
    internal static class ObjectMarshal
    {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntPtr DangerousGetObjectDataByteOffset<T>(object obj, ref T data)
        {
            var rawObj = Unsafe.As<RawObjectData>(obj)!;
            ref byte r0 = ref rawObj.Data;
            ref byte r1 = ref Unsafe.As<T, byte>(ref data);

            return Unsafe.ByteOffset(ref r0, ref r1);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T DangerousGetObjectDataReferenceAt<T>(object obj, IntPtr offset)
        {
            var rawObj = Unsafe.As<RawObjectData>(obj)!;
            ref byte r0 = ref rawObj.Data;
            ref byte r1 = ref Unsafe.AddByteOffset(ref r0, offset);
            ref T r2 = ref Unsafe.As<byte, T>(ref r1);

            return ref r2;
        }

        [StructLayout(LayoutKind.Explicit)]
        private sealed class RawObjectData
        {
            [FieldOffset(0)]
#pragma warning disable SA1401 // Fields should be private
            public byte Data;
#pragma warning restore SA1401
        }
    }
}