using System;
using System.Collections.Generic;
using Light.GuardClauses;

namespace MessageCommunicator
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

    public interface IMessageReceiveHandler
    {
        void OnMessageReceived(Message message);
    }

    public interface IMessageCommunicatorLogger
    {
        void Log(LoggingMessage loggingMessage);
    }

    public readonly struct LoggingMessage
    {
        public DateTime TimeStamp { get; }

        public LoggingMessageType MessageType { get; }

        public string MetaData { get; }

        public string Message { get; }

        public Exception? Exception { get; }

        public LoggingMessage(DateTime timestamp, LoggingMessageType messageType, string metaData, string message, Exception? exception)
        {
            metaData.MustNotBeNull(nameof(metaData));
            message.MustNotBeNull(nameof(message));

            this.TimeStamp = timestamp;
            this.MessageType = messageType;
            this.MetaData = metaData;
            this.Message = message;
            this.Exception = exception;
        }
    }
}
