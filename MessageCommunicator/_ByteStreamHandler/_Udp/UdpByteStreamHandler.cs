using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MessageCommunicator.Util;
using Light.GuardClauses;

namespace MessageCommunicator
{
    public class UdpByteStreamHandler : ByteStreamHandler
    {
        private const int RUNNING_LOOP_COUNTER_MAX = 1000;

        private readonly object _startStopLock;
        private int _runningLoopCounter;
        private bool _isRunning;
        private ConnectionState _connState;
        private UdpClient? _udpClient;
        private DateTime _lastConnectTimestampUtc;
        private DateTime _lastReceivedDataBlockTimestampUtc;

        public ushort ListeningPort { get; }

        public string RemoteHost { get; }

        public IPAddress RemoteIPAddress { get; }

        public ushort RemotePort { get; }

        /// <summary>
        /// Gets or sets the size of the receive buffer.
        /// </summary>
        public uint ReceiveBufferSize { get; set; } = 1024;

        /// <inheritdoc />
        public override ConnectionState State
        {
            get
            {
                if (!_isRunning) { return ConnectionState.Stopped; }
                return _connState;
            }
        }

        /// <inheritdoc />
        public override bool IsRunning => _isRunning;

        /// <inheritdoc />
        public override string LocalEndpointDescription
        {
            get
            {
                var currentSendSocket = _udpClient;
                try
                {
                    return currentSendSocket?.Client?.LocalEndPoint?.ToString() ?? string.Empty;
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
        }

        /// <inheritdoc />
        public override string RemoteEndpointDescription
        {
            get
            {
                var currentSendSocket = _udpClient;
                try
                {
                    return currentSendSocket?.Client?.RemoteEndPoint?.ToString() ?? string.Empty;
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
        }

        /// <inheritdoc />
        public override DateTime LastConnectTimestampUtc => _lastConnectTimestampUtc;

        /// <inheritdoc />
        public override DateTime LastReceivedDataBlockTimestampUtc => _lastReceivedDataBlockTimestampUtc;

        internal UdpByteStreamHandler(
            ushort listeningPort,
            IPAddress remoteIPAddress, ushort remotePort)
        {
            this.ListeningPort = listeningPort;
            this.RemoteHost = remoteIPAddress.ToString();
            this.RemoteIPAddress = remoteIPAddress;
            this.RemotePort = remotePort;

            _startStopLock = new object();
            _udpClient = null;
            _isRunning = false;
        }

        internal UdpByteStreamHandler(
            ushort listeningPort,
            string remoteHost, ushort remotePort)
        {
            this.ListeningPort = listeningPort;
            this.RemoteHost = remoteHost;
            this.RemoteIPAddress = IPAddress.None;
            this.RemotePort = remotePort;

            _startStopLock = new object();
            _udpClient = null;
            _isRunning = false;
        }

        /// <inheritdoc />
        protected override Task StartInternalAsync()
        {
            // Simple lock here to guard start and stop phase
            var loopId = 0;
            lock (_startStopLock)
            {
                if (_isRunning)
                {
                    throw new InvalidOperationException(
                        $"Unable to start {nameof(TcpActiveByteStreamHandler)} for host {this.RemoteHost} and port {this.RemotePort}: This object is started already!");
                }

                _isRunning = true;
                _connState = ConnectionState.Connecting;
                _runningLoopCounter++;
                if (_runningLoopCounter > RUNNING_LOOP_COUNTER_MAX)
                {
                    _runningLoopCounter = 1;
                }

                loopId = _runningLoopCounter;
            }

            this.RunConnectionMainLoop(loopId);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        protected override Task StopInternalAsync()
        {
            // Simple lock here to guard start and stop phase
            UdpClient? lastSendSocket = null;
            lock (_startStopLock)
            {
                if (!_isRunning)
                {
                    throw new InvalidOperationException(
                        $"Unable to stop {nameof(TcpActiveByteStreamHandler)} for host {this.RemoteHost} and port {this.RemotePort}: This object is stopped already!");
                }

                _isRunning = false;
                _connState = ConnectionState.Stopped;
                _runningLoopCounter++;
                if (_runningLoopCounter > RUNNING_LOOP_COUNTER_MAX)
                {
                    _runningLoopCounter = 1;
                }
                lastSendSocket = _udpClient;
                _udpClient = null;
            }

            // Dispose previous sockets
            TcpAsyncUtil.SafeDispose(ref lastSendSocket);

            this.Log(LoggingMessageType.Info, "UDP communication stopped");

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public override async Task<bool> SendAsync(ArraySegment<byte> buffer)
        {
            buffer.MustNotBeDefault(nameof(buffer));

            if(buffer.Count <= 0)
            {
                this.Log(LoggingMessageType.Error, "Unable to send message: Message is empty!");
                return false;
            }

            var currentClient = _udpClient;
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
                        StringBuffer.Format("Sent {0} bytes: {1}", buffer.Count, HexFormatUtil.ToHexString(buffer)));
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

        private async void RunConnectionMainLoop(int loopId)
        {
            this.Log(LoggingMessageType.Info, "TcpCommunicator started in active mode");

            // Ensure that we switch to the ThreadPool
            await Task.CompletedTask.ConfigureAwait(false);

            var reconnectErrorCount = 0;
            var remoteAddressStr = this.RemoteHost;
            while (loopId == _runningLoopCounter)
            {
                UdpClient? newClient = null;
                try
                {
                    _connState = ConnectionState.Connecting;
                    if (this.IsLoggerSet)
                    {
                        this.Log(
                            LoggingMessageType.Info,
                            StringBuffer.Format("Connecting to host {0} on port {1}", remoteAddressStr,
                                this.RemotePort));
                    }

                    newClient = new UdpClient(this.ListeningPort);
                    if (!ReferenceEquals(this.RemoteIPAddress, IPAddress.None))
                    {
                        newClient.Connect(this.RemoteIPAddress, this.RemotePort);
                    }
                    else
                    {
                        newClient.Connect(this.RemoteHost, this.RemotePort);
                    }
                    _udpClient = newClient;
                    reconnectErrorCount = 0;

                    _connState = ConnectionState.Connected;
                    if (this.IsLoggerSet)
                    {
                        this.Log(
                            LoggingMessageType.Info,
                            StringBuffer.Format("Successfully connected to host {0} on port {1}", remoteAddressStr,
                                this.RemotePort));
                    }
                }
                catch (Exception ex)
                {
                    TcpAsyncUtil.SafeDispose(ref newClient);
                    _udpClient = null;

                    if (this.IsLoggerSet)
                    {
                        this.Log(
                            LoggingMessageType.Error,
                            StringBuffer.Format("Error while connecting to host {0} on port {1}: {2}",
                                remoteAddressStr, this.RemotePort, ex.Message),
                            exception: ex);
                    }

                    _connState = ConnectionState.Connecting;
                    await Task.Delay(1000)
                        .ConfigureAwait(false);
                    //await this.WaitByReconnectWaitTimeAsync(reconnectErrorCount)
                    //    .ConfigureAwait(false);
                    reconnectErrorCount++;
                }
                if (_udpClient == null) { continue; }

                // Normal receive loop handling
                try
                {
                    await this.RunReceiveLoopAsync(
                        _udpClient, (IPEndPoint)_udpClient.Client.LocalEndPoint!,
                        (IPEndPoint)_udpClient.Client.RemoteEndPoint!, CancellationToken.None);
                }
                catch (Exception ex)
                {
                    if (this.IsLoggerSet)
                    {
                        this.Log(
                            LoggingMessageType.Error,
                            StringBuffer.Format(
                                "Error while running receive loop for host {0} on port {1}: {2}",
                                remoteAddressStr, this.RemotePort, ex.Message),
                            exception: ex);
                    }
                }

                // Client gets already disposed in receive loop
                _udpClient = null;
            }
        }

        /// <summary>
        /// Internal method which reacts on incoming bytes on the currently active tcp client connection.
        /// Only one of this connection is active at one time.
        /// </summary>
        protected async Task RunReceiveLoopAsync(UdpClient udpClient, IPEndPoint localEndPoint,
            IPEndPoint partnerEndPoint, CancellationToken cancelToken)
        {
            udpClient.MustNotBeNull(nameof(udpClient));
            localEndPoint.MustNotBeNull(nameof(localEndPoint));
            partnerEndPoint.MustNotBeNull(nameof(partnerEndPoint));

            _lastConnectTimestampUtc = DateTime.UtcNow;

            var localEndPointStr = localEndPoint.ToString();
            var partnerEndPointStr = partnerEndPoint.ToString();
            var newConnection = true;

            if (this.IsLoggerSet)
            {
                this.Log(
                    LoggingMessageType.Info,
                    StringBuffer.Format("Starting receive loop for connection {0} to {1}", localEndPointStr,
                        partnerEndPointStr));
            }

            var udpClientInternal = udpClient;
            var receiveBuffer = new byte[this.ReceiveBufferSize];
            while (!cancelToken.IsCancellationRequested)
            {
                int lastReceiveResult;
                try
                {
                    // Read next bytes
#if NETSTANDARD2_0
                    var receiveTask = udpClientInternal.Client.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), SocketFlags.None);
                    if (receiveTask.IsCompleted)
                    {
                        lastReceiveResult = receiveTask.Result;
                    }
                    else
                    {
                        lastReceiveResult = await receiveTask
                            .ConfigureAwait(false);
                    }
#else
                    var receiveTaskStruct =
                        udpClientInternal.Client.ReceiveAsync(new Memory<byte>(receiveBuffer), SocketFlags.None);
                    if (receiveTaskStruct.IsCompleted)
                    {
                        lastReceiveResult = receiveTaskStruct.Result;
                    }
                    else
                    {
                        lastReceiveResult = await receiveTaskStruct
                            .ConfigureAwait(false);
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
                        _lastReceivedDataBlockTimestampUtc = DateTime.UtcNow;
                    }
                }
                catch (ObjectDisposedException)
                {
                    udpClientInternal = null;
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
            TcpAsyncUtil.SafeDispose(ref udpClientInternal);
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
                    StringBuffer.Format("Received {0} bytes: {1}", receiveBuffer.Count,
                        HexFormatUtil.ToHexString(receiveBuffer)));
            }

            // Notify received bytes
            var messageRecognizer = this.MessageRecognizer;
            messageRecognizer?.OnReceivedBytes(newConnection, receiveBuffer);
        }
    }
}
