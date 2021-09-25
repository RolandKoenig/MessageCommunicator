using System;
using System.Collections.Generic;
using Light.GuardClauses;
using MessageCommunicator.Util;

namespace MessageCommunicator
{
    /// <summary>
    /// This class provides some manipulation methods for a character based message.
    /// Also, there is an integration pooling mechanism using <see cref="MessagePool"/> class to avoid object allocations during communication between two partners.
    /// </summary>
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

        /// <summary>
        /// Creates a new <see cref="Message"/> with tie given capacity.
        /// </summary>
        /// <param name="capacity">The total count of characters that are expected to be in this <see cref="Message"/></param>
        public Message(int capacity)
        {
            this.RawMessage = new StringBuffer(capacity);
        }

        /// <summary>
        /// Creates a new <see cref="Message"/> with the given content.
        /// </summary>
        /// <param name="rawMessage">Initial content of the <see cref="Message"/>.</param>
        public Message(string rawMessage)
        {
            if (string.IsNullOrEmpty(rawMessage))
            {
                this.RawMessage = new StringBuffer();
            }
            else
            {
                this.RawMessage = new StringBuffer(rawMessage);
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return this.RawMessage.ToString();
        }

        /// <summary>
        /// Gets a <see cref="Span{T}"/> for given offset and length.
        /// </summary>
        /// <param name="offset">The offset where the <see cref="Span{T}"/> should start.</param>
        /// <param name="length">Total length for the <see cref="Span{T}"/>.</param>
        /// <exception cref="IndexOutOfRangeException">Given offset and length do not match dimension of this message.</exception>
        /// <exception cref="InvalidOperationException">The message is cached inside the pool currently.</exception>
        /// <returns>The requested <see cref="Span{T}"/> structure.</returns>
        public Span<char> GetSpan(int offset, int length)
        {
            if (this.IsMessagePooled)
            {
                throw new InvalidOperationException("Unable to manipulate this message because it is cached inside the message pool!");
            }

            return this.RawMessage.GetPart(offset, length);
        }

        /// <summary>
        /// Gets a <see cref="Span{T}"/> for the full content of this <see cref="Message"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">The message is cached inside the pool currently.</exception>
        /// <returns>The requested <see cref="Span{T}"/> structure.</returns>
        public Span<char> GetSpan()
        {
            if (this.IsMessagePooled)
            {
                throw new InvalidOperationException("Unable to manipulate this message because it is cached inside the message pool!");
            }

            return this.RawMessage.GetPart(0, this.RawMessage.Count);
        }

        /// <summary>
        /// Gets a <see cref="ReadOnlySpan{T}"/> for given offset and length.
        /// </summary>
        /// <param name="offset">The offset where the <see cref="ReadOnlySpan{T}"/> should start.</param>
        /// <param name="length">Total length for the <see cref="ReadOnlySpan{T}"/>.</param>
        /// <exception cref="IndexOutOfRangeException">Given offset and length do not match dimension of this message.</exception>
        /// <exception cref="InvalidOperationException">The message is cached inside the pool currently.</exception>
        /// <returns>The requested <see cref="ReadOnlySpan{T}"/> structure.</returns>
        public ReadOnlySpan<char> GetSpanReadOnly(int offset, int length)
        {
            if (this.IsMessagePooled)
            {
                throw new InvalidOperationException("Unable to manipulate this message because it is cached inside the message pool!");
            }

            return this.RawMessage.GetPartReadOnly(offset, length);
        }

        /// <summary>
        /// Gets a <see cref="ReadOnlySpan{T}"/> for the full content of this <see cref="Message"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">The message is cached inside the pool currently.</exception>
        /// <returns>The requested <see cref="ReadOnlySpan{T}"/> structure.</returns>
        public ReadOnlySpan<char> GetSpanReadOnly()
        {
            if (this.IsMessagePooled)
            {
                throw new InvalidOperationException("Unable to manipulate this message because it is cached inside the message pool!");
            }

            return this.RawMessage.GetPartReadOnly(0, this.RawMessage.Count);
        }

        /// <summary>
        /// Overrides a part of this message with the given characters.
        /// </summary>
        public void OverridePart(int index, ReadOnlySpan<char> characters)
        {
            characters.MustBeShorterThanOrEqualTo(this.RawMessage.Count, nameof(characters));
            index.MustBeGreaterThan(-1, nameof(index));
            index.MustBeLessThanOrEqualTo(this.RawMessage.Count - characters.Length, nameof(index));

            var spanToOverride = this.GetSpan(index, characters.Length);
            characters.CopyTo(spanToOverride);
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

        /// <summary>
        /// Appends the given <see cref="string"/> to this <see cref="Message"/>.
        /// </summary>
        /// <param name="value">The value to be added.</param>
        public void Append(string value)
        {
            this.RawMessage.Append(value);
        }

        /// <summary>
        /// Appends a string subset to the current buffer.
        /// </summary>
        /// <param name="value">The string to append.</param>
        /// <param name="startIndex">The starting index within the string to begin reading characters.</param>
        /// <param name="count">The number of characters to append.</param>
        public void Append(string value, int startIndex, int count)
        {
            this.RawMessage.Append(value, startIndex, count);
        }

        /// <summary>
        /// Appends an array of characters to the current buffer.
        /// </summary>
        /// <param name="values">The characters to append.</param>
        /// <param name="startIndex">The starting index within the array to begin reading characters.</param>
        /// <param name="count">The number of characters to append.</param>
        public void Append(char[] values, int startIndex, int count)
        {
            this.RawMessage.Append(values, startIndex, count);
        }

        /// <summary>
        /// Appends a character to the current buffer.
        /// </summary>
        /// <param name="c">The character to append.</param>
        public void Append(char c)
        {
            this.RawMessage.Append(c);
        }

        /// <summary>
        /// Appends a character to the current buffer several times.
        /// </summary>
        /// <param name="c">The character to append.</param>
        /// <param name="count">The number of times to append the character.</param>
        public void Append(char c, int count)
        {
            this.RawMessage.Append(c, count);
        }

        /// <summary>
        /// Appends the specified value as a string to the current buffer.
        /// </summary>
        /// <param name="value">The value to append.</param>
        public void Append(bool value)
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
