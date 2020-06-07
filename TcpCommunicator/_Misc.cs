using System;
using System.Collections.Generic;
using System.Text;

namespace TcpCommunicator
{
    public enum LoggingMessageType
    {
        Info,

        Warning,

        Error
    }

    public struct LoggingMessage
    {
        public TcpCommunicatorBase Communicator { get; }

        public LoggingMessageType MessageType { get; }

        public string Message { get; }

        public Exception? Exception { get; }

        public LoggingMessage(TcpCommunicatorBase communicator, LoggingMessageType messageType, string message, Exception? exception)
        {
            this.Communicator = communicator;
            this.MessageType = messageType;
            this.Message = message;
            this.Exception = exception;
        }
    }
}
