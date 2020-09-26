using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Light.GuardClauses;

namespace MessageCommunicator
{
    /// <summary>
    /// A <see cref="ByteStreamHandler"/> is responsible for sending / receiving bytes to the connected partner. It also manages the connection, triggers reconnect after
    /// disconnect and so on.
    /// </summary>
    public abstract class ByteStreamHandler : IByteStreamHandler
    {
        /// <summary>
        /// Gets the current state of the connection.
        /// </summary>
        public abstract ConnectionState State { get; }

        /// <summary>
        /// Returns true if this instance is in running state, otherwise false.
        /// </summary>
        public abstract bool IsRunning { get; }

        /// <summary>
        /// Gets a short description of the local endpoint when started / connected.
        /// </summary>
        public abstract string LocalEndpointDescription { get; }

        /// <summary>
        /// Gets a short description of the remote endpoint when started / connected.
        /// </summary>
        public abstract string RemoteEndpointDescription { get; }

        /// <summary>
        /// Gets or sets a custom logger. If set, this delegate will be called with all relevant events.
        /// </summary>
        public IMessageCommunicatorLogger? Logger { get; set; }

        protected bool IsLoggerSet => this.Logger != null;

        /// <summary>
        /// The <see cref="IMessageRecognizer"/> to which to forward all received bytes.
        /// </summary>
        public IMessageRecognizer? MessageRecognizer { get; set; }

        /// <summary>
        /// Start this instance.
        /// </summary>
        internal Task StartAsync()
        {
            return this.StartInternalAsync();
        }

        protected abstract Task StartInternalAsync();

        /// <summary>
        /// Stops this instance.
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
            // ## Default implementation polls the State property

            // Fast path when connection is established already
            if (this.State == ConnectionState.Connected) { return; }

            // Some configuration for polling
            const int MAX_WAIT_TIME = 1000;
            var currentWaitTime = 100;

            // Loop for polling connection state
            while (!cancelToken.IsCancellationRequested)
            {
                await Task.Delay(currentWaitTime);
                if (currentWaitTime < MAX_WAIT_TIME)
                {
                    currentWaitTime *= 2;
                    if (currentWaitTime > MAX_WAIT_TIME) { currentWaitTime = MAX_WAIT_TIME; }
                }

                if (this.State == ConnectionState.Connected)
                {
                    // State is on Connected, so finish the task
                    return;
                }
            }
        }

        /// <summary>
        /// Calls current logger with the given message.
        /// </summary>
        protected void Log(LoggingMessageType messageType, string message, string metaData = "", Exception? exception = null)
        {
            message.MustNotBeNullOrEmpty(nameof(message));
            metaData.MustNotBeNull(nameof(metaData));

            var logger = this.Logger;
            logger?.Log(new LoggingMessage(DateTime.UtcNow, messageType, metaData, message, exception));
        }

        /// <inheritdoc />
        public abstract Task<bool> SendAsync(ArraySegment<byte> buffer);

        /// <inheritdoc />
        public abstract void TriggerReconnect();
    }
}
