using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TcpCommunicator
{
    public abstract class MessageRecognizerBase
    {
        public TcpCommunicatorBase Communicator { get; }

        protected MessageRecognizerBase(TcpCommunicatorBase communicator)
        {
            this.Communicator = communicator;
        }

        public abstract Task SendAsync(string rawMessage);
    }
}
