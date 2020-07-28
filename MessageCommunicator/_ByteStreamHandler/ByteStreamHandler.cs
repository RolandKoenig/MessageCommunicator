using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessageCommunicator
{
    public abstract class ByteStreamHandler
    {
        public abstract ConnectionState State { get; }

        public abstract bool IsRunning { get; }

        public abstract string LocalEndpointDescription { get; }

        public abstract string RemoteEndpointDescription { get; }

        /// <summary>
        /// A custom logger. If set, this delegate will be called with all relevant events.
        /// </summary>
        public IMessageCommunicatorLogger? Logger { get; internal set; }

        protected bool IsLoggerSet => this.Logger != null;

        public MessageRecognizer? MessageRecognizer { get; private set; }

        /// <summary>
        /// Start the communicator.
        /// </summary>
        internal Task StartAsync()
        {
            return this.StartInternalAsync();
        }

        protected abstract Task StartInternalAsync();

        /// <summary>
        /// Stops the communicator.
        /// </summary>
        internal Task StopAsync()
        {
            return this.StopInternalAsync();
        }

        protected abstract Task StopInternalAsync();

        /// <summary>
        /// Calls current logger with the given message.
        /// </summary>
        protected void Log(LoggingMessageType messageType, string message, string metaData = "", Exception? exception = null)
        {
            var logger = this.Logger;
            logger?.Log(new LoggingMessage(DateTime.UtcNow, messageType, metaData, message, exception));
        }

        internal void RegisterMessageRecognizer(MessageRecognizer messageRecognizer)
        {
            if (this.MessageRecognizer != null)
            {
                throw new InvalidOperationException(
                    $"There is already a MessageRecognizer assigned to this ByteStreamHandler!");
            }

            this.MessageRecognizer = messageRecognizer;
            this.MessageRecognizer.ByteStreamHandler = this;
        }

        public abstract Task<bool> SendAsync(ReadOnlyMemory<byte> buffer);
    }
}
