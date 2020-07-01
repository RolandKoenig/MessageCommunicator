using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace TcpCommunicator.Util
{
    public sealed unsafe partial class StringBuffer
    {
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

        public void Append(ArraySegment<byte> bytes, Encoding encoding)
        {
            if (bytes.Array == null) { return; }
            if (bytes.Count == 0) { return; }

            var charCount = encoding.GetCharCount(
                bytes.Array, bytes.Offset, bytes.Count);
            this.CheckCapacity(charCount);

            encoding.GetChars(
                bytes.Array, bytes.Offset, bytes.Count,
                _buffer, _currentCount);
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
