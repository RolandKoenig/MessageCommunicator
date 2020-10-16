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
        // Dummy continuation for ReceiveAsync calls after timeout
        private static readonly Action<Task<int>> s_dummyReceiveContinuation = (task) =>
        {
            try { _ = task.Result; }
            catch
            {
                // ignored
            }
        };

        private DateTime _lastSuccessfulConnectTimestampUtc;
        private DateTime _lastSuccessfulReceiveTimestampUtc;

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

        /// <summary>
        /// Connection will be closed when we don't receive anything in this period of time.
        /// </summary>
        public TimeSpan ReceiveTimeout { get; set; }

        /// <inheritdoc />
        public override DateTime LastConnectTimestampUtc => _lastSuccessfulConnectTimestampUtc;

        /// <inheritdoc />
        public override DateTime LastReceivedDataBlockTimestampUtc => _lastSuccessfulReceiveTimestampUtc;

        /// <summary>
        /// Creates a new <see cref="TcpByteStreamHandler"/>.
        /// </summary>
        protected TcpByteStreamHandler(ReconnectWaitTimeGetter reconnectWaitTimeGetter, TimeSpan receiveTimeout)
        {
            reconnectWaitTimeGetter.MustNotBeNull(nameof(reconnectWaitTimeGetter));

            this.ReconnectWaitTimeGetter = reconnectWaitTimeGetter;
            this.ReceiveTimeout = receiveTimeout;
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
                    var receiveTask = tcpClient.Client.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), SocketFlags.None);
                    if (receiveTask.IsCompleted)
                    {
                        lastReceiveResult = receiveTask.Result;
                    }
                    else if (this.ReceiveTimeout == TimeSpan.Zero)
                    {
                        lastReceiveResult = await receiveTask
                            .ConfigureAwait(false);
                    }
                    else
                    {
                        var timeoutTask = Task.Delay(this.ReceiveTimeout);
                        await Task.WhenAny(receiveTask, timeoutTask)
                            .ConfigureAwait(false);

                        if (receiveTask.IsCompleted)
                        {
                            lastReceiveResult = receiveTask.Result;
                        }
                        else
                        {
                            _ = receiveTask.ContinueWith(s_dummyReceiveContinuation);

                            if (this.IsLoggerSet)
                            {
                                this.Log(
                                    LoggingMessageType.Error,
                                    StringBuffer.Format(
                                        "Timeout while receiving from {0}!",
                                        partnerEndPointStr));
                            }
                            break;
                        }
                    }
#else
                    var receiveTaskStruct = tcpClient.Client.ReceiveAsync(new Memory<byte>(receiveBuffer), SocketFlags.None);
                    if (receiveTaskStruct.IsCompleted)
                    {
                        lastReceiveResult = receiveTaskStruct.Result;
                    }
                    else if (this.ReceiveTimeout == TimeSpan.Zero)
                    {
                        lastReceiveResult = await receiveTaskStruct
                            .ConfigureAwait(false);
                    }
                    else
                    {
                        var receiveTask = receiveTaskStruct.AsTask();
                        var timeoutTask = Task.Delay(this.ReceiveTimeout);
                        await Task.WhenAny(receiveTask, timeoutTask)
                            .ConfigureAwait(false);

                        if (receiveTask.IsCompleted)
                        {
                            lastReceiveResult = receiveTask.Result;
                        }
                        else 
                        {
                            _ = receiveTask.ContinueWith(s_dummyReceiveContinuation);

                            if (this.IsLoggerSet)
                            {
                                this.Log(
                                    LoggingMessageType.Error,
                                    StringBuffer.Format(
                                        "Timeout while receiving from {0}!",
                                        partnerEndPointStr));
                            }
                            break;
                        }
                    }
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
                    else
                    {
                        _lastSuccessfulReceiveTimestampUtc = DateTime.UtcNow;
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

        //private async Task ObserveConnection(TcpClient tcpClientToObserve, CancellationToken cancelToken)
        //{

        //    while (!cancelToken.IsCancellationRequested)
        //    {
                
        //    }
        //}

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
