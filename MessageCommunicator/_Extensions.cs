using System;
using System.Collections.Generic;
using System.Text;

namespace MessageCommunicator
{
    internal static class MessageCommunicatorExtensions
    {
        public static int GetCharCount(this Decoder decoder, ArraySegment<byte> arraySegment, bool flush)
        {
            return decoder.GetCharCount(
                arraySegment.Array ?? throw new ArgumentNullException(nameof(arraySegment)), 
                arraySegment.Offset, arraySegment.Count, flush);
        }

        public static int GetChars(this Decoder decoder, ArraySegment<byte> source,
            ArraySegment<char> target, bool flush)
        {
            return decoder.GetChars(
                source.Array ?? throw new ArgumentNullException(nameof(source)),
                source.Offset, source.Count,
                target.Array ?? throw new ArgumentNullException(nameof(target)),
                target.Offset,
                flush);
        }
    }
}
