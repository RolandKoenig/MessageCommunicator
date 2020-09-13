using System;
using System.Collections.Generic;
using System.Text;

namespace MessageCommunicator
{
    public readonly struct ReadOnlySendOrReceiveBuffer<T> where T : unmanaged
    {
        public ReadOnlyMemory<T> ReadOnlyMemory { get; }
        public ArraySegment<T> ArraySegment { get; }

        public int Length => this.ReadOnlyMemory.Length;

        public ReadOnlySpan<T> Span => this.ReadOnlyMemory.Span;

        public ReadOnlySendOrReceiveBuffer(T[] buffer)
            : this(
                new ReadOnlyMemory<T>(buffer),
                new ArraySegment<T>(buffer))
        {

        }

        public ReadOnlySendOrReceiveBuffer(T[] buffer, int offset, int count)
            : this(
                new ReadOnlyMemory<T>(buffer, offset, count),
                new ArraySegment<T>(buffer, offset, count))
        {

        }

        public ReadOnlySendOrReceiveBuffer(ReadOnlyMemory<T> readOnlyMemory, ArraySegment<T> arraySegment)
        {
            this.ReadOnlyMemory = readOnlyMemory;
            this.ArraySegment = arraySegment;
        }
    }
}
