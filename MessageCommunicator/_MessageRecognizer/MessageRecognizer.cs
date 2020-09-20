using System;
using System.Threading.Tasks;

namespace MessageCommunicator
{
    /// <summary>
    /// A <see cref="MessageRecognizer"/> is responsible to recognize incoming messages and for formatting
    /// outgoing messages.
    /// </summary>
    public abstract class MessageRecognizer : IMessageRecognizer
    {
        /// <summary>
        /// Gets or sets the <see cref="IByteStreamHandler"/> implementation to which to forward messages to be sent.
        /// </summary>
        public IByteStreamHandler? ByteStreamHandler { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IMessageReceiveHandler"/> implementation to which to forward all recognized messages.
        /// </summary>
        public IMessageReceiveHandler? ReceiveHandler { get; set; }

        /// <summary>
        /// Gets or sets a custom logger. If set, this delegate will be called with all relevant events.
        /// </summary>
        public IMessageCommunicatorLogger? Logger { get; set; }

        /// <summary>
        /// Sends the given message to the partner.
        /// </summary>
        /// <param name="rawMessage">The message to be sent.</param>
        /// <returns>True if sending was successful, otherwise false.</returns>
        /// <exception cref="ArgumentException">Invalid message.</exception>
        public Task<bool> SendAsync(ReadOnlySpan<char> rawMessage)
        {
            var byteStreamHandler = this.ByteStreamHandler;
            if (byteStreamHandler == null) { return Task.FromResult(false); }

            return this.SendInternalAsync(byteStreamHandler, rawMessage);
        }

        protected abstract Task<bool> SendInternalAsync(IByteStreamHandler byteStreamHandler, ReadOnlySpan<char> rawMessage);

        /// <inheritdoc />
        public abstract void OnReceivedBytes(bool isNewConnection, ArraySegment<byte> receivedBytes);
    }
}
