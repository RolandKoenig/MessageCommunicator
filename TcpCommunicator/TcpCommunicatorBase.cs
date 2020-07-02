using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using TcpCommunicator.Util;

namespace TcpCommunicator
{
    public abstract class TcpCommunicatorBase : ITcpCommunicator
    {
        public ReconnectWaitTimeGetter ReconnectWaitTimeGetter { get; set; }

        public abstract bool IsRunning { get; }

        public abstract ConnectionState State { get; }

        public string RemoteEndpointDescription
        {
            get
            {
                var currentSendSocket = this.GetCurrentSendSocket();
                try
                {
                    return currentSendSocket?.Client?.RemoteEndPoint?.ToString() ?? string.Empty;
                }
                catch (Exception )
                {
                    return string.Empty;
                }
            }
        }

        public string LocalEndpointDescription
        {
            get
            {
                var currentSendSocket = this.GetCurrentSendSocket();
                try
                {
                    return currentSendSocket?.Client?.RemoteEndPoint?.ToString() ?? string.Empty;
                }
                catch (Exception )
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// A custom logger. If set, this delegate will be called with all relevant events.
        /// </summary>
        public Action<LoggingMessage>? Logger { get; set; }

        public Action<ArraySegment<byte>>? ReceiveHandler { get; set; }

        protected bool IsLoggerSet => this.Logger != null;

        public uint ReceiveBufferSize { get; set; } = 1024;

        protected TcpCommunicatorBase(ReconnectWaitTimeGetter? reconnectWaitTimeGetter)
        {
            this.ReconnectWaitTimeGetter = reconnectWaitTimeGetter ?? new FixedReconnectWaitTimeGetter(TimeSpan.FromSeconds(1.0));
        }

        /// <summary>
        /// Calls current logger with the given message.
        /// </summary>
        protected void Log(LoggingMessageType messageType, string message, Exception? exception = null)
        {
            var logger = this.Logger;
            if (logger == null) { return; }

            var loggingMessage = new LoggingMessage(this, DateTime.UtcNow, messageType, message, exception);
            logger(loggingMessage);
        }

        protected Task WaitByReconnectWaitTimeAsync(int errorCountSinceLastConnect)
        {
            var waitTime = this.ReconnectWaitTimeGetter.GetWaitTime(errorCountSinceLastConnect);
            if (waitTime <= TimeSpan.Zero)
            {
                return TcpAsyncUtil.DummyFinishedTask;
            }

            return Task.Delay(waitTime);
        }

        /// <summary>
        /// Start the communicator.
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// Stops the communicator.
        /// </summary>
        public abstract void Stop();

        /// <summary>
        /// Tries to send the given message to the currently connected partner
        /// </summary>
        /// <param name="buffer">The bytes to be sent</param>
        /// <returns>True when sending was successful</returns>
        public async Task<bool> SendAsync(ArraySegment<byte> buffer)
        {
            if ((buffer.Array == null) ||
                (buffer.Count == 0))
            {
                this.Log(LoggingMessageType.Error, "Unable to send message: Message is empty!");
                return false;
            }

            var currentClient = this.GetCurrentSendSocket();
            if(currentClient == null)
            {
                this.Log(LoggingMessageType.Error, "Unable to send message: Connection is not established currently!");
                return false;
            }

            try
            {
                await currentClient.Client.SendAsync(buffer, SocketFlags.None)
                    .ConfigureAwait(false);

                if (this.IsLoggerSet)
                {
                    this.Log(
                        LoggingMessageType.Info,
                        StringBuffer.Format("Sent {0} bytes: {1}", buffer.Count, TcpAsyncUtil.ToHexString(buffer)));
                }
                
                return true;
            }
            catch (Exception sendException)
            {
                return false;
            }
        }

        protected abstract TcpClient? GetCurrentSendSocket();
         
        /// <summary>
        /// Internal method which reacts on incoming bytes on the currently active tcp client connection.
        /// Only one of this connection is active at one time.
        /// </summary>
        protected async Task RunReceiveLoopAsync(TcpClient? tcpClient, IPEndPoint localEndPoint, IPEndPoint partnerEndPoint, CancellationToken cancelToken)
        {
            var localEndPointStr = localEndPoint.ToString();
            var partnerEndPointStr = partnerEndPoint.ToString();

            if (this.IsLoggerSet)
            {
                this.Log(
                    LoggingMessageType.Info,
                    StringBuffer.Format("Starting receive loop for connection {0} to {1}", localEndPointStr, partnerEndPointStr));
            }

            var receiveBuffer = new byte[this.ReceiveBufferSize];
            while ((!cancelToken.IsCancellationRequested) &&
                   (tcpClient != null))
            {
                int lastReceiveResult;
                try
                {
                    // Read next bytes
                    lastReceiveResult = await tcpClient.Client
                        .ReceiveAsync(new ArraySegment<byte>(receiveBuffer), SocketFlags.None)
                        .ConfigureAwait(false);
                    
                    // Reset receive result if we where canceled already
                    if (cancelToken.IsCancellationRequested)
                    {
                        lastReceiveResult = 0;
                    }
                }
                catch (ObjectDisposedException)
                {
                    tcpClient = null;
                    break;
                }
                catch (SocketException socketException)
                {
                    if (this.IsLoggerSet)
                    {
                        this.Log(
                            LoggingMessageType.Error,
                            StringBuffer.Format(
                                "Error while receiving bytes from {0}: (Code: {1} - {2}) {3}", 
                                partnerEndPointStr, 
                                socketException.ErrorCode, socketException.SocketErrorCode.ToString(),
                                socketException.Message),
                            socketException);
                    }
                    break;
                }
                catch (Exception ex)
                {
                    if (this.IsLoggerSet)
                    {
                        this.Log(
                            LoggingMessageType.Error,
                            StringBuffer.Format(
                                "Error while receiving bytes from {0}: {1}", 
                                partnerEndPointStr,
                                ex.Message),
                            ex);
                    }
                    break;
                }

                if (lastReceiveResult <= 0) { break; }
                var receivedSegment = new ArraySegment<byte>(receiveBuffer, 0, lastReceiveResult);

                // Log currently received bytes
                if (this.IsLoggerSet)
                {
                    this.Log(
                        LoggingMessageType.Info,
                        StringBuffer.Format("Received {0} bytes: {1}", receivedSegment.Count, TcpAsyncUtil.ToHexString(receivedSegment)));
                }

                // Notify received bytes
                var receiveHandler = this.ReceiveHandler;
                receiveHandler?.Invoke(receivedSegment);
            }

            // Ensure that the socket is closed after ending this method
            TcpAsyncUtil.SafeDispose(ref tcpClient);
        }
    }
}
