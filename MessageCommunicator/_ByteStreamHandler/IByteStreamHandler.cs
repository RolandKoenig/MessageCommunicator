using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessageCommunicator
{
    public interface IByteStreamHandler
    {        
        /// <summary>
        /// Gets or sets the <see cref="IMessageRecognizer"/> which gets notified when we received bytes from the connected partner.
        /// </summary>
        public IMessageRecognizer? MessageRecognizer { get; set; }

        /// <summary>
        /// Sends the given bytes to the connected partner.
        /// </summary>
        /// <param name="buffer">The bytes to be sent.</param>
        /// <returns>True if sending was successful, otherwise false.</returns>
        Task<bool> SendAsync(ArraySegment<byte> buffer);
    }
}
