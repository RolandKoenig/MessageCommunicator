using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

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

        public void Append(ReadOnlySpan<char> span)
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
        }

        public void Append(ReadOnlySpan<byte> bytes, Encoding encoding)
        {
            if (bytes.Length <= 0) { return; }

            var charCount = encoding.GetCharCount(bytes);
            this.CheckCapacity(charCount);

            encoding.GetChars(
                bytes, new Span<char>(_buffer, _currentCount, charCount));

            _currentCount += charCount;
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
