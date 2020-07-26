using System;
using System.Collections.Generic;
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

    //public interface IByteStreamHandler
    //{
    //    public IByteResponseProcessor? MessageRecognizer { get; }

    //    void RegisterResponseProcessor(IByteResponseProcessor messageRecognizer);

    //    Task<bool> SendAsync(ReadOnlyMemory<byte> messageToSend);
    //}

    public interface IMessageReceiveHandler
    {
        void OnMessageReceived(Message message);
    }

    public interface IByteResponseProcessor
    {
        /// <summary>
        /// Notifies received bytes.
        /// Be careful, this method is called from the receive event of the <see cref="TcpByteStreamHandler"/> loop.
        /// Ensure that you block the calling thread as short as possible.
        /// </summary>
        /// <param name="isNewConnection">This flag is set to true when the given bytes are the first ones from a new connection. Typically this triggers receive buffer cleanup before processing received bytes.</param>
        /// <param name="receivedBytes">A span containing all received bytes.</param>
        void OnReceivedBytes(bool isNewConnection, ReadOnlySpan<byte> receivedBytes);
    }

    public readonly struct LoggingMessage
    {
        public ByteStreamHandler Communicator { get; }

        public DateTime TimeStamp { get; }

        public LoggingMessageType MessageType { get; }

        public string MetaData { get; }

        public string Message { get; }

        public Exception? Exception { get; }

        public LoggingMessage(ByteStreamHandler communicator, DateTime timestamp, LoggingMessageType messageType, string metaData, string message, Exception? exception)
        {
            this.Communicator = communicator;
            this.TimeStamp = timestamp;
            this.MessageType = messageType;
            this.MetaData = metaData;
            this.Message = message;
            this.Exception = exception;
        }
    }
}
