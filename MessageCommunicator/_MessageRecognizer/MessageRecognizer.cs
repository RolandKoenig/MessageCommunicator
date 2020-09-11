using System;
using System.Threading.Tasks;

namespace MessageCommunicator
{
    public abstract class MessageRecognizer : IMessageRecognizer
    {
        public IByteStreamHandler? ByteStreamHandler { get; set; }

        public IMessageReceiveHandler? ReceiveHandler { get; set; }

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
        public abstract void OnReceivedBytes(bool isNewConnection, ReadOnlySpan<byte> receivedBytes);
    }
}
