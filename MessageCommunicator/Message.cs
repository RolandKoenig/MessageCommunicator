using System;
using System.Collections.Generic;
using MessageCommunicator.Util;

namespace MessageCommunicator
{
    public class Message
    {
        internal StringBuffer RawMessage { get; }

        /// <summary>
        /// Returns true if this message is located inside the <see cref="MessagePool"/> currently.
        /// </summary>
        public bool IsMessagePooled { get; internal set; }

        /// <summary>
        /// Gets the total count of characters inside the message.
        /// </summary>
        public int Count => this.RawMessage.Count;

        /// <summary>
        /// Gets the total count of characters reserved in memory for this message.
        /// </summary>
        public int Capacity => this.RawMessage.Capacity;

        public Message(int capacity)
        {
            this.RawMessage = new StringBuffer(capacity);
        }

        public Message(string rawMessage)
        {
            this.RawMessage = new StringBuffer(rawMessage);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return this.RawMessage.ToString();
        }

        /// <summary>
        /// Gets a span for given offset and length.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <exception cref="IndexOutOfRangeException">Given offset and length do not match dimension of this message.</exception>
        public Span<char> GetSpan(int offset, int length)
        {
            return this.RawMessage.GetPart(offset, length);
        }

        public Span<char> GetSpan()
        {
            return this.RawMessage.GetPart(0, this.RawMessage.Count);
        }

        public ReadOnlySpan<char> GetSpanReadOnly(int offset, int length)
        {
            return this.RawMessage.GetPart(offset, length);
        }

        public ReadOnlySpan<char> GetSpanReadOnly()
        {
            return this.RawMessage.GetPart(0, this.RawMessage.Count);
        }

        /// <summary>
        /// Reserves memory for the given capacity.
        /// </summary>
        /// <param name="capacity">The total capacity you expect the message to grow to.</param>
        /// <exception cref="InvalidOperationException">The message is cached inside the pool currently.</exception>
        public void EnsureCapacity(int capacity)
        {
            if (this.IsMessagePooled)
            {
                throw new InvalidOperationException("Unable to manipulate this message because it is cached inside the message pool!");
            }

            this.RawMessage.EnsureCapacity(capacity);
        }

        /// <summary>
        /// Clears this message.
        /// </summary>
        /// <exception cref="InvalidOperationException">The message is cached inside the pool currently.</exception>
        public void Clear()
        {
            if (this.IsMessagePooled)
            {
                throw new InvalidOperationException("Unable to manipulate this message because it is cached inside the message pool!");
            }

            this.RawMessage.Clear();
        }

        public void Append(string value)
        {
            this.RawMessage.Append(value);
        }

        /// <summary>
        /// Returns this message to the <see cref="MessagePool"/>.
        /// The caller has to ensure that this message is not used after returning it to the pool.
        /// </summary>
        public void ReturnToPool()
        {
            MessagePool.Return(this);
        }
    }
}
