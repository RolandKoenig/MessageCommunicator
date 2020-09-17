using System;
using System.Collections.Generic;
using System.Text;

namespace MessageCommunicator
{
    internal static class MessageCommunicatorExtensions
    {
        /// <summary>
        /// Gets the total count of characters in the given <see cref="ArraySegment{T}"/>.
        /// </summary>
        /// <param name="decoder">The <see cref="Decoder"/> object which is created by an <see cref="Encoding"/>.</param>
        /// <param name="arraySegment">The <see cref="ArraySegment{T}"/> referencing all relevant bytes.</param>
        /// <returns>Total count of characters inside given buffer.</returns>
        public static int GetCharCount(this Decoder decoder, ArraySegment<byte> arraySegment)
        {
            return decoder.GetCharCount(
                arraySegment.Array ?? throw new ArgumentNullException(nameof(arraySegment)), 
                arraySegment.Offset, arraySegment.Count, false);
        }

        /// <summary>
        /// Parses all characters in the given <see cref="ArraySegment{T}"/>.
        /// </summary>
        /// <param name="decoder">The <see cref="Decoder"/> object which is created by an <see cref="Encoding"/>.</param>
        /// <param name="source">The <see cref="ArraySegment{T}"/> referencing all relevant bytes.</param>
        /// <param name="target">The <see cref="ArraySegment{T}"/> referencing to the target character buffer.</param>
        /// <returns>Total count of characters parsed.</returns>
        public static int GetChars(this Decoder decoder, ArraySegment<byte> source, ArraySegment<char> target)
        {
            return decoder.GetChars(
                source.Array ?? throw new ArgumentNullException(nameof(source)),
                source.Offset, source.Count,
                target.Array ?? throw new ArgumentNullException(nameof(target)),
                target.Offset,
                false);
        }
    }
}
