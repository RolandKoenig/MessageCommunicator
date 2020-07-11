using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TcpCommunicator
{
    public abstract class MessageRecognizerBase : ITcpResponseProcessor
    {
        public ITcpCommunicator Communicator { get; }

        public Action<Message>? ReceiveHandler { get; set; }

        protected MessageRecognizerBase(ITcpCommunicator communicator)
        {
            this.Communicator = communicator;

            communicator.RegisterResponseProcessor(this);
        }

        public abstract Task SendAsync(string rawMessage);

        /// <inheritdoc />
        public abstract void OnReceivedBytes(bool isNewConnection, ReadOnlySpan<byte> receivedBytes);
    }
}
