using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

// Type aliases for supporting lower .net standard
#if NETSTANDARD1_3
using ReadOnlySpanOfByte = MessageCommunicator.ReadOnlySegment<byte>;
using ReadOnlySpanOfChar = MessageCommunicator.ReadOnlySegment<char>;
#else
using ReadOnlySpanOfByte = System.ReadOnlySpan<byte>;
using ReadOnlySpanOfChar = System.ReadOnlySpan<char>;
#endif

namespace MessageCommunicator.Util
{
    internal sealed unsafe partial class StringBuffer
    {
        public int Capacity => _buffer.Length;

        public StringBuffer(string template)
            : this(template.Length)
        {
            this.Append(template);
        }

        public void RemoveFromStart(int countCharsToRemove)
        {
            if(countCharsToRemove > _currentCount){ throw new IndexOutOfRangeException(); }

            var remaining = _currentCount - countCharsToRemove;
            if (remaining == 0)
            {
                this.Clear();
                return;
            }

            Array.Copy(_buffer, countCharsToRemove, _buffer, 0, remaining);
            _currentCount = remaining;
        }

        public Span<char> GetPart(int offset, int count)
        {
            if (offset + count > _currentCount)
            {
                throw new IndexOutOfRangeException();
            }

            return new Span<char>(
                _buffer,
                offset, count);
        }

        public ReadOnlySpanOfChar GetPartReadOnly(int offset, int count)
        {
            if (offset + count > _currentCount)
            {
                throw new IndexOutOfRangeException();
            }

            return new ReadOnlySpanOfChar(
                _buffer,
                offset, count);
        }

        public int Append(ReadOnlySpanOfChar span)
        {
            if (span.Length <= 0)
            {
                throw new ArgumentNullException(nameof(span));
            }

            this.CheckCapacity(span.Length);
            for(var indexSource=0; indexSource<span.Length; indexSource++)
            {
                _buffer[_currentCount + indexSource] = span[indexSource];
            }
            _currentCount += span.Length;

            return span.Length;
        }

        public int Append(ReadOnlySpanOfByte bytes, Decoder decoder)
        {
            if (bytes.Length <= 0) { return 0; }

#if NETSTANDARD1_3
            var array = bytes.Array;
            var charCount = decoder.GetCharCount(bytes.Array, bytes.Offset, bytes.Length);
            this.CheckCapacity(charCount);

            decoder.GetChars(
                bytes.Array, bytes.Offset, bytes.Length,
                _buffer, _currentCount, false);
#else
            var charCount = decoder.GetCharCount(bytes, false);
            this.CheckCapacity(charCount);

            decoder.GetChars(
                bytes, 
                new Span<char>(_buffer, _currentCount, charCount), 
                false);
#endif
            _currentCount += charCount;

            return charCount;
        }

        public void EnsureCapacity(int count)
        {
            if (_buffer.Length < count)
            {
                Array.Resize(ref _buffer, count);
            }
        }
    }
}
