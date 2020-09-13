using System;
using System.Threading.Tasks;

// Type aliases for supporting lower .net standard
#if NETSTANDARD1_3
using ReadOnlySpanOfByte = MessageCommunicator.ReadOnlySegment<byte>;
using ReadOnlySpanOfChar = MessageCommunicator.ReadOnlySegment<char>;
#else
using ReadOnlySpanOfByte = System.ReadOnlySpan<byte>;
using ReadOnlySpanOfChar = System.ReadOnlySpan<char>;
#endif

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
        public Task<bool> SendAsync(ReadOnlySpanOfChar rawMessage)
        {
            var byteStreamHandler = this.ByteStreamHandler;
            if (byteStreamHandler == null) { return Task.FromResult(false); }

            return this.SendInternalAsync(byteStreamHandler, rawMessage);
        }

        protected abstract Task<bool> SendInternalAsync(IByteStreamHandler byteStreamHandler, ReadOnlySpanOfChar rawMessage);

        /// <inheritdoc />
        public abstract void OnReceivedBytes(bool isNewConnection, ReadOnlySpanOfByte receivedBytes);
    }
}
