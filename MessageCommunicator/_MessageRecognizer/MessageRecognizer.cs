using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TcpCommunicator
{
    public abstract class MessageRecognizer : IByteResponseProcessor
    {
        public ByteStreamHandler? ByteStreamHandler { get; internal set; }

        public IMessageReceiveHandler? ReceiveHandler { get; internal set; }

        public IMessageCommunicatorLogger? Logger { get; internal set; }

        public Task<bool> SendAsync(string rawMessage)
        {
            var byteStreamHandler = this.ByteStreamHandler;
            if (byteStreamHandler == null) { return Task.FromResult(false); }

            return this.SendInternalAsync(byteStreamHandler, rawMessage);
        }

        protected abstract Task<bool> SendInternalAsync(ByteStreamHandler byteStreamHandler, string rawMessage);

        /// <inheritdoc />
        public abstract void OnReceivedBytes(bool isNewConnection, ReadOnlySpan<byte> receivedBytes);
    }
}
