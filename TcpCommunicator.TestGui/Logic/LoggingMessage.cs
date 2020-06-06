using System;
using System.Collections.Generic;
using System.Text;

namespace TcpCommunicator.TestGui.Logic
{
    public class LoggingMessage
    {
        public string Message { get; }

        public LoggingMessage(string message)
        {
            this.Message = message;
        }
    }
}