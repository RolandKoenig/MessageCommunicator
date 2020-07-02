using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TcpCommunicator
{
    public enum LoggingMessageType
    {
        Info,

        Warning,

        Error
    }

    public enum ConnectionState
    {
        Stopped,

        Connecting,

        Connected,
    }

    public interface ITcpCommunicator
    {
        Action<ArraySegment<byte>> ReceiveHandler { get; set; }
        Task<bool> SendAsync(ArraySegment<byte> messageToSend);
    }

    public readonly struct LoggingMessage
    {
        public TcpCommunicatorBase Communicator { get; }

        public DateTime TimeStamp { get; }

        public LoggingMessageType MessageType { get; }

        public string Message { get; }

        public Exception? Exception { get; }

        public LoggingMessage(TcpCommunicatorBase communicator, DateTime timestamp, LoggingMessageType messageType, string message, Exception? exception)
        {
            this.Communicator = communicator;
            this.TimeStamp = timestamp;
            this.MessageType = messageType;
            this.Message = message;
            this.Exception = exception;
        }
    }
}
