using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MessageCommunicator
{
#if NETSTANDARD1_3
    public readonly struct ReadOnlySegment<T> where T : unmanaged
    {
        public readonly ArraySegment<T> ArraySegment;

        public int Offset => this.ArraySegment.Offset;

        public int Length => this.ArraySegment.Count;

        public T[] Array => this.ArraySegment.Array;

        public T this[int index] => this.Array[index];

        public ReadOnlySpan<T> Span => new ReadOnlySpan<T>(this.ArraySegment.Array, this.ArraySegment.Offset, this.ArraySegment.Count);

        public ReadOnlySegment(T[] buffer)
        {
            this.ArraySegment = new ArraySegment<T>(buffer);
        }

        public ReadOnlySegment(T[] buffer, int offset, int count)
        {
            this.ArraySegment = new ArraySegment<T>(buffer, offset, count);
        }

        public ReadOnlySegment(ArraySegment<T> segment)
        {
            this.ArraySegment = segment;
        }
    }
#endif
}
