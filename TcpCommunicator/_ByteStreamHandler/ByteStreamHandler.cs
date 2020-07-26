using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TcpCommunicator
{
    public abstract class ByteStreamHandler
    {
        public abstract ConnectionState State { get; }

        public abstract bool IsRunning { get; }

        /// <summary>
        /// A custom logger. If set, this delegate will be called with all relevant events.
        /// </summary>
        public Action<LoggingMessage>? Logger { get; set; }

        protected bool IsLoggerSet => this.Logger != null;

        public MessageRecognizer? MessageRecognizer { get; private set; }

        /// <summary>
        /// Start the communicator.
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// Stops the communicator.
        /// </summary>
        public abstract void Stop();

        /// <summary>
        /// Calls current logger with the given message.
        /// </summary>
        protected void Log(LoggingMessageType messageType, string message, string metaData = "", Exception? exception = null)
        {
            var logger = this.Logger;
            if (logger == null) { return; }

            var loggingMessage = new LoggingMessage(this, DateTime.UtcNow, messageType, metaData, message, exception);
            logger(loggingMessage);
        }

        internal void RegisterMessageRecognizer(MessageRecognizer messageRecognizer)
        {
            if (this.MessageRecognizer != null)
            {
                throw new InvalidOperationException(
                    $"There is already a MessageRecognizer assigned to this ByteStreamHandler!");
            }
            this.MessageRecognizer = messageRecognizer;
        }

        public abstract Task<bool> SendAsync(ReadOnlyMemory<byte> buffer);
    }
}
