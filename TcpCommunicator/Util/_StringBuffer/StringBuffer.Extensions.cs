using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace TcpCommunicator.Util
{
    public sealed unsafe partial class StringBuffer
    {
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

        public ArraySegment<char> GetPart(int offset, int count)
        {
            return new ArraySegment<char>(
                _buffer,
                offset, count);
        }

        public void Append(ArraySegment<char> arraySegment)
        {
            if (arraySegment.Array == null)
            {
                throw new ArgumentNullException(nameof(arraySegment.Array));
            }
            if (arraySegment.Offset < 0 || arraySegment.Offset + arraySegment.Count > arraySegment.Array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(arraySegment.Offset));
            }

            fixed (char* s = &arraySegment.Array[arraySegment.Offset])
            {
                this.Append(s, arraySegment.Count);
            }
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
