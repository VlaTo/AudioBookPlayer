using System;
using System.Buffers;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace LibraProgramming.Xamarin.Core
{
    internal static class MemoryStream
    {
        [Pure]
        public static Stream Create(ReadOnlyMemory<byte> memory, bool readOnly)
        {
            if (memory.IsEmpty)
            {
                // Return an empty stream if the memory was empty
                return new MemoryStream<ArrayOwner>(ArrayOwner.Empty, readOnly);
            }

            if (MemoryMarshal.TryGetArray(memory, out ArraySegment<byte> segment))
            {
                var arraySpanSource = new ArrayOwner(segment.Array!, segment.Offset, segment.Count);
                return new MemoryStream<ArrayOwner>(arraySpanSource, readOnly);
            }

            if (MemoryMarshal.TryGetMemoryManager<byte, MemoryManager<byte>>(memory, out var memoryManager, out int start, out int length))
            {
                MemoryManagerOwner memoryManagerSpanSource = new MemoryManagerOwner(memoryManager, start, length);
                return new MemoryStream<MemoryManagerOwner>(memoryManagerSpanSource, readOnly);
            }

            return ThrowNotSupportedExceptionForInvalidMemory();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidateDisposed(bool disposed)
        {
            if (disposed)
            {
                ThrowObjectDisposedException();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidatePosition(long position, int length)
        {
            if ((ulong) position > (ulong) length)
            {
                ThrowArgumentOutOfRangeExceptionForPosition();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidateBuffer(byte[] buffer, int offset, int count)
        {
            if (buffer is null)
            {
                ThrowArgumentNullExceptionForBuffer();
            }

            if (offset < 0)
            {
                ThrowArgumentOutOfRangeExceptionForOffset();
            }

            if (count < 0)
            {
                ThrowArgumentOutOfRangeExceptionForCount();
            }

            if (offset + count > buffer!.Length)
            {
                ThrowArgumentExceptionForLength();
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidateCanWrite(bool canWrite)
        {
            if (!canWrite)
            {
                ThrowNotSupportedException();
            }
        }

        public static long ThrowArgumentExceptionForSeekOrigin()
        {
            throw new ArgumentException("The input seek mode is not valid.", "origin");
        }

        public static Exception GetNotSupportedException()
        {
            return new NotSupportedException("The requested operation is not supported for this stream.");
        }

        public static void ThrowArgumentExceptionForEndOfStreamOnWrite()
        {
            throw new ArgumentException("The current stream can't contain the requested input data.");
        }

        public static void ThrowNotSupportedException()
        {
            throw GetNotSupportedException();
        }

        private static Stream ThrowNotSupportedExceptionForInvalidMemory()
        {
            throw new ArgumentException("The input instance doesn't have a valid underlying data store.");
        }
        
        private static void ThrowArgumentOutOfRangeExceptionForPosition()
        {
            throw new ArgumentOutOfRangeException(nameof(Stream.Position), "The value for the property was not in the valid range.");
        }

        private static void ThrowObjectDisposedException()
        {
            throw new ObjectDisposedException("source", "The current stream has already been disposed");
        }
        
        private static void ThrowArgumentNullExceptionForBuffer()
        {
            throw new ArgumentNullException("buffer", "The buffer is null.");
        }
        
        private static void ThrowArgumentOutOfRangeExceptionForOffset()
        {
            throw new ArgumentOutOfRangeException("offset", "Offset can't be negative.");
        }
        
        private static void ThrowArgumentOutOfRangeExceptionForCount()
        {
            throw new ArgumentOutOfRangeException("count", "Count can't be negative.");
        }

        private static void ThrowArgumentExceptionForLength()
        {
            throw new ArgumentException("The sum of offset and count can't be larger than the buffer length.", "buffer");
        }
    }

    internal class MemoryStream<TSource> : Stream
        where TSource : struct, ISpanOwner
    {
        private readonly bool readOnly;
        private TSource source;
        private int position;
        private bool disposed;

        public override bool CanRead
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => false == disposed;
        }

        public override bool CanSeek
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => false == disposed;
        }

        public override bool CanWrite
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => false == readOnly && false == disposed;
        }

        public override long Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                MemoryStream.ValidateDisposed(disposed);
                return source.Length;
            }
        }

        public override long Position
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                MemoryStream.ValidateDisposed(disposed);
                return position;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                MemoryStream.ValidateDisposed(disposed);
                MemoryStream.ValidatePosition(value, source.Length);

                position = unchecked((int)value);
            }
        }

        public MemoryStream(TSource source, bool readOnly)
        {
            this.source = source;
            this.readOnly = readOnly;
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            MemoryStream.ValidateDisposed(disposed);
            MemoryStream.ValidateBuffer(buffer, offset, count);

            var bytesAvailable = source.Length - position;
            var bytesCopied = Math.Min(bytesAvailable, count);

            var src = source.Span.Slice(position, bytesCopied);
            var dst = buffer.AsSpan(offset, bytesCopied);

            src.CopyTo(dst);

            position += bytesCopied;

            return bytesCopied;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            MemoryStream.ValidateDisposed(disposed);

            long index = origin switch
            {
                SeekOrigin.Begin => offset,
                SeekOrigin.Current => position + offset,
                SeekOrigin.End => source.Length + offset,
                _ => MemoryStream.ThrowArgumentExceptionForSeekOrigin()
            };

            MemoryStream.ValidatePosition(index, source.Length);

            position = unchecked((int)index);

            return index;
        }

        public override void SetLength(long value)
        {
            throw MemoryStream.GetNotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            MemoryStream.ValidateDisposed(this.disposed);
            MemoryStream.ValidateCanWrite(CanWrite);
            MemoryStream.ValidateBuffer(buffer, offset, count);

            var src = buffer.AsSpan(offset, count);
            var dst = source.Span.Slice(position);

            if (false == src.TryCopyTo(dst))
            {
                MemoryStream.ThrowArgumentExceptionForEndOfStreamOnWrite();
            }

            position += src.Length;
        }

        public sealed override int ReadByte()
        {
            MemoryStream.ValidateDisposed(disposed);

            if (position == source.Length)
            {
                return -1;
            }

            return source.Span[position++];
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled<int>(cancellationToken);
            }

            try
            {
                var result = Read(buffer, offset, count);
                return Task.FromResult(result);
            }
            catch (OperationCanceledException e)
            {
                return Task.FromCanceled<int>(e.CancellationToken);
            }
            catch (Exception e)
            {
                return Task.FromException<int>(e);
            }
        }

        public override void WriteByte(byte value)
        {
            MemoryStream.ValidateDisposed(disposed);
            MemoryStream.ValidateCanWrite(CanWrite);

            if (position == source.Length)
            {
                MemoryStream.ThrowArgumentExceptionForEndOfStreamOnWrite();
            }

            source.Span[position++] = value;
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled(cancellationToken);
            }

            try
            {
                Write(buffer, offset, count);
                return Task.CompletedTask;
            }
            catch (OperationCanceledException e)
            {
                return Task.FromCanceled(e.CancellationToken);
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled(cancellationToken);
            }

            return Task.CompletedTask;
        }

        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled(cancellationToken);
            }

            try
            {
                CopyTo(destination, bufferSize);
                return Task.CompletedTask;
            }
            catch (OperationCanceledException e)
            {
                return Task.FromCanceled(e.CancellationToken);
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            source = default;
        }
    }
}