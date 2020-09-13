using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessageCommunicator
{
    public abstract class ByteStreamHandler : IByteStreamHandler
    {
        public abstract ConnectionState State { get; }

        public abstract bool IsRunning { get; }

        public abstract string LocalEndpointDescription { get; }

        public abstract string RemoteEndpointDescription { get; }

        /// <summary>
        /// A custom logger. If set, this delegate will be called with all relevant events.
        /// </summary>
        public IMessageCommunicatorLogger? Logger { get; set; }

        protected bool IsLoggerSet => this.Logger != null;

        public IMessageRecognizer? MessageRecognizer { get; set; }

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
        /// Waits for successful connection with a partner.
        /// </summary>
        public virtual async Task WaitForConnectionAsync(CancellationToken cancelToken)
        {
            // Default implementation polls the State property

            // Fast path when connection is established already
            if (this.State == ConnectionState.Connected) { return; }

            const int MAX_WAIT_TIME = 1000;
            var currentWaitTime = 100;

            while (!cancelToken.IsCancellationRequested)
            {
                await Task.Delay(currentWaitTime);
                if (currentWaitTime < MAX_WAIT_TIME)
                {
                    currentWaitTime *= 2;
                    if (currentWaitTime > MAX_WAIT_TIME) { currentWaitTime = MAX_WAIT_TIME; }
                }

                if (this.State == ConnectionState.Connected) { return; }
            }
        }

        /// <summary>
        /// Calls current logger with the given message.
        /// </summary>
        protected void Log(LoggingMessageType messageType, string message, string metaData = "", Exception? exception = null)
        {
            var logger = this.Logger;
            logger?.Log(new LoggingMessage(DateTime.UtcNow, messageType, metaData, message, exception));
        }

        /// <inheritdoc />
        public abstract Task<bool> SendAsync(ReadOnlySendOrReceiveBuffer<byte> buffer);
    }
}
