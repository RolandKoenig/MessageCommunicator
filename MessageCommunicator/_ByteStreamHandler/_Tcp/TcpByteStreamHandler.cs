using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Light.GuardClauses;
using MessageCommunicator.Util;

namespace MessageCommunicator
{
    /// <summary>
    /// This <see cref="IByteStreamHandler"/> implementation sends/receives bytes over a TCP socket.
    /// </summary>
    public abstract class TcpByteStreamHandler : ByteStreamHandler
    {
        private DateTime _lastSuccessfulConnectTimestampUtc;

        /// <summary>
        /// Gets or sets the <see cref="ReconnectWaitTimeGetter"/> which controls the wait time before reconnect after lost connections.
        /// </summary>
        public ReconnectWaitTimeGetter ReconnectWaitTimeGetter { get; set; }

        /// <inheritdoc />
        public override string RemoteEndpointDescription
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

        /// <inheritdoc />
        public override string LocalEndpointDescription
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
        /// Gets or sets the size of the receive buffer.
        /// </summary>
        public uint ReceiveBufferSize { get; set; } = 1024;

        /// <inheritdoc />
        public override DateTime LastSuccessfulConnectTimestampUtc => _lastSuccessfulConnectTimestampUtc;

        /// <summary>
        /// Creates a new <see cref="TcpByteStreamHandler"/>.
        /// </summary>
        protected TcpByteStreamHandler(ReconnectWaitTimeGetter reconnectWaitTimeGetter)
        {
            reconnectWaitTimeGetter.MustNotBeNull(nameof(reconnectWaitTimeGetter));

            this.ReconnectWaitTimeGetter = reconnectWaitTimeGetter;
        }

        /// <summary>
        /// A helper method
        /// </summary>
        /// <param name="errorCountSinceLastConnect"></param>
        /// <returns></returns>
        protected Task WaitByReconnectWaitTimeAsync(int errorCountSinceLastConnect)
        {
            errorCountSinceLastConnect.MustBeGreaterThanOrEqualTo(0, nameof(errorCountSinceLastConnect));

            var waitTime = this.ReconnectWaitTimeGetter.GetWaitTime(errorCountSinceLastConnect);
            if (waitTime <= TimeSpan.Zero)
            {
                return TcpAsyncUtil.DummyFinishedTask;
            }

            return Task.Delay(waitTime);
        }

        /// <summary>
        /// Tries to send the given message to the currently connected partner
        /// </summary>
        /// <param name="buffer">The bytes to be sent</param>
        /// <returns>True when sending was successful</returns>
        public override async Task<bool> SendAsync(ArraySegment<byte> buffer)
        {
            buffer.MustNotBeDefault(nameof(buffer));

            if(buffer.Count <= 0)
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
                if (this.IsLoggerSet)
                {
                    this.Log(
                        LoggingMessageType.Info,
                        StringBuffer.Format("Error while sending message: {0}", sendException.Message));
                }
                return false;
            }
        }

        /// <inheritdoc />
        public override void TriggerReconnect()
        {
            this.Log(LoggingMessageType.Info, "Triggering reconnect...!");

            var currentClient = this.GetCurrentSendSocket();
            if(currentClient == null)
            {
                this.Log(LoggingMessageType.Info, "Reconnect not possible because there is no open connection");
                return;
            }

            TcpAsyncUtil.SafeDispose(currentClient.Client);
            this.Log(LoggingMessageType.Info, "Successfully stopped connection");
        }

        /// <summary>
        /// Gets the current <see cref="TcpClient"/> object for sending.
        /// This method returns null when this <see cref="IByteStreamHandler"/> is not connected to a remote partner.
        /// </summary>
        /// <returns>The <see cref="TcpClient"/> object for sending</returns>
        protected abstract TcpClient? GetCurrentSendSocket();
         
        /// <summary>
        /// Internal method which reacts on incoming bytes on the currently active tcp client connection.
        /// Only one of this connection is active at one time.
        /// </summary>
        protected async Task RunReceiveLoopAsync(TcpClient tcpClient, IPEndPoint localEndPoint, IPEndPoint partnerEndPoint, CancellationToken cancelToken)
        {
            tcpClient.MustNotBeNull(nameof(tcpClient));
            localEndPoint.MustNotBeNull(nameof(localEndPoint));
            partnerEndPoint.MustNotBeNull(nameof(partnerEndPoint));

            _lastSuccessfulConnectTimestampUtc = DateTime.UtcNow;

            var localEndPointStr = localEndPoint.ToString();
            var partnerEndPointStr = partnerEndPoint.ToString();
            var newConnection = true;

            if (this.IsLoggerSet)
            {
                this.Log(
                    LoggingMessageType.Info,
                    StringBuffer.Format("Starting receive loop for connection {0} to {1}", localEndPointStr, partnerEndPointStr));
            }

            var tcpClientInternal = tcpClient;
            var receiveBuffer = new byte[this.ReceiveBufferSize];
            while (!cancelToken.IsCancellationRequested)
            {
                int lastReceiveResult;
                try
                {
                    // Read next bytes
#if NETSTANDARD2_0
                    lastReceiveResult = await tcpClient.Client
                        .ReceiveAsync(new ArraySegment<byte>(receiveBuffer), SocketFlags.None)
                        .ConfigureAwait(false);
#else
                    lastReceiveResult = await tcpClient.Client
                        .ReceiveAsync(new Memory<byte>(receiveBuffer), SocketFlags.None)
                        .ConfigureAwait(false);
#endif

                    // Reset receive result if we where canceled already
                    if (lastReceiveResult == 0)
                    {
                        this.Log(LoggingMessageType.Info, "Connection closed by remote partner");
                    }
                    else if (cancelToken.IsCancellationRequested)
                    {
                        this.Log(LoggingMessageType.Info, "Connection canceled by local program");
                        lastReceiveResult = 0;
                    }
                }
                catch (ObjectDisposedException)
                {
                    tcpClientInternal = null;
                    break;
                }
                catch (SocketException socketException)
                {
                    if (this.IsLoggerSet)
                    {
                        this.Log(
                            LoggingMessageType.Error,
                            StringBuffer.Format(
                                "Error while receiving bytes from {0}: (Code: {1}) {2}", 
                                partnerEndPointStr, 
                                socketException.SocketErrorCode.ToString(),
                                socketException.Message),
                            exception: socketException);
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
                            exception: ex);
                    }
                    break;
                }

                if (lastReceiveResult <= 0) { break; }
                this.ProcessReceivedBytes(newConnection, receiveBuffer, lastReceiveResult);

                newConnection = false;
            }

            // Ensure that the socket is closed after ending this method
            TcpAsyncUtil.SafeDispose(ref tcpClientInternal);
        }

        private void ProcessReceivedBytes(bool newConnection, byte[] buffer, int receivedBytesCount)
        {
            buffer.MustNotBeNull(nameof(buffer));

            var receiveBuffer = new ArraySegment<byte>(buffer, 0, receivedBytesCount);

            // Log currently received bytes
            if (this.IsLoggerSet)
            {
                this.Log(
                    LoggingMessageType.Info,
                    StringBuffer.Format("Received {0} bytes: {1}", receiveBuffer.Count, TcpAsyncUtil.ToHexString(receiveBuffer)));
            }

            // Notify received bytes
            var messageRecognizer = this.MessageRecognizer;
            messageRecognizer?.OnReceivedBytes(newConnection, receiveBuffer);
        }
    }
}
