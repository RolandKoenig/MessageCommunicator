using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TcpCommunicator
{
    public abstract class MessageRecognizer : IByteResponseProcessor
    {
        public ByteStreamHandler ByteStreamHandler { get; }

        public IMessageReceiveHandler? ReceiveHandler { get; set; }

        protected MessageRecognizer(ByteStreamHandler byteStreamHandler)
        {
            this.ByteStreamHandler = byteStreamHandler;

            byteStreamHandler.RegisterMessageRecognizer(this);
        }

        public abstract Task<bool> SendAsync(string rawMessage);

        /// <inheritdoc />
        public abstract void OnReceivedBytes(bool isNewConnection, ReadOnlySpan<byte> receivedBytes);
    }
}
