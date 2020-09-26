using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessageCommunicator
{
    /// <summary>
    /// A <see cref="IByteStreamHandler"/> is responsible for sending / receiving bytes to the connected partner. It also manages the connection, triggers reconnect after
    /// disconnect and so on.
    /// </summary>
    public interface IByteStreamHandler
    {        
        /// <summary>
        /// Gets or sets the <see cref="IMessageRecognizer"/> which gets notified when we received bytes from the connected partner.
        /// </summary>
        IMessageRecognizer? MessageRecognizer { get; set; }

        /// <summary>
        /// Sends the given bytes to the connected partner.
        /// </summary>
        /// <param name="buffer">The bytes to be sent.</param>
        /// <returns>True if sending was successful, otherwise false.</returns>
        Task<bool> SendAsync(ArraySegment<byte> buffer);

        /// <summary>
        /// Triggers reconnect in case of an established connection.
        /// </summary>
        void TriggerReconnect();
    }
}
