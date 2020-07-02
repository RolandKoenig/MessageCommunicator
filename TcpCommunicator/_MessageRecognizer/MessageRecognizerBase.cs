using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TcpCommunicator
{
    public abstract class MessageRecognizerBase
    {
        public ITcpCommunicator Communicator { get; }

        protected MessageRecognizerBase(ITcpCommunicator communicator)
        {
            this.Communicator = communicator;
        }

        public abstract Task SendAsync(string rawMessage);
    }
}
