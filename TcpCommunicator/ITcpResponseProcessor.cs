using System;
using System.Collections.Generic;
using System.Buffers;

namespace TcpCommunicator
{
    public interface ITcpResponseProcessor
    {
        /// <summary>
        /// Notifies received bytes.
        /// Be careful, this method is called from the receive event of the <see cref="TcpCommunicatorBase"/> loop.
        /// Ensure that you block the calling thread as short as possible.
        /// </summary>
        /// <param name="isNewConnection">This flag is set to true when the given bytes are the first ones from a new connection. Typically this triggers receive buffer cleanup before processing received bytes.</param>
        /// <param name="receivedBytes">A span containing all received bytes.</param>
        void OnReceivedBytes(bool isNewConnection, ReadOnlySpan<byte> receivedBytes);
    }
}
