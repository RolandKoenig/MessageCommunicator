using System;
using System.Collections.Generic;
using System.Text;

// Type aliases for supporting lower .net standard
#if NETSTANDARD1_3
using ReadOnlySpanOfByte = MessageCommunicator.ReadOnlySegment<byte>;
#else
using ReadOnlySpanOfByte = System.ReadOnlySpan<byte>;
#endif

namespace MessageCommunicator
{
    public interface IMessageRecognizer
    {
        /// <summary>
        /// Gets or sets the <see cref="IByteStreamHandler"/> which will be called inside the send method.
        /// </summary>
        IByteStreamHandler? ByteStreamHandler { get; set; }

        /// <summary>
        /// Notifies received bytes.
        /// Be careful, this method is called from the receive event of the <see cref="TcpByteStreamHandler"/> loop.
        /// Ensure that you block the calling thread as short as possible.
        /// </summary>
        /// <param name="isNewConnection">This flag is set to true when the given bytes are the first ones from a new connection. Typically this triggers receive buffer cleanup before processing received bytes.</param>
        /// <param name="receivedBytes">A span containing all received bytes.</param>
        void OnReceivedBytes(bool isNewConnection, ReadOnlySpanOfByte receivedBytes);
    }
}
