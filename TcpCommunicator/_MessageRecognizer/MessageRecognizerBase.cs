using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TcpCommunicator
{
    public abstract class MessageRecognizerBase
    {
        public ITcpCommunicator Communicator { get; }

        public Action<Message>? ReceiveHandler { get; set; }

        protected MessageRecognizerBase(ITcpCommunicator communicator)
        {
            this.Communicator = communicator;
        }

        public abstract Task SendAsync(string rawMessage);
    }
}
