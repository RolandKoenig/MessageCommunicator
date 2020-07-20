﻿using System;
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

    public interface ITcpCommunicator
    {
        public ITcpResponseProcessor? ResponseProcessor { get; }

        void RegisterResponseProcessor(ITcpResponseProcessor responseProcessor);

        Task<bool> SendAsync(ReadOnlyMemory<byte> messageToSend);
    }

    public readonly struct LoggingMessage
    {
        public TcpCommunicatorBase Communicator { get; }

        public DateTime TimeStamp { get; }

        public LoggingMessageType MessageType { get; }

        public string MetaData { get; }

        public string Message { get; }

        public Exception? Exception { get; }

        public LoggingMessage(TcpCommunicatorBase communicator, DateTime timestamp, LoggingMessageType messageType, string metaData, string message, Exception? exception)
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
