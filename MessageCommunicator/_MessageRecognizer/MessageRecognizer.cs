using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MessageCommunicator
{
    public abstract class MessageRecognizer : IByteResponseProcessor
    {
        public ByteStreamHandler? ByteStreamHandler { get; internal set; }

        public IMessageReceiveHandler? ReceiveHandler { get; internal set; }

        public IMessageCommunicatorLogger? Logger { get; internal set; }

        public Task<bool> SendAsync(ReadOnlySpan<char> rawMessage)
        {
            var byteStreamHandler = this.ByteStreamHandler;
            if (byteStreamHandler == null) { return Task.FromResult(false); }

            return this.SendInternalAsync(byteStreamHandler, rawMessage);
        }

        protected abstract Task<bool> SendInternalAsync(ByteStreamHandler byteStreamHandler, ReadOnlySpan<char> rawMessage);

        /// <inheritdoc />
        public abstract void OnReceivedBytes(bool isNewConnection, ReadOnlySpan<byte> receivedBytes);
    }
}
